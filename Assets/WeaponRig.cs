using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRig : MonoBehaviour
{
	// Inspector
	[SerializeField]
	AnimationCurve		m_ReloadCurve;

	public bool reloading { get; private set; }  = false;

	public Weapon current;

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
		bool canReload = true;

		switch (current.type)
		{
			case Weapon.Type.Shotgun:
				canReload = Player.local.shotgunAmmo > 0 && current.ammo < 8;
			break;
		}

		if (!reloading && canReload)
			StartCoroutine(ReloadSequence());
	}

	IEnumerator ReloadSequence ()
	{
		reloading = true;

		if (current.reloadAudio != "")
			AudioManager.PlaySoundEffect(current.reloadAudio, transform.position);

		float reloadDuration = 1f;
		float reloadTimer = 0;
		while (reloadTimer < reloadDuration)
		{
			reloadTimer += Time.deltaTime;
			transform.localEulerAngles = Vector3.right * m_ReloadCurve.Evaluate(Mathf.Clamp01(reloadTimer / reloadDuration)) * 45;

			yield return 0;
		}

		switch (current.type)
		{
			case Weapon.Type.Shotgun:
				int want = Mathf.Min(8 - current.ammo, Player.local.shotgunAmmo);
				Player.local.shotgunAmmo -= want;
				current.ammo += want;
			break;
		}

		reloading = false;

		current.Cycle(0);
	}

	public void Recoil (float amount)
	{
		recoilTimer = amount;
		Player.local.KickCamera(0.01f);
	}

	IEnumerator RecoilSequence ()
	{
		float recoilDuration = 1;
		float recoilTimer = 0;
		while (recoilTimer < recoilDuration)
		{
			recoilTimer += Time.deltaTime;
			transform.localEulerAngles = Vector3.right * m_ReloadCurve.Evaluate(Mathf.Clamp01(recoilTimer / recoilDuration)) * 45;

			yield return 0;
		}
	}
}
