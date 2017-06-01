using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class FBExtinguisher : FBTool {
	private ParticleSystem ps;
	private ParticleSystem psFaisceau;
	private List<GameObject> fires;
	public float raycastDistance = 5.0f;
	public AnimationCurve forceDecrease;
	public float force = 50.0f;
	public ForceMode forceMode = ForceMode.Impulse;

	[Range (1, 20)]
	public int testRate = 1;
	private int testFrame = 0;

	[Range (1.0f, 50.0f)]
	public float raycastDivisions = 10.0f;

	void Awake () {
		ps = gameObject.GetComponent<ParticleSystem> ();
		psFaisceau = transform.GetChild (0).GetComponent<ParticleSystem> ();
		ps.Stop ();
		psFaisceau.Stop ();
		fires = new List<GameObject> ();
		raycastDivisions = Mathf.Round (raycastDivisions) - 0.01f;
	}

	public override void Enable () {
		ps.Play ();
	}

	public override void Disable () {
		ps.Stop ();
	}

	void FixedUpdate () {
		testFrame++;
		if ((testFrame = (testFrame + 1) % testRate) != 0)
			return;

		if (ps.isPlaying) {
			Debug.Log (Time.time);
			Ray ray = new Ray ();
			float maxDistance = float.MaxValue;
			RaycastHit[] hits;

			float raycastDelta = psFaisceau.main.startLifetimeMultiplier / raycastDivisions;
			float distance = 0.0f;
			float totalSpeed = 0.0f;

			//Debug.Log (ps.main.startSpeedMultiplier + " ; " + ps.main.startLifetimeMultiplier + " ; " + ps.main.gravityModifierMultiplier);
			Vector3 speed = transform.forward * psFaisceau.main.startSpeedMultiplier;
			Vector3 position = transform.position;
			float time = 0.0f;
			float division = 1.0f;
			while (time < psFaisceau.main.startLifetimeMultiplier) {
#if DEBUG_ENABLED
				Vector3 oldDirection = speed;
#endif
				totalSpeed += speed.magnitude;
				speed += psFaisceau.main.gravityModifierMultiplier * Physics.gravity * raycastDelta;

				maxDistance = float.MaxValue;
				ray = new Ray (position, speed);
				hits = Physics.RaycastAll (ray, speed.magnitude * raycastDelta, 0x1301); // 8 : Walkable, 9 : VerticalObstacle, 1 : Default, 12 : Fire

				foreach (RaycastHit hit in hits) {
					if (hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 8) {
						time = psFaisceau.main.startLifetimeMultiplier;
						if (hit.distance < maxDistance)
							maxDistance = hit.distance;
					}
					else if (hit.rigidbody && !hit.rigidbody.isKinematic) {
						hit.rigidbody.AddForce (transform.forward * force * forceDecrease.Evaluate (((distance + hit.distance) / (totalSpeed / division)) / psFaisceau.main.startLifetimeMultiplier), forceMode);
					}
				}
				foreach (RaycastHit hit in hits) {
					if (hit.collider.gameObject.layer == 12) {
						float delay = (distance + hit.distance) / (totalSpeed / division);
						if (delay < psFaisceau.main.startLifetimeMultiplier && hit.distance < maxDistance) {
							if (!fires.Contains (hit.collider.gameObject)) {
								StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
							}
						}
					}
				}


#if DEBUG_ENABLED
				Debug.DrawRay (position, speed * raycastDelta, Color.red, Time.fixedDeltaTime * testRate);
#endif
				distance += (position - (position += speed * raycastDelta)).magnitude;
#if DEBUG_ENABLED
				Debug.DrawLine (position, position + (speed.normalized - oldDirection.normalized).normalized / -5.0f, new Color (1.0f, 0.5f, 0.0f), Time.fixedDeltaTime * testRate);
#endif
				time += raycastDelta;
				division += 1.0f;
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
