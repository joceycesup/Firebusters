using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public enum FBTriggerEvent {
	HitByAxe,
	Destroyed,
	Exit,
	Enter,
	Open,
	PutOut,
	Draw,
	Grab,
	Trigger
}

[Serializable]
public class FBObjectEvent : FBEditable {
	public FBTriggerEvent Event = FBTriggerEvent.Destroyed;
	public GameObject Source;
	public GameObject Trigger;

	public FBObjectEvent () {
	}

	public FBObjectEvent (FBTriggerEvent e = FBTriggerEvent.Destroyed, GameObject source = null, GameObject trigger = null) {
		Event = e;
		Source = source;
		Trigger = trigger;
	}

#if UNITY_EDITOR
	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		Event = (FBTriggerEvent) EditorGUILayout.EnumPopup ("Trigger event", Event);

		GameObject tmpObject = null;

		switch (Event) {
			case FBTriggerEvent.Enter:
			case FBTriggerEvent.Exit:
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<Collider> ())
					EditorGUILayout.HelpBox ("This object doesn't have a Collider", MessageType.Warning);

				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event trigger", Trigger, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Trigger = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<Rigidbody> ())
					EditorGUILayout.HelpBox ("This object doesn't have a Rigidbody", MessageType.Warning);
				break;
			case FBTriggerEvent.Destroyed:
			case FBTriggerEvent.HitByAxe:
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<FBHittable> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBHittable", MessageType.Warning);
				break;
			case FBTriggerEvent.Open:
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<FBDoor> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBDoor", MessageType.Warning);
				break;
			case FBTriggerEvent.PutOut:
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<FBFire> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBFire", MessageType.Warning);
				break;
			case FBTriggerEvent.Draw:
			case FBTriggerEvent.Grab:
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<FBPuppetController> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBPuppetController", MessageType.Warning);
				break;
			case FBTriggerEvent.Trigger:
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<FBEvent> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBEvent", MessageType.Warning);
				break;
		}
		return this;
	}
#endif
}