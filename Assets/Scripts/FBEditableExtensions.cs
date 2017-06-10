#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class FBEditableExtensions<T> where T : FBEditable, new() {

	public static List<T> GUIField (List<T> list, GameObject attachedGameObject) {
		if (list == null) {
			list = new List<T> ();
		}

		List<int> removeIndexList = new List<int> ();
		for (int i = 0; i < list.Count; ++i)
			if (list[i] == null)
				removeIndexList.Add (i);
		if (removeIndexList.Count > 0)
			for (int i = removeIndexList.Count - 1; i >= 0; --i)
				list.RemoveAt (removeIndexList[i]);

		int removeIndex = -1;
		int moveUpIndex = -1;
		int moveDownIndex = -1;

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("\u25C0", GUILayout.Width (20))) {//u25B2
			for (int i = 0; i < list.Count; ++i)
				list[i].showingInInspector = false;
			SceneView.RepaintAll ();
		}
		if (GUILayout.Button ("\u25B6", GUILayout.Width (20))) {//u25B2
			for (int i = 0; i < list.Count; ++i)
				list[i].showingInInspector = true;
			SceneView.RepaintAll ();
		}
		EditorGUILayout.EndHorizontal ();

		for (int i = 0; i < list.Count; ++i) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUI.BeginChangeCheck ();
			list[i].showingInInspector = EditorGUILayout.Foldout (list[i].showingInInspector, list[i].Name);
			if (EditorGUI.EndChangeCheck ())
				SceneView.RepaintAll ();
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
			if (list[i].showingInInspector) {
				list[i].GUIField ();
				EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider);
			}
		}

		if (removeIndex >= 0) {
			GameObject go = list[removeIndex].gameObject;
			list.RemoveAt (removeIndex);
			UnityEngine.Object.DestroyImmediate (go);
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

		if (GUILayout.Button ("Add " + typeof (T).ToString ())) {
			GameObject go = new GameObject ();
			list.Add (go.AddComponent<T> ());
			go.transform.SetParent (attachedGameObject.transform);
			//go.hideFlags = HideFlags.HideInHierarchy;
		}
		return list;
	}
}
#endif