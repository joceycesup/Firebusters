using UnityEngine;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FBWaypoint :
#if UNITY_EDITOR
FBEditable
#else
	MonoBehaviour
#endif
	{

	public delegate void WaypointEvent (FBPath path);
	public event WaypointEvent OnOpenPath;
	public event WaypointEvent OnClosePath;

	[SerializeField, HideInInspector]
	public List<FBPath> paths;
#if UNITY_EDITOR
	private void Start () {
		if (Name.Length <= 0)
			Name = name;
	}

	public override FBEditable GUIField (string label = "") {
		base.GUIField ();

		return this;
	}
#endif

	public void OpenPath (FBPath path) {
		OnOpenPath (path);
	}
	public void ClosePath (FBPath path) {
		OnClosePath (path);
	}
}
