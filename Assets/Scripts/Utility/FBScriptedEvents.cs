using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
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
	Draw
}

[Serializable]
public enum FBScriptedAction {
	Destroy,
	Disable,
	Enable,
	Open,
	SetDestructible,
	PlayAnimation
}

[Serializable]
public class ObjectAction {
	public FBScriptedAction Action;
	public GameObject Target;
	public string strArg;
	public float Delay;

	public ObjectAction (FBScriptedAction action = FBScriptedAction.Destroy, GameObject target = null, string args = "", float delay = -1.0f) {
		Action = action;
		Target = target;
		strArg = args;
		Delay = delay;
	}

	public ObjectAction GUIField () {
		Action = (FBScriptedAction) EditorGUILayout.EnumPopup ("Scripted action", Action);

		switch (Action) {
			case FBScriptedAction.Destroy:
			case FBScriptedAction.Disable:
			case FBScriptedAction.Enable:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				break;
			case FBScriptedAction.Open:
				Target = (GameObject) EditorGUILayout.ObjectField ("Action target", Target, typeof (GameObject), true);
				if (Target && !Target.GetComponent<FBDoor> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBDoor", MessageType.Warning);
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
		}
		Delay = EditorGUILayout.FloatField ("Delay", Delay);
		return this;
	}
}

[Serializable]
public class FBEvent {
#if UNITY_EDITOR
	public string Name = "";
#endif
	[SerializeField]
	private FBTriggerEvent _Event;
	public FBTriggerEvent Event { get { return _Event; } private set { _Event = value; } }
	public GameObject _Source;
	public GameObject Source { get { return _Source; } private set { _Source = value; } }
	public GameObject _Trigger;
	public GameObject Trigger { get { return _Trigger; } private set { _Trigger = value; } }

	[SerializeField]
	public List<ObjectAction> Actions = new List<ObjectAction> ();

	public List<IEnumerator> Couroutines = new List<IEnumerator> ();

#if UNITY_EDITOR
	public bool showingInInspector = false;

	private static string[] names = { "Hello gorgeous ;)", "Pls name me :'(", "Monde de merde...", "My name is nobody", "Chuck Norris' favorite" };

	public FBEvent GUIField () {
		EditorGUI.indentLevel++;
		if (Name.Length <= 0)
			Name = names[UnityEngine.Random.Range (0, names.Length)];
		Name = EditorGUILayout.TextField ("Name", Name);

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
				EditorGUI.BeginChangeCheck ();
				tmpObject = (GameObject) EditorGUILayout.ObjectField ("Event source", Source, typeof (GameObject), true);
				if (EditorGUI.EndChangeCheck ())
					Source = tmpObject;
				if (tmpObject && !tmpObject.GetComponent<FBPuppetController> ())
					EditorGUILayout.HelpBox ("This object doesn't have a FBPuppetController", MessageType.Warning);
				break;
		}
		//###############################################################
		EditorGUILayout.Space ();
		int removeIndex = -1;
		int moveUpIndex = -1;
		int moveDownIndex = -1;

		for (int i = 0; i < Actions.Count; ++i) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUI.EndDisabledGroup ();
			if (GUILayout.Button ("X", GUILayout.Width (20))) {
				removeIndex = i;
				break;
			}
			EditorGUI.BeginDisabledGroup (i == 0);
			if (GUILayout.Button ("\u25B2", GUILayout.Width (20))) {
				moveUpIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			EditorGUI.BeginDisabledGroup (i >= Actions.Count - 1);
			if (GUILayout.Button ("\u25BC", GUILayout.Width (20))) {
				moveDownIndex = i;
				break;
			}
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndHorizontal ();
			Actions[i].GUIField ();
		}

		if (removeIndex >= 0) {
			Actions.RemoveAt (removeIndex);
		}
		else if (moveUpIndex >= 0) {
			ObjectAction tmpAction = Actions[moveUpIndex];
			Actions[moveUpIndex] = Actions[moveUpIndex - 1];
			Actions[moveUpIndex - 1] = tmpAction;
		}
		else if (moveDownIndex >= 0) {
			ObjectAction tmpAction = Actions[moveDownIndex];
			Actions[moveDownIndex] = Actions[moveDownIndex + 1];
			Actions[moveDownIndex + 1] = tmpAction;
		}
		if (GUILayout.Button ("Add action")) {
			Actions.Add (new ObjectAction ());
		}

		EditorGUI.indentLevel--;
		return this;
	}
#endif

	public bool Setup () {
		bool res = true;
		if (!Source || Actions.Count <= 0 || ((Event == FBTriggerEvent.Enter || Event == FBTriggerEvent.Exit) && !Trigger)) {
			res = false;
#if UNITY_EDITOR
			Debug.LogError ((Source ? (Actions.Count > 0 ? "Trigger is" : "Actions are") : "Source is") + (" not setup on event " + Name));
			Debug.Break ();
#endif
		}
		else {
			switch (Event) {
				case FBTriggerEvent.Enter:
				case FBTriggerEvent.Exit:
					if (!Source.GetComponent<FBTriggerZone> ())
						Source.AddComponent<FBTriggerZone> ();
					break;
				default:
					break;
			}
		}
		return res;
	}
}

public class FBScriptedEvents : MonoBehaviour {
	[SerializeField]
	public List<FBEvent> zones = new List<FBEvent> ();

	void Awake () {
		foreach (FBEvent e in zones) {
			if (e.Setup ()) {
				switch (e.Event) {
					case FBTriggerEvent.Enter:
						e.Source.GetComponent<FBTriggerZone> ().OnEnter += CheckEnter;
						break;
					case FBTriggerEvent.Exit:
						e.Source.GetComponent<FBTriggerZone> ().OnExit += CheckExit;
						break;
					case FBTriggerEvent.HitByAxe:
						e.Source.GetComponent<FBHittable> ().OnHitByAxe += CheckHitByAxe;
						break;
					case FBTriggerEvent.Destroyed:
						e.Source.GetComponent<FBHittable> ().OnDestroyed += CheckDestroyed;
						break;
					case FBTriggerEvent.Open:
						e.Source.GetComponent<FBDoor> ().OnOpen += CheckOpen;
						break;
					case FBTriggerEvent.PutOut:
						e.Source.GetComponent<FBFire> ().OnPutOut += CheckPutOut;
						break;
					case FBTriggerEvent.Draw:
						e.Source.GetComponent<FBPuppetController> ().OnDraw += CheckDraw;
						break;
				}
			}
		}
	}

	void CheckHitByAxe (GameObject go) { CheckEvent (FBTriggerEvent.HitByAxe, go); }
	void CheckDestroyed (GameObject go) { CheckEvent (FBTriggerEvent.Destroyed, go); }
	void CheckOpen (GameObject go) { CheckEvent (FBTriggerEvent.Open, go); }
	void CheckPutOut (GameObject go) { CheckEvent (FBTriggerEvent.PutOut, go); }
	void CheckDraw (GameObject go) { CheckEvent (FBTriggerEvent.Draw, go); }
	void CheckEnter (GameObject source, GameObject trigger) { CheckEvent (FBTriggerEvent.Enter, source, trigger); }
	void CheckExit (GameObject source, GameObject trigger) { CheckEvent (FBTriggerEvent.Exit, source, trigger); }

	void CheckEvent (FBTriggerEvent ev, GameObject source, GameObject trigger = null) {
		foreach (FBEvent e in zones) {
			if (e.Event != ev)
				continue;
			if (e.Source != source)
				continue;
			if (e.Event == FBTriggerEvent.Enter || e.Event == FBTriggerEvent.Exit)
				if (e.Trigger != trigger)
					continue;
			foreach (ObjectAction action in e.Actions) {
				IEnumerator co = TriggerAction (action);
				if (action.Delay > 0.0f)
					e.Couroutines.Add (co);
				StartCoroutine (co);
			}
			break;
		}
	}

	private IEnumerator TriggerAction (ObjectAction a) {
		yield return new WaitForSeconds (a.Delay);
		switch (a.Action) {
			case FBScriptedAction.Destroy:
				Destroy (a.Target);
				break;
			case FBScriptedAction.Disable:
				a.Target.SetActive (false);
				break;
			case FBScriptedAction.Enable:
				a.Target.SetActive (true);
				break;
			case FBScriptedAction.Open:
				a.Target.GetComponent<FBDoor> ().Open ();
				break;
			case FBScriptedAction.SetDestructible:
				a.Target.GetComponent<FBHittable> ().destructible = true;
				break;
			case FBScriptedAction.PlayAnimation:
				a.Target.GetComponent<Animator> ().Play (a.strArg);
				break;
		}
	}
}
