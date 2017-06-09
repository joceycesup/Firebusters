using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBScriptedEvents))]
public class FBScriptedEventsInspector : Editor {
	private FBScriptedEvents script;

	private void OnEnable () {
		script = (FBScriptedEvents) target;
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		if (GUI.changed) {
			EditorUtility.SetDirty (target);
		}

		FBEditableExtensions<FBEvent>.GUIField (script.zones, script.gameObject);

		if (GUI.changed) {
			//Debug.Log ("Dirty");
			EditorUtility.SetDirty (script);
		}
	}
}
