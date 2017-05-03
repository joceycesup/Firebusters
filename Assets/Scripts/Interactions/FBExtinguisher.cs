using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBExtinguisher : MonoBehaviour {
	private ParticleSystem ps;
	private List<GameObject> fires;
	public float raycastDistance = 5.0f;

	void Awake () {
		ps = gameObject.GetComponent<ParticleSystem> ();
		fires = new List<GameObject> ();
	}

	void OnDisable () {
		StopAllCoroutines ();
		fires.Clear ();
	}

	void Update () {
		Debug.DrawRay (transform.position, transform.forward * raycastDistance, Color.red);
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, raycastDistance, 1 << 9)) {
			float delay = hit.distance / ps.main.startSpeedMultiplier;
			if (hit.collider.CompareTag ("Fire") && delay < ps.main.startLifetimeMultiplier) {
				if (!fires.Contains (hit.collider.gameObject)) {
					StartCoroutine (PutOutFire (hit.collider.gameObject, delay));
				}
			}
		}

		//ps.startLifetime
	}

	private IEnumerator PutOutFire (GameObject fire, float delay) {
		fires.Add (fire);
		yield return new WaitForSeconds (delay);
		fires.Remove (fire);
		Destroy (fire);
	}
}
