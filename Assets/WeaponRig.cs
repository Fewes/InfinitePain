using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRig : MonoBehaviour
{
	// Inspector
	[SerializeField]
	AnimationCurve		m_ReloadCurve;

	public bool reloading { get; private set; }  = false;

	float recoilTimer = 0;

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.R))
			Reload();

		recoilTimer = Mathf.Clamp01(recoilTimer - Time.deltaTime);
		transform.localPosition = Vector3.back * recoilTimer;
	}

	public void Reload ()
	{
		StartCoroutine(ReloadSequence());
	}

	IEnumerator ReloadSequence ()
	{
		reloading = true;

		float reloadDuration = 1;
		float reloadTimer = 0;
		while (reloadTimer < reloadDuration)
		{
			reloadTimer += Time.deltaTime;
			transform.localEulerAngles = Vector3.right * m_ReloadCurve.Evaluate(Mathf.Clamp01(reloadTimer / reloadDuration)) * 45;

			yield return 0;
		}

		reloading = false;
	}

	public void Recoil (float amount)
	{
		recoilTimer = amount;
	}

	IEnumerator RecoilSequence ()
	{
		float reloadDuration = 1;
		float reloadTimer = 0;
		while (reloadTimer < reloadDuration)
		{
			reloadTimer += Time.deltaTime;
			transform.localEulerAngles = Vector3.right * m_ReloadCurve.Evaluate(Mathf.Clamp01(reloadTimer / reloadDuration)) * 45;

			yield return 0;
		}
	}
}
