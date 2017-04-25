using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class FBWWiseTests : MonoBehaviour {
	public AnimationCurve curve;
	public float maxVelocity;

	void OnCollisionEnter (Collision coll) {
		//.225
		//.600
		if (coll.gameObject.CompareTag ("Player")) {
			float velocity = curve.Evaluate (coll.relativeVelocity.magnitude / maxVelocity);
			//Debug.Log (this + " : " + velocity);

			AkSoundEngine.SetRTPCValue ("velocite", velocity);
			AkSoundEngine.PostEvent ("Play_Marionnette_Autohit", gameObject);
		}
	}
}
