using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
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
}

public class Brute : Enemy
{
	float running = 0;
	NavMeshAgent navigator;
	CapsuleCollider collider;

    // Start is called before the first frame update
    new void Start ()
    {
        base.Start();

		killable.OnDeath += Killable_OnDeath;
		killable.OnDamage += Killable_OnDamage;

		navigator = GetComponent<NavMeshAgent>();
		collider = GetComponent<CapsuleCollider>();
    }

	private void Killable_OnDamage (float damage)
	{
		if (killable.isAlive && navigator.enabled)
		{
			StartCoroutine(DisableNavigator(0.5f));
			animator.SetTrigger("Hurt");
		}
		rigidbody.velocity += Vector3.up * 0.1f;
	}

	IEnumerator DisableNavigator (float duration)
	{
		navigator.enabled = false;
		yield return new WaitForSeconds(duration);
		if (killable.isAlive)
			navigator.enabled = true;
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

		if (alert && killable.isAlive && navigator.enabled)
		{
			if (!navigator.isOnNavMesh)
			{
				Destroy(gameObject);
				return;
			}
			navigator.destination = Player.local.transform.position;
		}

		animator.SetFloat("Run", Mathf.Clamp01(navigator.velocity.magnitude));
    }

	private void FixedUpdate ()
	{
		rigidbody.isKinematic = navigator.enabled;
	}
}
