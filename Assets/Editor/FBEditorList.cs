using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class FBEditorList<T> {
	private List<T> list;

	public FBEditorList () {
		list = new List<T> ();
	}

	public void GUIField () {
		int removeIndex = -1;
		int moveUpIndex = -1;
		int moveDownIndex = -1;

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("\u25C0", GUILayout.Width (20))) {//u25B2
			for (int i = 0; i < list.Count; ++i)
				((FBEditable) (object) list[i]).showingInInspector = false;
		}
		if (GUILayout.Button ("\u25B6", GUILayout.Width (20))) {//u25B2
			for (int i = 0; i < list.Count; ++i)
				((FBEditable) (object) list[i]).showingInInspector = true;
		}
		EditorGUILayout.EndHorizontal ();

		for (int i = 0; i < list.Count; ++i) {
			EditorGUILayout.BeginHorizontal ();
			FBEditable item = ((FBEditable) (object) list[i]);
			item.showingInInspector = EditorGUILayout.Foldout (item.showingInInspector, item.Name);
			EditorGUI.BeginDisabledGroup (i == 0);
			if (GUILayout.Button ("\u25B2", GUILayout.Width (20))) {//u25B2
				moveUpIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			EditorGUI.BeginDisabledGroup (i >= list.Count - 1);
			if (GUILayout.Button ("\u25BC", GUILayout.Width (20))) {//u25BC
				moveDownIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			if (GUILayout.Button ("X", GUILayout.Width (20))) {
				removeIndex = i;
				break;
			}
			EditorGUILayout.EndHorizontal ();
			if (item.showingInInspector) {
				item.GUIField ();
				EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider);
			}
		}

		if (removeIndex >= 0) {
			list.RemoveAt (removeIndex);
		}
		else if (moveUpIndex >= 0) {
			T tmpItem = list[moveUpIndex];
			list[moveUpIndex] = list[moveUpIndex - 1];
			list[moveUpIndex - 1] = tmpItem;
		}
		else if (moveDownIndex >= 0) {
			T tmpItem = list[moveDownIndex];
			list[moveDownIndex] = list[moveDownIndex + 1];
			list[moveDownIndex + 1] = tmpItem;
		}

		if (GUILayout.Button ("Add element")) {
			list.Add ((T) (object) new FBEditable ());
		}
	}
}
