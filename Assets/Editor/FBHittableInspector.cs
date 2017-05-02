using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBHittable))]
public class FBHittableInspector : Editor {
	private FBHittable hittable;

	private void OnEnable () {
		hittable = (FBHittable) target;
	}

	public override void OnInspectorGUI () {
		hittable.destroyable = EditorGUILayout.Toggle ("Destroyable", hittable.destroyable);

		hittable.axeSound = (FBAxeSound) EditorGUILayout.EnumPopup ("Axe Sound", hittable.axeSound);
		hittable.hitSound = (FBHitSound) EditorGUILayout.EnumPopup ("Hit Sound", hittable.hitSound);

		if (GUI.changed) {
			EditorUtility.SetDirty (hittable);
		}
	}
}
