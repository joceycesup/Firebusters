using UnityEngine;
using UnityEditor;

public class RearrangeSortingOrderRecursively : EditorWindow {
	static int go_count = 0, replacing_count = 0;

	public static float yStep = 0.01f;

	private static int[] order;

	[MenuItem ("Window/RearrangeSortingOrderRecursively")]
	public static void ShowWindow () {
		EditorWindow.GetWindow (typeof (RearrangeSortingOrderRecursively));
	}

	public void OnGUI () {
		yStep = EditorGUILayout.FloatField ("Y step", yStep);
		if (GUILayout.Button ("Replace sortingOrder in selected GameObjects")) {
			FindInSelected ();
		}
	}
	private static void FindInSelected () {
		GameObject[] go = Selection.gameObjects;
		order = new int[go.Length];
		go_count = 0;
		replacing_count = 0;
		for (int i = 0;i < go.Length; i++) {
			order[i] = 0;
			FindInGO (go[i], i);
		}
		Debug.Log (string.Format ("Searched {0} GameObjects, replaced sortingOrder on {1} objects", go_count, replacing_count));
	}

	private static void FindInGO (GameObject g, int index) {
		go_count++;
		SpriteRenderer sr = g.GetComponent<SpriteRenderer> ();
		if (sr) {
			sr.sortingOrder = order[index];
			g.transform.position = new Vector3 (g.transform.position.x, order[index] * yStep, g.transform.position.z);
			order[index]++;
			replacing_count++;
			Debug.Log ("Resorted on object : " + g);
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform) {
			//Debug.Log("Searching " + childT.name  + " " );
			FindInGO (childT.gameObject, index);
		}
	}
}