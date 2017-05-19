using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct TriggerZone {

	public TriggerZone GUIField () {
		EditorGUI.indentLevel++;
		EditorGUILayout.LabelField ("coucou");
		EditorGUI.indentLevel--;
		return this;
	}
}

public class FBScriptedZones : MonoBehaviour {
	public List<TriggerZone> zones = new List<TriggerZone> ();

	void Awake () {
	}
}
