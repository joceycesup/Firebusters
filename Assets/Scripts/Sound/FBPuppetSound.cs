using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class FBPuppetSound : MonoBehaviour {
	public AnimationCurve curve;
	public float maxVelocity;

	void OnCollisionEnter (Collision coll) {
		//.225
		//.600
		if (coll.gameObject.CompareTag ("Marionette")) {
			float velocity = Mathf.Clamp01 (coll.relativeVelocity.magnitude / maxVelocity);
			//Debug.Log (this + " : " + velocity);

			AkSoundEngine.SetRTPCValue ("velocite", velocity, gameObject);
			AkSoundEngine.PostEvent ("Play_Marionnette_Autohit", gameObject);
		}
	}
}
