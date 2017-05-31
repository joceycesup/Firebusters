using UnityEngine;

public class FBDrawer : MonoBehaviour {
	public float torqueForce;

	private void OnTriggerExit (Collider coll) {
		if (coll.CompareTag ("Hittable")) {
			//Debug.Log (name + " is free!!");
			Rigidbody rb = GetComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.None;
			rb.AddTorque ((transform.right + Random.Range (0.0f, 0.1f) * transform.forward) * torqueForce, ForceMode.Impulse);
			if (transform.childCount > 0) {
				Destroy (GetComponent<BoxCollider> ());
				transform.GetChild (0).gameObject.SetActive (true);
			}
			else
				GetComponent<BoxCollider> ().isTrigger = false;
			Destroy (this);
		}
	}
}
