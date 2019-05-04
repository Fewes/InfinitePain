using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
	public float width	= 3;
	public float length = 3;
	public float height = 3;

	public Bounds bounds
	{
		get
		{
			return new Bounds(transform.position, new Vector3(width, height, length));
		}
	}
	public bool playerInside
	{
		get
		{
			return bounds.Contains(Player.local.transform.position);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(width, height, length));
	}
}
