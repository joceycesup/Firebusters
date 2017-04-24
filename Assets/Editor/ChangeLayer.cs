using UnityEngine;
using UnityEditor;
using System;

public class ChangeLayer : EditorWindow {
	static int go_count = 0, corrected_count = 0;

	static int layer;
	static string layerName = "";
	static string objectNameLayer = "";
	static string tag = "";
	static string objectNameTag = "";

	[MenuItem ("Window/ChangeLayer")]
	public static void ShowWindow () {
		EditorWindow.GetWindow (typeof (ChangeLayer));
	}

	public void OnGUI () {
		if (layerName != (layerName = EditorGUILayout.TextField ("Layer name", layerName)) && layerName.Length > 0) {
			layer = LayerMask.NameToLayer (layerName);
			//Debug.Log (layerName + " : " + layer);
		}
		if (layer < 0 || layerName.Length <= 0) {
			GUIStyle style = new GUIStyle ();
			style.normal.textColor = Color.red;
			EditorGUILayout.LabelField ("Layer name is invalid", style);
		}
		objectNameLayer = EditorGUILayout.TextField ("Object name", objectNameLayer);
		if (GUILayout.Button ("Reset layer")) {
			if (layer < 0)
				Debug.LogWarning ("Layer name is invalid, please correct");
			else if (objectNameLayer.Length <= 0)
				Debug.LogWarning ("The name field is empty, please fill it");
			else
				CorrectLayerInSelected ();
		}

		EditorGUILayout.Space ();

		bool tagExists = true;
		if (tag != (tag = EditorGUILayout.TextField ("Tag ", tag)) && tag.Length > 0) {
			layer = LayerMask.NameToLayer (layerName);
			//Debug.Log (layerName + " : " + layer);
		}
		try {
			GameObject.FindGameObjectsWithTag (tag);
		} catch (Exception) {
			tagExists = false;
		}
		if (!tagExists) {
			GUIStyle style = new GUIStyle ();
			style.normal.textColor = Color.red;
			EditorGUILayout.LabelField ("Tag is invalid", style);
		}
		objectNameTag = EditorGUILayout.TextField ("Object name", objectNameTag);
		if (GUILayout.Button ("Reset name")) {
			if (!tagExists)
				Debug.LogWarning ("Tag doesn't exist");
			else if (objectNameTag.Length <= 0)
				Debug.LogWarning ("The name field is empty, please fill it");
			else
				CorrectTagInSelected ();
		}
	}
	//------------------------------------------------------------

	private static void CorrectLayerInSelected () {
		GameObject[] go = Selection.gameObjects;
		go_count = 0;
		corrected_count = 0;
		foreach (GameObject g in go) {
			CorrectLayerInGO (g);
		}
		Debug.Log (string.Format ("Searched {0} GameObjects, corrected {1}", go_count, corrected_count));
	}

	private static void CorrectLayerInGO (GameObject g) {
		go_count++;
		if (g.name.CompareTo (objectNameLayer) == 0 && g.layer != layer) {
			corrected_count++;
			g.layer = layer;
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform) {
			//Debug.Log("Searching " + childT.name  + " " );
			CorrectLayerInGO (childT.gameObject);
		}
	}
	//------------------------------------------------------------

	private static void CorrectTagInSelected () {
		GameObject[] go = Selection.gameObjects;
		go_count = 0;
		corrected_count = 0;
		foreach (GameObject g in go) {
			CorrectTagInGO (g);
		}
		Debug.Log (string.Format ("Searched {0} GameObjects, corrected {1}", go_count, corrected_count));
	}

	private static void CorrectTagInGO (GameObject g) {
		go_count++;
		if (g.tag.CompareTo (tag) == 0 && g.name.CompareTo (objectNameTag) != 0) {
			corrected_count++;
			g.name = objectNameTag;
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform) {
			//Debug.Log("Searching " + childT.name  + " " );
			CorrectTagInGO (childT.gameObject);
		}
	}
}