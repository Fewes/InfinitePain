using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
	public float damage			= 10;
	public bool  killPlayer		= false;
	public bool  killEnemies	= false;

	private void OnTriggerEnter (Collider other)
	{
		var killable = other.GetComponentInChildren<Killable>();

		bool isPlayer = killable == Player.local.killable;

		if ((isPlayer && killPlayer) || (!isPlayer && killEnemies))
			killable.Kill();
		else
			killable.Damage(damage);

		Debug.Log(other.name);
	}
}
