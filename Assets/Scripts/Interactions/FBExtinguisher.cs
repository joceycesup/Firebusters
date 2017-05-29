using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBExtinguisher : FBTool {
	private ParticleSystem ps;
	private List<GameObject> fires;
	public float raycastDistance = 5.0f;
	public AnimationCurve forceDecrease;
	public float force = 50.0f;
	public ForceMode forceMode = ForceMode.Impulse;

	void Awake () {
		ps = gameObject.GetComponent<ParticleSystem> ();
		ps.Stop ();
		fires = new List<GameObject> ();
	}

	public override void Enable () {
		ps.Play ();
	}

	public override void Disable () {
		ps.Stop ();
	}

	void FixedUpdate () {//*
		if (ps.isPlaying) {
			Debug.DrawRay (transform.position, transform.forward * raycastDistance, Color.red);
			Ray ray = new Ray (transform.position, transform.forward);
			RaycastHit[] hits = Physics.RaycastAll (ray, raycastDistance, (1 << 9) | 1 | (1 << 12));
			foreach (RaycastHit hit in hits) { // 9 : VerticalObstacle
				float delay = hit.distance / ps.main.startSpeedMultiplier;
				if (delay < ps.main.startLifetimeMultiplier) {
					if (hit.collider.CompareTag ("Fire")) {
						if (!fires.Contains (hit.collider.gameObject)) {
							StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
						}
					}
					else if (hit.rigidbody && !hit.rigidbody.isKinematic) {
						hit.rigidbody.AddForce (transform.forward * force * forceDecrease.Evaluate (hit.distance), forceMode);
					}
				}
			}//*/
		}

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
}
