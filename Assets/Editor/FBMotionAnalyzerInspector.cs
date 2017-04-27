using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FBMotionAnalyzer))]
public class FBMotionAnalyzerInspector : Editor {
	private FBMotionAnalyzer motion;

	private void OnEnable () {
		motion = (FBMotionAnalyzer) target;
	}

	public override void OnInspectorGUI () {
		motion.abilities = (FBMotionAnalyzer.Action) EditorGUILayout.EnumMaskField ("Abilities", motion.abilities);
		motion.usePhoneDataHandler = EditorGUILayout.Toggle ("Use phone data handler", motion.usePhoneDataHandler);

		EditorGUILayout.Space ();

		motion.maxRoll = EditorGUILayout.FloatField ("Max Roll", motion.maxRoll);
		motion.rollFactor = EditorGUILayout.CurveField ("Roll factor", motion.rollFactor);
		motion.maxPitch = EditorGUILayout.FloatField ("Max Pitch", motion.maxPitch);
		motion.pitchFactor = EditorGUILayout.CurveField ("Pitch factor", motion.pitchFactor);

		if (GUI.changed) {
			EditorUtility.SetDirty (motion);
		}
	}
}