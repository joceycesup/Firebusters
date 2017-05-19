using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBScriptedZones))]
public class FBScriptedZonesInspector : Editor {
	private FBScriptedZones script;

	private void OnEnable () {
		script = (FBScriptedZones) target;
	}

	public override void OnInspectorGUI () {
		foreach (TriggerZone tz in script.zones) {
			tz.GUIField ();
		}
		if (GUILayout.Button ("Add element")) {
			script.zones.Add (new TriggerZone ());
		}

		if (GUI.changed) {
			EditorUtility.SetDirty (script);
		}
	}
}
