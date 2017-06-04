using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBPutOutInEditor))]
public class FBPutOutInEditorInspector : Editor {
	private FBPutOutInEditor poie;

	private void OnEnable () {
		poie = (FBPutOutInEditor) target;
	}

	public override void OnInspectorGUI () {
		if (GUILayout.Button ("PutOut")) {
			poie.fire.PutOut ();
		}
	}
}
