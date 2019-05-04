using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
	[Range(0, 1f)]
	public float odds = 0.5f;

	public bool brute;

    void OnEnable ()
    {
        if (Random.Range(0f, 0.5f) <= odds)
			Spawn();
    }

    void Spawn ()
    {
		StartCoroutine(SpawnDelayed());
    }

	IEnumerator SpawnDelayed ()
	{
		yield return 0;
		var pool = new List<GameObject>();
        if (brute)
			pool.Add(SpawnManager.brute);

		Instantiate(pool[Random.Range(0, pool.Count)], transform.position, transform.rotation);
	}
}
