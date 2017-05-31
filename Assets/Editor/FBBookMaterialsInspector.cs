using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBBookMaterials))]
public class FBBookMaterialsInspector : Editor {
	private FBBookMaterials materials;

	private void OnEnable () {
		materials = (FBBookMaterials) target;
	}

	public override void OnInspectorGUI () {
		int removeIndex = -1;
		int moveUpIndex = -1;
		int moveDownIndex = -1;

		for (int i = 0; i < materials.limits.Count; ++i) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Type", GUILayout.Width (40));
			materials.limits[i].k = EditorGUILayout.IntField (materials.limits[i].k);
			EditorGUILayout.LabelField ("Max", GUILayout.Width (40));
			materials.limits[i].v = EditorGUILayout.IntField (materials.limits[i].v);
			EditorGUI.BeginDisabledGroup (i == 0);
			if (GUILayout.Button ("\u25B2", GUILayout.Width (20))) {
				moveUpIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			EditorGUI.BeginDisabledGroup (i >= materials.limits.Count - 1);
			if (GUILayout.Button ("\u25BC", GUILayout.Width (20))) {
				moveDownIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			if (GUILayout.Button ("X", GUILayout.Width (20))) {
				removeIndex = i;
				break;
			}
			EditorGUILayout.EndHorizontal ();
		}

		if (removeIndex >= 0) {
			materials.limits.RemoveAt (removeIndex);
		}
		else if (moveUpIndex >= 0) {
			Pair tmpPair = materials.limits[moveUpIndex];
			materials.limits[moveUpIndex] = materials.limits[moveUpIndex - 1];
			materials.limits[moveUpIndex - 1] = tmpPair;
		}
		else if (moveDownIndex >= 0) {
			Pair tmpPair = materials.limits[moveDownIndex];
			materials.limits[moveDownIndex] = materials.limits[moveDownIndex + 1];
			materials.limits[moveDownIndex + 1] = tmpPair;
		}

		if (GUILayout.Button ("Add element")) {
			materials.limits.Add (new Pair ());
		}

		if (GUI.changed) {
			//Debug.Log ("Dirty");
			EditorUtility.SetDirty (materials);
		}
	}
}
