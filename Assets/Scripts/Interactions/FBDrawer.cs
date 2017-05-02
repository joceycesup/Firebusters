using UnityEngine;

public class FBDrawer : MonoBehaviour {
	public float torqueForce;

	private void OnTriggerExit (Collider coll) {
		if (coll.CompareTag ("Hittable")) {
			Debug.Log (name + " is free!!");
			Rigidbody rb = GetComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.None;
			rb.AddTorque ((transform.right + Random.Range (0.0f, 0.1f) * transform.forward) * torqueForce, ForceMode.Impulse);
			GetComponent<BoxCollider> ().isTrigger = false;
			Destroy (this);
		}
	}
}
