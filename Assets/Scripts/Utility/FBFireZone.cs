using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider))]
public class FBFireZone : MonoBehaviour {
	private static int fire_counter_total;
	private int puppet_counter = 0;
	private int fire_counter = 0;

	private void Start () {
		List<GameObject> fires = new List<GameObject> ();
		foreach (BoxCollider bc in GetComponents<BoxCollider> ()) {
			foreach (Collider c in bc.OverlapBox (1 << 12)) {
				if (!fires.Contains (c.gameObject)) {
					fires.Add (c.gameObject);
					c.GetComponent<FBFire> ().OnPutOut += RemoveFire;
					Debug.Log ("Fire " + c.name + " found in zone " + name);
					fire_counter++;
				}
			}
		}
		Debug.Log (fire_counter + " fires in zone " + name);
	}

	void OnTriggerEnter (Collider coll) {
		//Debug.Log ("Enter " + coll);
		if (coll.CompareTag ("Marionette") && coll.GetComponent<FBPuppetController> () != null) {
			if (puppet_counter <= 0) {
				fire_counter_total += fire_counter;
				UpdateFireCount ();
			}
			puppet_counter++;
		}
	}

	void OnTriggerExit (Collider coll) {
		//Debug.Log ("Exit " + coll);
		if (coll.CompareTag ("Marionette") && coll.GetComponent<FBPuppetController> () != null) {
			puppet_counter--;
			if (puppet_counter <= 0) {
				fire_counter_total -= fire_counter;
				UpdateFireCount ();
			}
		}
	}

	void RemoveFire (GameObject go) {
		go.GetComponent<FBFire> ().OnPutOut -= RemoveFire;
		fire_counter--;
		if (puppet_counter > 0) {
			fire_counter_total--;
			UpdateFireCount ();
		}
	}

	static void UpdateFireCount () {
		Debug.Log ("FBFireZone : Setting Fire_Count to " + fire_counter_total);
		AkSoundEngine.SetRTPCValue ("Fire_Count", fire_counter_total, FBGlobalSoundManager.instance);
	}
}
