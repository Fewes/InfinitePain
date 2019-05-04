using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	static SpawnManager instance;

	public static GameObject brute { get { return instance.m_Brute; } }

    [SerializeField]
	GameObject	m_Brute;

	private void Awake ()
	{
		if (instance && instance != this)
			Debug.LogError("Multiple SpawnManagers found");
		else
			instance = this;
	}
}
