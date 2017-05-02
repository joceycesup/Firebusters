using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBMailBox : FBHittable {
	public float throwForce = 200.0f;
	public float torqueForce = 200.0f;

	protected override void OnHit (Collision collision) {
		base.OnHit (collision);
	}

	protected override void OnHitByAxe (Collision collision) {
		//base.OnHitByAxe (collision);
		Debug.Log (collision.relativeVelocity.magnitude);
		AkSoundEngine.PostEvent ("Play_mailbox_hit", gameObject);
		if (transform.childCount > 0) {
			Transform drawer = transform.GetChild (Random.Range (0, transform.childCount));
			drawer.parent = null;
			Rigidbody rb = drawer.gameObject.AddComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.FreezePositionY;
			drawer.gameObject.AddComponent<FBDrawer> ().torqueForce = torqueForce;
			StartCoroutine (ThrowDrawer (rb));
		}
	}

	private IEnumerator ThrowDrawer (Rigidbody rb) {
		rb.AddForce (transform.forward * -throwForce);
		yield return null;
	}
}
