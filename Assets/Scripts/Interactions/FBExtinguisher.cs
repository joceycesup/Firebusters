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
			float maxDistance = float.MaxValue;
			RaycastHit[] hits = Physics.RaycastAll (ray, raycastDistance, (1 << 9) | 1); // 9 : VerticalObstacle, 1 : Default, 12 : Fire
			foreach (RaycastHit hit in hits) {
				if (hit.collider.gameObject.layer == 9) {
					if (hit.distance < maxDistance) {
						maxDistance = hit.distance;
					}
				}
				else if (hit.rigidbody && !hit.rigidbody.isKinematic) {
					hit.rigidbody.AddForce (transform.forward * force * forceDecrease.Evaluate (hit.distance / raycastDistance), forceMode);
				}
			}//*/
			hits = Physics.RaycastAll (ray, raycastDistance, 1 << 12); // 9 : VerticalObstacle, 1 : Default, 12 : Fire
			foreach (RaycastHit hit in hits) {
				float delay = hit.distance / ps.main.startSpeedMultiplier;
				if (delay < ps.main.startLifetimeMultiplier && hit.distance < maxDistance) {
					if (!fires.Contains (hit.collider.gameObject)) {
						StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
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
