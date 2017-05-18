using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FBTriggerZone : MonoBehaviour {
	
	void OnTriggerEnter (Collider coll) {
		Debug.Log ("Enter " + coll);
	}
	
	void OnTriggerExit (Collider coll) {
		Debug.Log ("Exit " + coll);
	}
}
