using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomElement : MonoBehaviour
{
	[SerializeField]
	GameObject[] m_Scenarios;

	private void OnEnable ()
	{
		int element = Random.Range(0, m_Scenarios.Length);
		for (int i = 0; i < m_Scenarios.Length; i++)
		{
			if (m_Scenarios[i])
				m_Scenarios[i].SetActive(i == element);
		}
	}
}
