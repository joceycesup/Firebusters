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
	private void Awake () {
#if !UNITY_EDITOR
		Destroy (GetComponent<MeshRenderer> ());
		}
#else
		paths = new List<FBPath> ();
		if (Name.Length <= 0)
			Name = name;
	}

	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		return this;
	}
#endif
	public void AddPath (FBPath path) {
		//Debug.Log (Name + " add " + path.Name);
		paths.Add (path);
		path.OnOpen += OpenPath;
		path.OnClose += ClosePath;
	}
	public void RemovePath (FBPath path) {
		paths.Remove (path);
		path.OnOpen -= OpenPath;
		path.OnClose -= ClosePath;
	}

	public void OpenPath (FBPath path) {
		if (OnOpenPath != null)
			OnOpenPath (path);
	}
	public void ClosePath (FBPath path) {
		if (OnClosePath != null)
			OnClosePath (path);
	}

	public FBPath GetNextPath (FBPath from) {
		int[] openPaths = GetOpenPaths (from);
		if (openPaths.Length == 0) {
			if (from != null && (from.CanTakePath (this)))
				return from;
			else
				return null;
		}
		return paths[openPaths[UnityEngine.Random.Range (0, openPaths.Length)]];
	}

	public int[] GetOpenPaths (FBPath ignored = null) {
		int[] res = new int[paths.Count];
		int count = 0;
		for (int i = 0; i < paths.Count; ++i) {
			if (paths[i].open && paths[i] != ignored) {
				if (paths[i].CanTakePath(this)) {
					res[count] = i;
					count++;
				}
			}
		}
		Array.Resize (ref res, count);
		return res;
	}

#if UNITY_EDITOR
	public override string ToString () {
		string res = "FBWaypoint " + Name + " (" + GetOpenPaths ().Length + " open) : ";
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
