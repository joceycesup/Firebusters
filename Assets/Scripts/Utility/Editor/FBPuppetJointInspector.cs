﻿using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBPuppetJoint)), CanEditMultipleObjects]
public class FBPuppetJointInspector : Editor {
	private FBPuppetJoint joint;
	private float breakDistance;
	private float rebindDistance;

	private void OnEnable () {
		joint = (FBPuppetJoint) target;
		breakDistance = Mathf.Sqrt (joint.breakSqrDistance);
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		if (GUI.changed) {
			EditorUtility.SetDirty (joint);
		}

		EditorGUI.BeginChangeCheck ();
		joint.breakSqrDistance = Mathf.Max (EditorGUILayout.FloatField ("Break square distance", joint.breakSqrDistance), float.Epsilon);
		if (EditorGUI.EndChangeCheck ())
			breakDistance = Mathf.Sqrt (joint.breakSqrDistance);
		EditorGUI.BeginChangeCheck ();
		breakDistance = Mathf.Max (EditorGUILayout.FloatField ("Break distance", breakDistance), float.Epsilon);
		if (EditorGUI.EndChangeCheck ())
			joint.breakSqrDistance = breakDistance * breakDistance;

		EditorGUI.BeginChangeCheck ();
		joint.rebindSqrDistance = Mathf.Max (EditorGUILayout.FloatField ("Rebind square distance", joint.rebindSqrDistance), float.Epsilon);
		if (EditorGUI.EndChangeCheck ())
			rebindDistance = Mathf.Sqrt (joint.rebindSqrDistance);
		EditorGUI.BeginChangeCheck ();
		rebindDistance = Mathf.Max (EditorGUILayout.FloatField ("Rebind distance", rebindDistance), float.Epsilon);
		if (EditorGUI.EndChangeCheck ())
			joint.rebindSqrDistance = rebindDistance * rebindDistance;
		else if (joint.rebindSqrDistance != rebindDistance * rebindDistance)
			rebindDistance = Mathf.Sqrt (joint.rebindSqrDistance);
	}
}