using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBPuppetJoint)), CanEditMultipleObjects]
public class FBPuppetJointInspector : Editor {
	private FBPuppetJoint joint;
	private float breakDistance;

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
	}
}