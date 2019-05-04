using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
	Transform doorL, doorR;

	private void OnEnable ()
	{
		doorL = transform.Find("DoorL");
		doorR = transform.Find("DoorR");

		GetComponentInParent<Room>().OnPlayerEntered += ExitPoint_OnPlayerEntered;
	}

	private void ExitPoint_OnPlayerEntered()
	{
		if (doorL && doorR)
		StartCoroutine(Open());
	}

	IEnumerator Open ()
	{
		while (doorL.localPosition.x > -2f)
		{
			yield return 0;
			doorL.localPosition += Vector3.left  * Time.deltaTime;
			doorR.localPosition += Vector3.right * Time.deltaTime;
		}
	}
}
