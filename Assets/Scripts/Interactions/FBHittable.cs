using UnityEngine;
using System;
//*
[Serializable]
public enum FBHitSound {
	None,
	Metal,
	Wood,
	Ceramic
}
[Serializable]
public enum FBAxeSound {
	None,
	Trolley,
	Wood,
	Mailbox
}//*/

public class FBHittable : MonoBehaviour {
	public delegate void HitEvent(GameObject go);
	public event HitEvent OnDestroyed;
	public event HitEvent OnHit;
	public event HitEvent OnHitByAxe;

	public bool destructible = false;
	public FBHitSound hitSound = FBHitSound.None;
	public FBAxeSound axeSound = FBAxeSound.None;
	public float maxVelocity;

	private void Awake () {
		tag = "Hittable";
		//Debug.Log (name);
	}

	private void OnCollisionEnter (Collision collision) {
		if (collision.collider.tag == "Axe") {
			HitByAxe (collision);
		}
		else {
			Hit (collision);
		}
	}

	protected virtual void HitByAxe (Collision collision) {
		Debug.Log ("Ouille!");
		if (OnHitByAxe != null)
			OnHitByAxe (gameObject);
		PlayAxeSound ();
		if (destructible) {
			Transform subItemsContainer = transform.GetChild (0);
			subItemsContainer.gameObject.SetActive (true);
			subItemsContainer.DetachChildren ();
			if (OnDestroyed != null)
				OnDestroyed (gameObject);
			Destroy (gameObject);
		}
	}
	protected virtual void Hit (Collision collision) {
		Debug.Log ("Clonk!");
		if (OnHit != null)
			OnHit (gameObject);
		float velocity = Mathf.Clamp01 (collision.relativeVelocity.magnitude / maxVelocity);
		AkSoundEngine.SetRTPCValue ("velocite", velocity, gameObject);
		PlayHitSound ();
	}

	protected virtual void PlayAxeSound () {
		switch (axeSound) {
			case FBAxeSound.Mailbox:
				AkSoundEngine.PostEvent ("Play_axe_mailbox_hit", gameObject);
				break;
			case FBAxeSound.Trolley:
				AkSoundEngine.PostEvent ("Play_axe_chariot_hit", gameObject);
				break;
			case FBAxeSound.Wood:
				AkSoundEngine.PostEvent ("Play_axe_wood_hit", gameObject);
				break;
			case FBAxeSound.None:
			default:
				PlayHitSound ();
				break;
		}
	}

	protected virtual void PlayHitSound () {
		switch (hitSound) {
			case FBHitSound.Ceramic:
				AkSoundEngine.PostEvent ("Play_Marionnette_hit_ceramique", gameObject);
				break;
			case FBHitSound.Metal:
				AkSoundEngine.PostEvent ("Play_Marionnette_hit_metal", gameObject);
				break;
			case FBHitSound.Wood:
				AkSoundEngine.PostEvent ("Play_Marionnette_hit_bois", gameObject);
				break;
			case FBHitSound.None:
			default:
				break;
		}
	}
}
