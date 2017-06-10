using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public enum FBScriptedAction {
	Destroy,
	Disable,
	Enable,
	Open,
	Close,
	SetDestructible,
	PlayAnimation,
	LoadScene
}

[Serializable]
public class FBObjectAction : FBEditable {
	public FBScriptedAction Action = FBScriptedAction.Destroy;
	public GameObject Target;
	public string strArg = "";
	public float Delay = -1.0f;

	public FBObjectAction () {
	}

	public FBObjectAction (FBScriptedAction action = FBScriptedAction.Destroy, GameObject target = null, string args = "", float delay = -1.0f) {
		Action = action;
		Target = target;
		strArg = args;
		Delay = delay;
	}

#if UNITY_EDITOR
	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		Action = (FBScriptedAction) EditorGUILayout.EnumPopup ("Scripted action", Action);
		if (!Target && Action != FBScriptedAction.LoadScene)
			EditorGUILayout.HelpBox ("Please specify a non null object!", MessageType.Error);

		switch (Action) {
			case FBScriptedAction.Destroy:
			case FBScriptedAction.Disable:
			case FBScriptedAction.Enable:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				break;
			case FBScriptedAction.Open:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				if (Target && !(Target.GetComponent<FBDoor> () || Target.GetComponent<FBPath> ()))
					EditorGUILayout.HelpBox ("This object doesn't have either a FBDoor or a FBPath", MessageType.Warning);
				break;
			case FBScriptedAction.Close:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				if (Target && !Target.GetComponent<FBPath> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBPath", MessageType.Warning);
				break;
			case FBScriptedAction.SetDestructible:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				if (Target && !Target.GetComponent<FBHittable> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBHittable", MessageType.Warning);
				break;
			case FBScriptedAction.PlayAnimation:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				if (Target && !Target.GetComponent<Animator> ())
					EditorGUILayout.HelpBox ("This object doesn't have an Animator", MessageType.Warning);
				strArg = EditorGUILayout.TextField ("Animation", strArg);
				break;
			case FBScriptedAction.LoadScene:
				strArg = EditorGUILayout.TextField ("Scene", strArg);
				break;
		}
		Delay = EditorGUILayout.FloatField ("Delay", Delay);
		return this;
	}
#endif
}