using UnityEngine;
using UnityEditor;

public class ReplaceMAterialsRecursively : EditorWindow {
	static int go_count = 0, replacing_count = 0;

	public static Material lastMaterial;
	public static Material newMaterial;

	[MenuItem ("Window/ReplaceMAterialsRecursively")]
	public static void ShowWindow () {
		EditorWindow.GetWindow (typeof (ReplaceMAterialsRecursively));
	}

	public void OnGUI () {
		lastMaterial = (Material) EditorGUILayout.ObjectField ("Last material", lastMaterial, typeof (Material), false);
		newMaterial = (Material) EditorGUILayout.ObjectField ("New material", newMaterial, typeof (Material), false);
		if (GUILayout.Button ("Replace material in selected GameObjects")) {
			FindInSelected ();
		}
	}
	private static void FindInSelected () {
		GameObject[] go = Selection.gameObjects;
		go_count = 0;
		replacing_count = 0;
		foreach (GameObject g in go) {
			FindInGO (g);
		}
		Debug.Log (string.Format ("Searched {0} GameObjects, replaced material on {1} objects", go_count, replacing_count));
	}

	private static void FindInGO (GameObject g) {
		go_count++;
		MeshRenderer mr = g.GetComponent<MeshRenderer> ();
		if (mr) {
			if (mr.sharedMaterial == lastMaterial) {
				replacing_count++;
				mr.sharedMaterial = newMaterial;
				Debug.Log ("Replaced material on object : " + g);
			}
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform) {
			//Debug.Log("Searching " + childT.name  + " " );
			FindInGO (childT.gameObject);
		}
	}
}