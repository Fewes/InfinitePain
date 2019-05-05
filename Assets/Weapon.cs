using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	public enum Type
	{
		Shotgun
	}

	// Inspector
	public Type			type			= Type.Shotgun;
	public float		recoilAmount	= 1f;
	public int			ammo			= 8;
	public Transform	pump;
	[Header("Audio")]
	public string		fireAudio;
	public string		shellAudio;
	public string		cycleAudio;
	public string		reloadAudio;

	WeaponRig			m_WeaponRig;
	ParticleSystem		m_Muzzle;
	ParticleSystem		m_Shell;

	Vector3				pumpStartPos;
	bool				isCycling;

    // Start is called before the first frame update
    void Start ()
    {
        m_WeaponRig = GetComponentInParent<WeaponRig>();
        m_Muzzle = transform.Find("Muzzle").GetComponent<ParticleSystem>();
        m_Shell = transform.Find("Shell").GetComponent<ParticleSystem>();
		pumpStartPos = pump.localPosition;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0)
		{
			Fire();
		}
    }

	public void Fire ()
	{
		if (isCycling)
			return;

		m_Muzzle.Play();
		m_WeaponRig.Recoil(recoilAmount);
		if (fireAudio != "")
			AudioManager.PlaySoundEffect(fireAudio, m_Muzzle.transform.position);

		if (ammo > 0)
			Cycle();
	}

	public void Cycle (float delay = 0.3f)
	{
		StartCoroutine(CycleSequence(delay));
	}

	IEnumerator CycleSequence (float delay = 0.3f)
	{
		isCycling = true;

		yield return new WaitForSeconds(delay);

		if (cycleAudio != "")
			AudioManager.PlaySoundEffect(cycleAudio, transform.position);

		float timer = 0;
		// Pump back
		while (timer < 0.25f)
		{
			timer = Mathf.Min(timer + Time.deltaTime, 0.25f);
			pump.localPosition = pumpStartPos - pump.parent.InverseTransformDirection(pump.forward) * timer;
			transform.localEulerAngles = Vector3.up * (-90 - timer * 40f); 
			yield return 0;
		}

		ammo--;

		m_Shell.Play();
		if (shellAudio != "")
			StartCoroutine(ShellAudio());

		// Pump forward
		while (timer > 0f)
		{
			timer = Mathf.Max(timer - Time.deltaTime, 0f);
			pump.localPosition = pumpStartPos - pump.parent.InverseTransformDirection(pump.forward) * timer;
			transform.localEulerAngles = Vector3.up * (-90 - timer * 40f); 
			yield return 0;
		}
		
		isCycling = false;
	}

	IEnumerator ShellAudio ()
	{
		var pos = Player.local.transform.position;
		yield return new WaitForSeconds(0.5f);
		AudioManager.PlaySoundEffect(shellAudio, pos);
	}
}
