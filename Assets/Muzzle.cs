using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Muzzle : MonoBehaviour
{
	// Inspector
	public float bulletForce = 1f;
	public float bulletDamage = 10f;
	public Killable[] ignoreKillables;

	ParticleSystem system;
	List<ParticleCollisionEvent> events;

    void Start ()
    {
        system = GetComponent<ParticleSystem>();
		events = new List<ParticleCollisionEvent>();
    }

	void OnParticleCollision (GameObject other)
    {
        int numCollisionEvents = system.GetCollisionEvents(other, events);

        Rigidbody rb = other.GetComponent<Rigidbody>();
		Killable killable = other.GetComponentInChildren<Killable>();

		if (ignoreKillables != null)
		{
			foreach (var k in ignoreKillables)
			{
				if (k == killable)
					return;
			}
		}

		for (int i = 0; i < numCollisionEvents; i++)
		{
			Vector3 pos		= events[i].intersection;
            Vector3 force	= events[i].velocity * bulletForce;
			if (rb)
				rb.AddForceAtPosition(force, pos);
			if (killable)
			{
				killable.Damage(bulletDamage);
				PoolManager.GetPooledObject("Effects", "BloodSplatSmall", pos);
			}
		}
    }
}
