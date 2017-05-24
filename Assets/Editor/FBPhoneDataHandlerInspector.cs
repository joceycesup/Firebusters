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

		sensor.comNum = EditorGUILayout.IntField ("Com num", sensor.comNum);

		if (GUI.changed) {
			EditorUtility.SetDirty (sensor);
		}
	}
}