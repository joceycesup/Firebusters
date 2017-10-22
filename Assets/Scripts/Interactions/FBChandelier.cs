using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class FBChandelier : MonoBehaviour {
	private Rigidbody rb;
	private Vector3 lastVelocity;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		AkSoundEngine.PostEvent ("Play_Squeak", gameObject);
	}

	void FixedUpdate () {
		if (lastVelocity.z * rb.velocity.z < 0.0f && rb.velocity.z < 0.0f) {
			AkSoundEngine.SetRTPCValue ("Angle_Lustre", Vector3.Angle (Vector3.up, transform.up), gameObject);
		}
		lastVelocity = rb.velocity;
	}
}
