using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBPhoneDataHandler))]
public class FBPhoneDataHandlerInspector : Editor {
	private FBPhoneDataHandler sensor;

	private void OnEnable () {
		sensor = (FBPhoneDataHandler) target;
	}

	public override void OnInspectorGUI () {
		EditorGUILayout.HelpBox (sensor.connected ? "Connected" : "Port closed", MessageType.Info);

		sensor.id = EditorGUILayout.IntField ("ID", sensor.id);

		if (GUI.changed) {
			EditorUtility.SetDirty (sensor);
		}
	}
}