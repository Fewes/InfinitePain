using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Debug = UnityEngine.Debug;

[System.Serializable]
public struct PoolCategory
{
	[SerializeField] public string name;
	[SerializeField] public ObjectPooler[] m_Pools;
}

public struct PoolID
{
	public short catID;
	public short typeID;

	public PoolID(short cat, short type)
	{
		catID = cat;
		typeID = type;
	}
}

[System.Serializable]
public class PoolManager : MonoBehaviour
{
	public delegate void PoolManagerEvent();
	public static event PoolManagerEvent AfterInitialization;

	public static PoolManager	manager;
	public static bool			isInstantiating { get; private set; }
	static List<GameObject>		active;

	[SerializeField]
	bool						startupCaching = true;
	[SerializeField]
	bool						logging = false;

#if !UNITY_EDITOR
	List<string>				log;
#endif

	[SerializeField] public PoolCategory[] m_Categories;

	void Log (string msg)
	{
#if UNITY_EDITOR
		using (StreamWriter sw = File.AppendText("PoolManagerLog.txt")) 
        {
            sw.WriteLine(msg);
        }
#else
		log.Add(msg);
#endif
	}

	void Awake()
	{
		// Singleton pattern
		if (manager && manager != this)
		{
			Debug.LogError("Multiple PoolManager instances found.");
			manager.enabled = false;
		}
		manager = this;
	}

	private void Start()
	{
		Initialize();
	}

	void Initialize ()
	{
		#if UNITY_EDITOR
		// Recreate the log file
		var f = File.Create("PoolManagerLog.txt");
		f.Close();
#else
		log = new List<string>();
#endif

		if (logging)
		{
			int objectCount = 0;
			foreach (var cat in m_Categories)
				objectCount += cat.m_Pools.Length;
			Log(m_Categories.Length + " categories with " + objectCount + " objects in total.");
			Log("Startup caching is " + (startupCaching ? "enabled." : "disabled."));
		}

		active = new List<GameObject>();

		isInstantiating = true;
		Stopwatch sw = new Stopwatch();
		sw.Start();
		// Initialize pools
		foreach (var cat in m_Categories)
		{
			foreach(var pool in cat.m_Pools)
			{
				if (!pool.pooledObject)
					continue;

				// Instantiate pooled objects
				pool.pooledObjects = new List<PooledObject>();

				// Only initialize "Allow steal" pools if startup caching is disabled
				if (!pool.allowSteal && !startupCaching)
					continue;

				for (int i = 0; i < pool.poolSize; ++i)
				{
					PooledObject po = new PooledObject();
					po.go = Instantiate(pool.pooledObject);
					po.go.transform.SetParent(transform);
					po.go.SetActive(false);
					pool.pooledObjects.Add(po);
				}
			}
		}
		isInstantiating = false;
		sw.Stop();
		Debug.Log("PoolManager instantiated in " + (float)sw.ElapsedMilliseconds * 0.001f + " s");
		if (logging)
		{
			Log("");
			Log("Objects instantiated in " + (float)sw.ElapsedMilliseconds * 0.001f + " s");
			Log("");
			Log("------------------------------------------------------------");
			Log("");
		}

		if (AfterInitialization != null)
			AfterInitialization();
	}

	private void OnDisable()
	{
		if (logging)
		{
			Log("");
			Log("------------------------------------------------------------");
			Log("");

			Log("Shutting down.");

			Log("");
			Log("Expanded pools:");

			int count = 0;
			foreach (var cat in m_Categories)
			{
				foreach (var pool in cat.m_Pools)
				{
					int initialSize = 0;
					if (startupCaching || pool.allowSteal)
						initialSize = pool.poolSize;
					if (pool.pooledObjects.Count != initialSize)
					{
						count++;
						Log("\t" + pool.name + ": " + pool.pooledObjects.Count +
							(pool.pooledObjects.Count != initialSize ? " <= up from " + initialSize : "")
						);
					}
				}
			}
			if (count == 0)
				Log("\t" + "None.");

			Log("");
			Log("Total number of instantiated objects:");
			foreach (var cat in m_Categories)
			{
				Log("\t" + cat.name + ":");
				foreach (var pool in cat.m_Pools)
				{
					int initialSize = 0;
					if (startupCaching || pool.allowSteal)
						initialSize = pool.poolSize;
					Log("\t" + "\t" + pool.name + ": " + pool.pooledObjects.Count +
						(pool.pooledObjects.Count != initialSize ? " <= up from " + initialSize : "")
					);
				}
			}
		}

#if !UNITY_EDITOR
		// Output to file
		File.WriteAllLines("PoolManagerLog.txt", log);
#endif
	}

	public static PoolID GetPoolID (string category, string type)
	{
		short i = 0;
		short j = 0;
		// Find category by name
		foreach (var cat in manager.m_Categories)
		{
			if (cat.name == "" || cat.name == category)
			{
				// Find pool by name
				foreach (var pool in cat.m_Pools)
				{
					if (pool.name == type)
					{
						return new PoolID(i, j);
					}
					j++;
				}
			}
			i++;
		}
		return new PoolID(-1, -1);
	}

	public static string GetCategoryName (PoolID id)
	{
		return manager.m_Categories[id.catID].name;
	}

	public static string GetTypeName (PoolID id)
	{
		return manager.m_Categories[id.catID].m_Pools[id.typeID].name;
	}

	// Get a pooled object by ID (faster)
	public static GameObject GetPooledObject(PoolID id, bool autoActivate = true)
	{
		return GetPooledObject(id, Vector3.zero, autoActivate);
	}
	public static GameObject GetPooledObject(PoolID id, Vector3 position, bool autoActivate = true)
	{
		return manager.GetPooledObjectM(id, position, autoActivate);
	}

	// Get a pooled object by strings (slower)
	public static GameObject GetPooledObject(string category, string type, bool autoActivate = true)
	{
		return GetPooledObject(category, type, Vector3.zero, autoActivate);
	}
	public static GameObject GetPooledObject(string category, string type, Vector3 position, bool autoActivate = true)
	{
		PoolID id = GetPoolID(category, type);
		return manager.GetPooledObjectM(id, position, autoActivate);
	}

	// Member function
	private GameObject GetPooledObjectM(PoolID id, Vector3 position, bool autoActivate = true)
	{
		if (id.catID == -1 || id.typeID == -1)
			return null;

		// Note that this will most likely crash if you tamper with the ID
		var pool = m_Categories[id.catID].m_Pools[id.typeID];

		// Search for available pooled objects
		foreach (var po in pool.pooledObjects)
		{
			if (!po.go.activeInHierarchy)
			{
				// Available object found
				po.go.transform.SetParent(null);
				po.go.transform.position = position;
				if (po.go && autoActivate)
					po.go.SetActive(true);
				po.setSpawnTime(Time.time);
				return po.go;
			}
		}

		// No available pooled object found, check if we are allowed to expand the pool
		if (pool.allowExpand)
		{
			Stopwatch sw = null;
			if (logging)
			{
				sw = new Stopwatch();
				sw.Start();
			}
			PooledObject po = new PooledObject();
			po.go = Instantiate(pool.pooledObject);
			pool.pooledObjects.Add(po);
			po.go.transform.SetParent(null);
			po.go.transform.position = position;
			if (po.go && autoActivate)
				po.go.SetActive(true);
			po.setSpawnTime(Time.time);
			if (logging)
			{
				sw.Stop();
				Log("Expanding pool of [" + m_Categories[id.catID].name + ", " + pool.name + "] due to insufficient inactive objects (" + sw.ElapsedMilliseconds + " ms).");
			}
			return po.go;
		}

		// No available pooled object found and we were not allowed to expand the pool, check if we are allowed to steal active objects
		if (pool.allowSteal)
		{
			// Find the oldest active object
			PooledObject po = pool.pooledObjects[0];
			foreach (var p in pool.pooledObjects)
			{
				if (p.getSpawnTime() < po.getSpawnTime())
					po = p;
			}

			po.go.SetActive(false);
			po.go.transform.SetParent(null);
			po.go.transform.position = position;
			if (po.go && autoActivate)
				po.go.SetActive(true);
			po.setSpawnTime(Time.time);
			return po.go;
		}

		// No available pooled object found and we were not allowed to expand the pool OR steal active objects
		return null;
	}

	// Get how many objects are currently active in a pool
	public int GetActiveObjectCount(PoolID id)
	{
		return m_Categories[id.catID].m_Pools[id.typeID].GetActiveObjectCount();
	}

	private void Update()
	{
		// Remove inactive objects from list
		for (int i = 0; i < active.Count;)
		{
			if (active[i].activeInHierarchy)
				i++;
			else
			{
				// Also reset parent
				active[i].transform.SetParent(transform);
				active.RemoveAt(i);
			}
		}
	}
}
