﻿using System.Collections;
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
}

public class Brute : Enemy
{
	float running = 0;
	NavMeshAgent navigator;
	CapsuleCollider collider;

	public bool isAttacking { get; private set; }

    // Start is called before the first frame update
    new void Start ()
    {
        base.Start();

		killable.OnDeath += Killable_OnDeath;
		killable.OnDamage += Killable_OnDamage;

		navigator = GetComponent<NavMeshAgent>();
		collider = GetComponent<CapsuleCollider>();

		isAttacking = false;
    }

	private void Killable_OnDamage (float damage)
	{
		Flinch();
		PoolManager.GetPooledObject("Effects", "BloodSplat", transform.position + Vector3.up * 1.5f);
	}

	public override void Flinch()
	{
		if (killable.isAlive)
		{
			StartCoroutine(DisableNavigator(0.5f));
			animator.SetTrigger("Hurt");
		}
		rigidbody.velocity += Vector3.up * 0.2f;
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

		animator.SetFloat("Run", Mathf.Clamp01(navigator.velocity.magnitude));
    }

	IEnumerator AttackSequence ()
	{
		navigator.enabled = false;
		isAttacking = true;
		animator.SetTrigger("Attack");
		yield return new WaitForSeconds(0.5f);

		// Do damage
		if (Vector3.Distance(Player.local.transform.position, transform.position) <= navigator.stoppingDistance)
			Player.local.killable.Damage(15);

		yield return new WaitForSeconds(0.2f);
		navigator.enabled = killable.isAlive;
		isAttacking = false;
	}

	private void FixedUpdate ()
	{
		rigidbody.isKinematic = navigator.enabled;
	}
}
