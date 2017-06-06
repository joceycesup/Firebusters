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

	[HideInInspector, SerializeField]
	public List<FBPath> paths;// = new List<FBPath> ();
	[HideInInspector, SerializeField]
	public List<bool> openPaths;// = new List<bool> ();
	[HideInInspector, SerializeField]
	public int openPathsCount = 0;
#if UNITY_EDITOR
	private void Awake () {
		paths = new List<FBPath> ();
		openPaths = new List<bool> ();
		if (Name.Length <= 0)
			Name = name;
	}

	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		paths = new List<FBPath> ();
		openPaths = new List<bool> ();
		return this;
	}
#endif
	public void AddPath (FBPath path) {
		//Debug.Log (Name + " add " + path.Name);
		paths.Add (path);
		path.OnOpen += OpenPath;
		path.OnClose += ClosePath;
		openPaths.Add (path.open);
		if (path.open)
			openPathsCount++;
	}
	public void RemovePath (FBPath path) {
		int index = paths.FindIndex ((p) => { return p == path; });
		openPaths.RemoveAt (index);
		if (path.open)
			openPathsCount--;
		paths.RemoveAt (index);
		path.OnOpen -= OpenPath;
		path.OnClose -= ClosePath;
	}

	public void OpenPath (FBPath path) {
		if (OnOpenPath != null)
			OnOpenPath (path);
		int index = paths.FindIndex ((p) => { return p == path; });
		if (!openPaths[index])
			openPathsCount++;
	}
	public void ClosePath (FBPath path) {
		if (OnClosePath != null)
			OnClosePath (path);
		int index = paths.FindIndex ((p) => { return p == path; });
		if (openPaths[index])
			openPathsCount--;
	}

	public FBPath GetNextPath (FBPath from) {
		FBPath res = from;
		int max = openPathsCount;
		if (openPathsCount == 0)
			return null;
		if (from != null) {
			if (openPathsCount == 1)
				return from;
			max--;
		}
		int index = UnityEngine.Random.Range (0, max);
		int i = 0;
		for (int j = 0; i < paths.Count; ++i) {
			if (paths[i].open && paths[i] != from)
				j++;
			if (j == index + 1)
				break;
		}
		if (i >= paths.Count)
			return null;
		res = paths[i];

		return res;
	}

#if UNITY_EDITOR
	public override string ToString () {
		string res = "FBWaypoint " + Name + " (" + openPathsCount + " open) : ";
		for (int i = 0; i < paths.Count; ++i) {
			if (paths[i].start == this) {
				if (i > 0)
					res += ", ";
				if (!paths[i].open)
					res += "!";
				res += paths[i].end.Name;
			}
			else if (paths[i].end == this) {
				if (i > 0)
					res += ", ";
				if (!paths[i].open)
					res += "!";
				res += paths[i].start.Name;
			}
		}
		return res;
	}
#endif
}
