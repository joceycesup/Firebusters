using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBInteractInEditor))]
public class FBInteractInEditorInspector : Editor {
	private FBInteractInEditor iie;

	private void OnEnable () {
		iie = (FBInteractInEditor) target;
	}

	public override void OnInspectorGUI () {
		if (iie.fire && GUILayout.Button ("PutOut")) {
			iie.fire.PutOut ();
		}
		if (iie.hittable) {
			if (GUILayout.Button ("Hit")) {
				iie.hittable.Hit ();
			}
			if (GUILayout.Button ("Hit with Axe")) {
				iie.hittable.HitByAxe ();
			}
		}
	}
}
