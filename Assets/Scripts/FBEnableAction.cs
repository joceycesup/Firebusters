using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBEnableAction : MonoBehaviour {

	private void OnTriggerEnter (Collider other) {
		if (other.CompareTag ("Marionette")) {
			FBMotionAnalyzer motion = other.gameObject.GetComponent<FBMotionAnalyzer> ();
			if (motion != null) {
				Destroy (other.gameObject.GetComponent<Rigidbody> ());
				motion.ToggleAbilities (motion.isAxePuppet ? FBAction.Strike : FBAction.Draw);
				Destroy (other);
				Destroy (gameObject);
			}
		}
	}
}
