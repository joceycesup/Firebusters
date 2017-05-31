using System.Collections;
using UnityEngine;

public class FBMailBox : FBHittable {
	public float throwForce = 200.0f;
	public float torqueForce = 200.0f;
	public int itemsPerHit = 1;
#if UNITY_EDITOR
	private void Start () {
		gameObject.AddComponent<FBHitInEditor> ().hittable = this;
	}
#endif

	public override void Hit (Collision collision = null) {
		base.Hit (collision);
	}

	public override void HitByAxe (Collision collision = null) {
		base.HitByAxe (collision);
		//Debug.Log (collision.relativeVelocity.magnitude);
		for (int i = 0; i < itemsPerHit; ++i) {
			if (transform.childCount > 0) {
				Transform drawer = transform.GetChild (Random.Range (0, transform.childCount));
				drawer.parent = null;
				Rigidbody rb = drawer.gameObject.GetComponent<Rigidbody> ();
				if (rb == null)
					rb = drawer.gameObject.AddComponent<Rigidbody> ();
				rb.constraints = RigidbodyConstraints.FreezePositionY;
				rb.isKinematic = false;
				drawer.gameObject.AddComponent<FBDrawer> ().torqueForce = torqueForce;
				StartCoroutine (ThrowDrawer (rb));
			}
			else {
				break;
			}
		}
	}

	private IEnumerator ThrowDrawer (Rigidbody rb) {
		rb.AddForce (transform.forward * -throwForce);
		yield return null;
	}
}
