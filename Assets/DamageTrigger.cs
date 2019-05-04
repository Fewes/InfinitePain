using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
	public float damage = 10;
	public bool  kill	= false;

	private void OnTriggerEnter (Collider other)
	{
		var killable = other.GetComponentInChildren<Killable>();

		if (kill)
			killable.Kill();
		else
			killable.Damage(damage);

		Debug.Log(other.name);
	}
}
