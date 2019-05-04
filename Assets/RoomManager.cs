using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomManager : MonoBehaviour
{
	// Inspector
	[SerializeField]
	Room			startRoom;
	[SerializeField]
	GameObject[]	roomPool;

	Room current, previous, next;

	List<Room> stack;

    // Start is called before the first frame update
    void Start ()
    {
		stack = new List<Room>();
        current = startRoom;
		GenerateNext();
    }

	void GenerateNext ()
	{
		if (next)
			next.OnPlayerEntered -= Progress;
		var newRoom = Instantiate(roomPool[Random.Range(0, roomPool.Length)]).GetComponent<Room>();
		newRoom.transform.localScale = new Vector3(Random.Range(0, 2) == 0 ? 1 : -1, 1, 1);
		var rotDiff = current.exitPoint.rotation * Quaternion.Inverse(newRoom.entryPoint.rotation);
		newRoom.transform.rotation *= rotDiff;
		var posDiff = current.exitPoint.position - newRoom.entryPoint.position;
		newRoom.transform.position += posDiff;
		next = newRoom;
		next.OnPlayerEntered += Progress;
		GetComponent<NavMeshSurface>().BuildNavMesh();
	}

	void Progress ()
	{
		if (previous)
		{
			//stack.Add(previous);
			Destroy(previous.gameObject);
		}
		previous = current;
		current = next;
		GenerateNext();
	}

	void ClearStack ()
	{
		for (int i = 0; i < stack.Count; i++)
		{
			Destroy(stack[i].gameObject);
		}
		stack.Clear();
	}

    // Update is called once per frame
    void Update ()
    {
        
    }
}
