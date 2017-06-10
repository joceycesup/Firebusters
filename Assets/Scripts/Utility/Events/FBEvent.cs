using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FBEvent : FBEditable {
	[SerializeField, HideInInspector]
	public bool repeat = true;

	[SerializeField, HideInInspector]
	public List<FBObjectEvent> Events = new List<FBObjectEvent> ();

	[SerializeField, HideInInspector]
	public List<FBObjectAction> Actions = new List<FBObjectAction> ();

	[HideInInspector]
	public List<IEnumerator> Couroutines = new List<IEnumerator> ();

#if UNITY_EDITOR
	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		repeat = EditorGUILayout.Toggle ("Repeat", repeat);
		EditorGUI.indentLevel++;
		FBEditableExtensions<FBObjectEvent>.GUIField (Events, gameObject);
		//###############################################################
		EditorGUILayout.Space ();

		FBEditableExtensions<FBObjectAction>.GUIField (Actions, gameObject);

		EditorGUI.indentLevel--;
		return this;
	}
#endif

	public bool Setup () {
		bool res = true;//*
		if (Events.Count > 1)
			repeat = false;
		foreach (FBObjectEvent oe in Events) {
			if (!oe.Source || Actions.Count <= 0 || ((oe.Event == FBTriggerEvent.Enter || oe.Event == FBTriggerEvent.Exit) && !oe.Trigger)) {
				res = false;
#if UNITY_EDITOR
				Debug.LogError ((oe.Source ? (Actions.Count > 0 ? "Trigger is" : "Actions are") : "Source is") + (" not setup on event " + oe.Name));
				Debug.Break ();
#endif
			}
			else {
				switch (oe.Event) {
					case FBTriggerEvent.Enter:
					case FBTriggerEvent.Exit:
						if (!oe.Source.GetComponent<FBTriggerZone> ())
							oe.Source.AddComponent<FBTriggerZone> ();
						break;
					default:
						break;
				}
			}//*/
		}
		return res;
	}

	private void Awake () {
		if (Setup ()) {
			foreach (FBObjectEvent oe in Events) {
				switch (oe.Event) {
					case FBTriggerEvent.Enter:
						oe.Source.GetComponent<FBTriggerZone> ().OnEnter += CheckEnter;
						break;
					case FBTriggerEvent.Exit:
						oe.Source.GetComponent<FBTriggerZone> ().OnExit += CheckExit;
						break;
					case FBTriggerEvent.HitByAxe:
						oe.Source.GetComponent<FBHittable> ().OnHitByAxe += CheckHitByAxe;
						break;
					case FBTriggerEvent.Destroyed:
						oe.Source.GetComponent<FBHittable> ().OnDestroyed += CheckDestroyed;
						break;
					case FBTriggerEvent.Open:
						oe.Source.GetComponent<FBDoor> ().OnOpen += CheckOpen;
						break;
					case FBTriggerEvent.PutOut:
						oe.Source.GetComponent<FBFire> ().OnPutOut += CheckPutOut;
						break;
					case FBTriggerEvent.Draw:
						oe.Source.GetComponent<FBPuppetController> ().OnDraw += CheckDraw;
						break;
					case FBTriggerEvent.Grab:
						oe.Source.GetComponent<FBPuppetController> ().OnGrab += CheckGrab;
						break;
				}
			}
		}//*/
	}

	void CheckHitByAxe (GameObject go) { CheckEvent (FBTriggerEvent.HitByAxe, go); }
	void CheckDestroyed (GameObject go) { CheckEvent (FBTriggerEvent.Destroyed, go); }
	void CheckOpen (GameObject go) { CheckEvent (FBTriggerEvent.Open, go); }
	void CheckPutOut (GameObject go) { CheckEvent (FBTriggerEvent.PutOut, go); }
	void CheckDraw (GameObject go) { CheckEvent (FBTriggerEvent.Draw, go); }
	void CheckGrab (GameObject go) { CheckEvent (FBTriggerEvent.Grab, go); }
	void CheckEnter (GameObject source, GameObject trigger) { CheckEvent (FBTriggerEvent.Enter, source, trigger); }
	void CheckExit (GameObject source, GameObject trigger) { CheckEvent (FBTriggerEvent.Exit, source, trigger); }

	void CheckEvent (FBTriggerEvent ev, GameObject source, GameObject trigger = null) {
		FBObjectEvent found = null;
		if (Events.Count <= 0)
			return;
		foreach (FBObjectEvent oe in Events) {
			if (oe.Event != ev || oe.Source != source)
				return;
			if ((oe.Event == FBTriggerEvent.Enter || oe.Event == FBTriggerEvent.Exit) && oe.Trigger != trigger)
				return;
			found = oe;
			break;
		}
		if (found == null)
			return;
		if (!repeat) {
			Events.Remove (found);
			Destroy (found.gameObject);
			if (Events.Count > 0)
				return;
		}
		float maxDelay = -1.0f;
		foreach (FBObjectAction action in Actions) {
			IEnumerator co = TriggerAction (action);
			if (action.Delay > maxDelay)
				maxDelay = action.Delay;
			if (action.Delay > 0.0f)
				Couroutines.Add (co);
			StartCoroutine (co);
		}

		if (!repeat)
			StartCoroutine (DestroyAfter (maxDelay));
	}

	private IEnumerator DestroyAfter (float delay) {
		if (delay > 0.0f)
			yield return new WaitForSeconds (delay);
		Destroy (gameObject);
	}

	private IEnumerator TriggerAction (FBObjectAction a) {
		if (a.Delay > 0.0f)
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
				FBDoor door = a.Target.GetComponent<FBDoor> ();
				if (door)
					door.Open ();
				else if (a.Target != null)
					a.Target.GetComponent<FBPath> ().open = true;
				break;
			case FBScriptedAction.Close:
				if (a.Target != null)
					a.Target.GetComponent<FBPath> ().open = false;
				break;
			case FBScriptedAction.SetDestructible:
				a.Target.GetComponent<FBHittable> ().destructible = true;
				break;
			case FBScriptedAction.PlayAnimation:
				a.Target.GetComponent<Animator> ().Play (a.strArg);
				break;
			case FBScriptedAction.LoadScene:
				int sceneNumber;
				if (int.TryParse (a.strArg, out sceneNumber))
					SceneManager.LoadSceneAsync (sceneNumber);
				else
					SceneManager.LoadSceneAsync (a.strArg);
				break;
		}
	}
}