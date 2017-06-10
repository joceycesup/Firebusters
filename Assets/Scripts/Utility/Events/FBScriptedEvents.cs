using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FBScriptedEvents : MonoBehaviour {
	[SerializeField]
	public List<FBEvent> zones = new List<FBEvent> ();
	/*
	void Awake () {
		foreach (FBEvent e in zones) {
			if (e.Setup ()) {
				foreach (FBObjectEvent oe in e.Events) {
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
			}
		}
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
		FBEvent foundEvent = null;
		foreach (FBEvent e in zones) {//*
			foreach (FBObjectEvent oe in e.Events) {
				if (oe.Event != ev) {
					continue;
				}
				if (oe.Source != source)
					continue;
				if (oe.Event == FBTriggerEvent.Enter || oe.Event == FBTriggerEvent.Exit)
					if (oe.Trigger != trigger)
						continue;
				found = oe;
			}
			if (found == null)
				continue;
			if (!e.repeat) {
				e.Events.Remove (found);
				Destroy (found.gameObject);
				if (e.Events.Count > 0)
					continue;
			}
			foundEvent = e;
			foreach (FBObjectAction action in e.Actions) {
				IEnumerator co = TriggerAction (action);
				if (action.Delay > 0.0f)
					e.Couroutines.Add (co);
				StartCoroutine (co);
			}
			break;
		}

		if (foundEvent && !foundEvent.repeat) {
			zones.Remove (foundEvent);
			Destroy (foundEvent.gameObject);
		}
	}

	private IEnumerator TriggerAction (FBObjectAction a) {
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
		}
	}//*/
}
