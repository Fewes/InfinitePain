using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	static SpawnManager instance;

	public static GameObject brute { get { return instance.m_Brute; } }
	public static GameObject health { get { return instance.m_Health; } }
	public static GameObject ammo { get { return instance.m_Ammo; } }

    [SerializeField]
	GameObject	m_Brute;
	[SerializeField]
	GameObject	m_Health;
	[SerializeField]
	GameObject	m_Ammo;

	private void Awake ()
	{
		if (instance && instance != this)
			Debug.LogError("Multiple SpawnManagers found");
		else
			instance = this;
	}
}
