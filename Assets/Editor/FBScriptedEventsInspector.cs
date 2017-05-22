using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBScriptedEvents))]
public class FBScriptedEventsInspector : Editor {
	private FBScriptedEvents script;

	private void OnEnable () {
		script = (FBScriptedEvents) target;
	}

	public override void OnInspectorGUI () {
		int removeIndex = -1;
		int moveUpIndex = -1;
		int moveDownIndex = -1;

		for (int i = 0; i < script.zones.Count; ++i) {
			EditorGUILayout.BeginHorizontal ();
			script.zones[i].showingInInspector = EditorGUILayout.Foldout (script.zones[i].showingInInspector, script.zones[i].Name);
			EditorGUI.BeginDisabledGroup (i == 0);
			if (GUILayout.Button ("\u25B2", GUILayout.Width (20))) {
				moveUpIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			EditorGUI.BeginDisabledGroup (i >= script.zones.Count - 1);
			if (GUILayout.Button ("\u25BC", GUILayout.Width (20))) {
				moveDownIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			if (GUILayout.Button ("X", GUILayout.Width (20))) {
				removeIndex = i;
				break;
			}
			EditorGUILayout.EndHorizontal ();
			if (script.zones[i].showingInInspector) {
				script.zones[i].GUIField ();
				EditorGUILayout.LabelField ("", GUI.skin.horizontalSlider);
			}
		}

		if (removeIndex >= 0) {
			script.zones.RemoveAt (removeIndex);
		}
		else if (moveUpIndex >= 0) {
			FBEvent tmpEvent = script.zones[moveUpIndex];
			script.zones[moveUpIndex] = script.zones[moveUpIndex - 1];
			script.zones[moveUpIndex - 1] = tmpEvent;
		}
		else if (moveDownIndex >= 0) {
			FBEvent tmpEvent = script.zones[moveDownIndex];
			script.zones[moveDownIndex] = script.zones[moveDownIndex + 1];
			script.zones[moveDownIndex + 1] = tmpEvent;
		}

		if (GUILayout.Button ("Add element")) {
			script.zones.Add (new FBEvent ());
		}

		if (GUI.changed) {
			//Debug.Log ("Dirty");
			EditorUtility.SetDirty (script);
		}
	}
}
