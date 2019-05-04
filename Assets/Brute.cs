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
		animator.SetTrigger("Hurt");
	}

	protected void Update ()
	{
		if (alert)
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Player.local.transform.position - transform.position, Vector3.up), Time.deltaTime * 3);
	}
}

public class Brute : Enemy
{
	float running = 0;
	NavMeshAgent navigator;

    // Start is called before the first frame update
    new void Start ()
    {
        base.Start();

		killable.OnDeath += Killable_OnDeath;

		navigator = GetComponent<NavMeshAgent>();
    }

	private void Killable_OnDeath(object sender)
	{
		navigator.enabled = false;
		animator.SetTrigger("Death");
	}

	// Update is called once per frame
	new void Update ()
    {
        base.Update();

		if (alert)
		{
			navigator.destination = Player.local.transform.position;
		}

		animator.SetFloat("Run", Mathf.Clamp01(navigator.velocity.magnitude));
    }

	private void FixedUpdate ()
	{
		rigidbody.isKinematic = navigator.enabled;
	}
}
