using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor (typeof (FBPathsGroup))]
public class FBPathsGroupInspector : Editor {
	private FBPathsGroup group;

	private void OnEnable () {
		group = (FBPathsGroup) target;
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		if (GUI.changed) {
			EditorUtility.SetDirty (group);
		}

		if (group.paths == null) {
			group.paths = new List<FBPath> ();
		}

		if (group.paths == null) {
			return;
		}
		List<int> removeIndexList = new List<int> ();
		for (int i = 0; i < group.paths.Count; ++i)
			if (group.paths[i] == null)
				removeIndexList.Add (i);
		if (removeIndexList.Count > 0)
			for (int i = removeIndexList.Count - 1; i >= 0; --i)
				group.paths.RemoveAt (removeIndexList[i]);

		FBEditableExtensions<FBPath>.GUIField (group.paths, group.gameObject);
	}

	private void OnSceneGUI () {
		if (group.paths == null)
			return;
		foreach (FBPath path in group.paths) {
			path.DrawOnScene (true);
		}
	}
}