﻿using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBInteractInEditor)), CanEditMultipleObjects]
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
		if (iie.button && GUILayout.Button ("Click")) {
			iie.button.Click ();
		}
		if (iie.door && GUILayout.Button ("Open")) {
			iie.door.Open ();
		}
	}
}
