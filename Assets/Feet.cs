using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
	public float footstepVolume = 1f;
	public float footstepDistance = 1f;

	public bool grounded { get; private set; }

	float distanceTraveled;
	Vector3 prevPos;

    void OnEnable ()
    {
		distanceTraveled = 0;
        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update ()
    {
        grounded = Physics.Raycast(new Ray(transform.position + Vector3.up * 0.2f, Vector3.down), 0.4f);

		if (grounded)
			distanceTraveled += (transform.position - prevPos).magnitude;

		if (distanceTraveled > footstepDistance)
		{
			distanceTraveled = distanceTraveled % footstepDistance;
			AudioManager.PlaySoundEffect("Footstep", transform.position);
		}

		prevPos = transform.position;
    }
}
