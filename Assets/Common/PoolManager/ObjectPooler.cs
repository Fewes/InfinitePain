using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PooledObject
{
	public GameObject go;
	private float spawnTime;

	public void setSpawnTime(float time)
	{
		spawnTime = time;
	}
	public float getSpawnTime()
	{
		return spawnTime;
	}
}

[System.Serializable]
public class ObjectPooler// : MonoBehaviour
{
	[SerializeField] public string name;
	[SerializeField] public GameObject pooledObject;
	[SerializeField] public int poolSize;
	[SerializeField] public bool allowExpand = false;
	[SerializeField] public bool allowSteal = false;

	[HideInInspector] public List<PooledObject> pooledObjects;

	public int GetActiveObjectCount()
	{
		int count = 0;
		foreach (var pooledObject in pooledObjects)
			if (pooledObject.go.activeInHierarchy)
				count++;

		return count;
	}
}