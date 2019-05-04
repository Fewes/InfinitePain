using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	public delegate void RoomAction();
    public event RoomAction OnPlayerEntered;
    public event RoomAction OnPlayerExited;

	Transform _entryPoint;
	public Transform entryPoint
	{
		get
		{
			if (!_entryPoint)
				_entryPoint = transform.Find("Entry");
			return _entryPoint;
		}
	}
	Transform _exitPoint;
	public Transform exitPoint
	{
		get
		{
			if (!_exitPoint)
				_exitPoint = transform.Find("Exit");
			return _exitPoint;
		}
	}

	PlayerTrigger[] playerTriggers;
	bool playerInside;

	private void OnEnable ()
	{
		playerInside = false;
		playerTriggers = GetComponentsInChildren<PlayerTrigger>();
	}

	private void Update ()
	{
		foreach (var trigger in playerTriggers)
		{
			bool any = false;
			if (trigger.playerInside)
			{
				any = true;

				if (!playerInside)
				{
					playerInside = true;
					OnPlayerEntered?.Invoke();
				}
				
				break;
			}
			if (!any && playerInside)
			{
				playerInside = false;
				OnPlayerExited?.Invoke();
			}
		}
	}
}
