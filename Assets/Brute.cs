using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
	public bool alert;
	protected Animator animator;
	protected Killable killable;
	new protected Rigidbody rigidbody;

	protected void Start()
	{
		animator = GetComponentInChildren<Animator>();
		killable = GetComponentInChildren<Killable>();
		rigidbody = GetComponentInChildren<Rigidbody>();

		killable.OnDamage += Killable_OnDamage;
	}

	private void Killable_OnDamage (float damage)
	{
		
	}

	protected void Update ()
	{
		//if (alert)
		//	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.local.transform.position - transform.position, Vector3.up), Time.deltaTime * 3);
	}

	public abstract void Flinch ();
	public abstract void Explode ();
}

public class Brute : Enemy
{
	float running = 0;
	NavMeshAgent navigator;
	CapsuleCollider collider;
	Feet feet;

	public bool isAttacking { get; private set; }

	bool navigatorDisabled = false;

    // Start is called before the first frame update
    new void Start ()
    {
        base.Start();

		killable.OnDeath += Killable_OnDeath;
		killable.OnDamage += Killable_OnDamage;

		navigator = GetComponent<NavMeshAgent>();
		collider = GetComponent<CapsuleCollider>();
		feet = GetComponent<Feet>();

		isAttacking = false;
		navigatorDisabled = false;
    }

	private void Killable_OnDamage (float damage)
	{
		Flinch();
		PoolManager.GetPooledObject("Effects", "BloodSplat", transform.position + Vector3.up * 1.5f);
	}

	public override void Flinch ()
	{
		if (killable.isAlive)
		{
			StartCoroutine(DisableNavigator(0.5f));
			animator.SetTrigger("Hurt");
		}
		rigidbody.velocity += Vector3.up * 0.2f;
	}

	public override void Explode ()
	{
		PoolManager.GetPooledObject("Effects", "BruteExplosion", transform.position + Vector3.up * 1.5f);
		Destroy(gameObject);
	}

	IEnumerator DisableNavigator (float duration)
	{
		navigatorDisabled = true;
		yield return new WaitForSeconds(duration);
		navigatorDisabled = false;
	}

	private void Killable_OnDeath(object sender)
	{
		navigator.enabled = false;
		animator.SetTrigger("Death");
		gameObject.layer = LayerMask.NameToLayer("Debris");
		collider.radius = 0.2f;
		collider.height = 0.2f;
		collider.center = Vector3.up * 0.2f;
	}

	// Update is called once per frame
	new void Update ()
    {
        base.Update();

		if (alert && killable.isAlive)
		{
			if (!isAttacking && navigator.enabled)
			{
				if (!navigator.isOnNavMesh)
				{
					Destroy(gameObject);
					return;
				}

				navigator.destination = Player.local.transform.position;

				if (Vector3.Distance(transform.position, Player.local.transform.position) <= navigator.stoppingDistance)
				{
					StartCoroutine(AttackSequence());
				}
			}

			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.local.transform.position - transform.position, Vector3.up), Time.deltaTime * 2);
		}

		navigator.enabled = !navigatorDisabled && killable.isAlive && !isAttacking && feet.grounded;

		animator.SetFloat("Run", Mathf.Clamp01(navigator.velocity.magnitude));
    }

	IEnumerator AttackSequence ()
	{
		isAttacking = true;
		animator.SetTrigger("Attack");
		yield return new WaitForSeconds(0.4f);

		// Do damage
		if (Vector3.Distance(Player.local.transform.position, transform.position) <= navigator.stoppingDistance + 0.5f)
		{
			Player.local.killable.Damage(15);
			AudioManager.PlaySoundEffect("AxeHit", transform.position + Vector3.up * 1.5f);
		}
		else
		{
			AudioManager.PlaySoundEffect("AxeMiss", transform.position + Vector3.up * 1.5f);
		}

		yield return new WaitForSeconds(0.3f);
		isAttacking = false;
	}

	private void FixedUpdate ()
	{
		rigidbody.isKinematic = navigator.enabled;
	}
}
