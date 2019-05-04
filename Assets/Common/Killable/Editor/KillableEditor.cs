using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Killable)), CanEditMultipleObjects]
public class KillableEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		Killable killable = (Killable)target;
		if (Application.isPlaying)
		{
			if (GUILayout.Button("Kill"))
			{
				killable.Kill();
			}
			if (GUILayout.Button("¨Destroy"))
			{
				killable.Destroy();
			}
			if (GUILayout.Button("Resurrect"))
			{
				killable.Resurrect();
			}
		}
	}
}
