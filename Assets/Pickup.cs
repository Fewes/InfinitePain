using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	[System.Serializable]
	public enum Type
	{
		Health,
		Ammo
	}

	public Type type = Type.Health;

	Transform pivot;
	Bounds bounds;

    void OnEnable ()
    {
        pivot = transform.Find("Pivot");
		bounds = new Bounds(transform.position, Vector3.one * 2);
    }

    void Update ()
    {
		pivot.localPosition = Vector3.up * (Mathf.Sin(Time.time * 3) * 0.25f + 0.25f);
        pivot.localEulerAngles = -Vector3.up * Time.time * 180;

		bounds.center = transform.position;
		bool shouldDestroy = false;
		if (bounds.Contains(Player.local.transform.position))
		{
			switch (type)
			{
				case Type.Health:
					shouldDestroy = Player.RefillHealth(25f);
				break;
				case Type.Ammo:
					shouldDestroy = Player.RefillAmmo(8);
				break;
			}
		}

		if (shouldDestroy)
			Destroy(gameObject);
    }
}
