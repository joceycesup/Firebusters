using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (Collider))]
public class FBPuppetSound : MonoBehaviour {

	void OnCollisionEnter (Collision coll) {
		//.225
		//.600
		if (coll.gameObject.CompareTag ("Marionette")) {
			float velocity = Mathf.Clamp01 (coll.relativeVelocity.magnitude / FBGlobalSoundManager.puppetMaxVelocity);
			//Debug.Log (this + " : " + velocity);

			AkSoundEngine.SetRTPCValue ("velocite", FBGlobalSoundManager.puppetSoundCurve.Evaluate (velocity), gameObject);
			AkSoundEngine.PostEvent ("Play_Marionnette_Autohit", gameObject);
		}
	}
}
