﻿using System.Collections;
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

		if (!killable)
			return;

		bool isPlayer = killable == Player.local.killable;

		if (isPlayer && killPlayer)
			killable.Kill();
		else if (!isPlayer && killEnemies)
		{
			var enemy = killable.GetComponentInChildren<Enemy>();
			if (enemy)
				enemy.Explode();
			else
				killable.Kill();
		}
		else
			killable.Damage(damage);

		Debug.Log(other.name);
	}
}
