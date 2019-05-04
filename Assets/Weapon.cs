using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	ParticleSystem m_Muzzle;

    // Start is called before the first frame update
    void Start ()
    {
        m_Muzzle = transform.Find("Muzzle").GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
		{
			Fire();
		}
    }

	public void Fire ()
	{
		m_Muzzle.Play();
	}
}
