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
	public Type		type			= Type.Shotgun;
	public float	recoilAmount	= 1f;
	public int		ammo			= 8;

	WeaponRig		m_WeaponRig;
	ParticleSystem	m_Muzzle;
	ParticleSystem	m_Shell;

    // Start is called before the first frame update
    void Start ()
    {
        m_WeaponRig = GetComponentInParent<WeaponRig>();
        m_Muzzle = transform.Find("Muzzle").GetComponent<ParticleSystem>();
        m_Shell = transform.Find("Shell").GetComponent<ParticleSystem>();
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
		m_Muzzle.Play();
		m_Shell.Play();
		m_WeaponRig.Recoil(recoilAmount);
		ammo--;
	}
}
