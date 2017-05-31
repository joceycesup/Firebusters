using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBHitInEditor))]
public class FBHitInEditorInspector : Editor {
	private FBHitInEditor hie;

	private void OnEnable () {
		hie = (FBHitInEditor) target;
	}

	public override void OnInspectorGUI () {
		if (GUILayout.Button ("Hit")) {
			hie.hittable.Hit ();
		}
		if (GUILayout.Button ("Hit with Axe")) {
			hie.hittable.HitByAxe ();
		}
	}
}
