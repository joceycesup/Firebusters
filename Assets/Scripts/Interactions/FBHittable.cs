using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBHittable : MonoBehaviour {

	private void Awake () {
		tag = "Hittable";
	}

	private void OnCollisionEnter (Collision collision) {
		if (collision.collider.tag == "Axe") {
			OnHitByAxe (collision);
		}
		else {
			OnHit (collision);
		}
	}

	protected virtual void OnHitByAxe (Collision collision) {
		Debug.Log ("Ouille!");
	}
	protected virtual void OnHit (Collision collision) {
		Debug.Log ("Clonk!");
	}
}
