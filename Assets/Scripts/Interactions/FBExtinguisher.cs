using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBExtinguisher : MonoBehaviour {
	private ParticleSystem ps;
	private List<GameObject> fires;
	public float raycastDistance = 5.0f;
	public float force = 50.0f;
	public ForceMode forceMode = ForceMode.Impulse;

	void Awake () {
		ps = gameObject.GetComponent<ParticleSystem> ();
		fires = new List<GameObject> ();
		gameObject.SetActive (false);
	}

	void OnDisable () {
		StopAllCoroutines ();
		fires.Clear ();
	}

	void FixedUpdate () {//*
		Debug.DrawRay (transform.position, transform.forward * raycastDistance, Color.red);
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, raycastDistance, 1 << 9 | 1)) {
			float delay = hit.distance / ps.main.startSpeedMultiplier;
			if (delay < ps.main.startLifetimeMultiplier) {
				if (hit.collider.CompareTag ("Fire")) {
					if (!fires.Contains (hit.collider.gameObject)) {
						StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
					}
				}
				else {
					if (hit.rigidbody) {
						hit.rigidbody.AddForce (transform.forward * force * forceDecrease (hit.distance), forceMode);
					}
				}
			}
		}//*/

		//ps.startLifetime
	}

	private IEnumerator PutOutFire (GameObject fire, float delay) {
		fires.Add (fire);
		yield return new WaitForSeconds (delay);
		fires.Remove (fire);
		if (fire != null) {
			fire.GetComponent<FBFire> ().PutOut ();
		}
	}

	private float forceDecrease (float distance) {
		float res = distance / raycastDistance;
		return 1.0f - res * res * res;
	}
}
