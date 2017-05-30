using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class FBExtinguisher : FBTool {
	private ParticleSystem ps;
	private List<GameObject> fires;
	public float raycastDistance = 5.0f;
	public AnimationCurve forceDecrease;
	public float force = 50.0f;
	public ForceMode forceMode = ForceMode.Impulse;

	[Range (1.0f, 20.0f)]
	public float raycastDivisions = 10.0f;

	void Awake () {
		ps = gameObject.GetComponent<ParticleSystem> ();
		ps.Stop ();
		fires = new List<GameObject> ();
		raycastDivisions = Mathf.Round (raycastDivisions) - 0.01f;
	}

	public override void Enable () {
		ps.Play ();
	}

	public override void Disable () {
		ps.Stop ();
	}

	void Update () {
#if DEBUG_ENABLED
		if (!Application.isPlaying)
			raycastDivisions = Mathf.Round (raycastDivisions) - 0.01f;
#endif
		if (ps.isPlaying) {/*
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
			}
			hits = Physics.RaycastAll (ray, raycastDistance, 1 << 12); // 9 : VerticalObstacle, 1 : Default, 12 : Fire
			foreach (RaycastHit hit in hits) {
				float delay = hit.distance / ps.main.startSpeedMultiplier;
				if (delay < ps.main.startLifetimeMultiplier && hit.distance < maxDistance) {
					if (!fires.Contains (hit.collider.gameObject)) {
						StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
					}
				}
			}//*/

			Ray ray = new Ray ();
			float maxDistance = float.MaxValue;
			RaycastHit[] hits;

			float raycastDelta = ps.main.startLifetimeMultiplier / raycastDivisions;
			float distance = 0.0f;
			float totalSpeed = 0.0f;

			//Debug.Log (ps.main.startSpeedMultiplier + " ; " + ps.main.startLifetimeMultiplier + " ; " + ps.main.gravityModifierMultiplier);
			Vector3 speed = transform.forward * ps.main.startSpeedMultiplier;
			Vector3 position = transform.position;
			float time = 0.0f;
			float i = 1.0f;
			while (time < ps.main.startLifetimeMultiplier) {
#if DEBUG_ENABLED
				Vector3 oldDirection = speed;
#endif
				totalSpeed += speed.magnitude;
				speed += ps.main.gravityModifierMultiplier * Physics.gravity * raycastDelta;

				maxDistance = float.MaxValue;
				ray = new Ray (position, speed);
				hits = Physics.RaycastAll (ray, speed.magnitude * raycastDelta, 0x1301); // 8 : Walkable, 9 : VerticalObstacle, 1 : Default, 12 : Fire

				foreach (RaycastHit hit in hits) {
					if (hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 8) {
						time = ps.main.startLifetimeMultiplier;
						if (hit.distance < maxDistance)
							maxDistance = hit.distance;
					}
					else if (hit.rigidbody && !hit.rigidbody.isKinematic) {
						hit.rigidbody.AddForce (transform.forward * force * forceDecrease.Evaluate (((distance + hit.distance) / (totalSpeed / i)) / ps.main.startLifetimeMultiplier), forceMode);
					}
				}
				foreach (RaycastHit hit in hits) {
					if (hit.collider.gameObject.layer == 12) {
						float delay = (distance + hit.distance) / (totalSpeed / i);
						if (delay < ps.main.startLifetimeMultiplier && hit.distance < maxDistance) {
							if (!fires.Contains (hit.collider.gameObject)) {
								StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
							}
						}
					}
				}


#if DEBUG_ENABLED
				Debug.DrawRay (position, speed * raycastDelta, Color.red);
#endif
				distance += (position - (position += speed * raycastDelta)).magnitude;
#if DEBUG_ENABLED
				Debug.DrawLine (position, position + (speed.normalized - oldDirection.normalized).normalized / -5.0f, new Color (1.0f, 0.5f, 0.0f));
#endif
				time += raycastDelta;
				i += 1.0f;
			}
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
