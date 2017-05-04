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
	public delegate void DestroyedEvent();
	public event DestroyedEvent OnDestroyed;
	
	public bool destructible = false;
	public FBHitSound hitSound = FBHitSound.None;
	public FBAxeSound axeSound = FBAxeSound.None;

	private void Awake () {
		tag = "Hittable";
		Debug.Log (name);
	}

	private void OnCollisionEnter (Collision collision) {
		if (collision.collider.tag == "Axe") {
			OnHitByAxe (collision);
		}
		else {
			OnHit (collision);
		}
	}

	protected virtual void OnHitByAxe (Collision collision) {
		Debug.Log ("Ouille!");
		PlayAxeSound ();
		if (destructible) {
			Transform subItemsContainer = transform.GetChild (0);
			subItemsContainer.gameObject.SetActive (true);
			subItemsContainer.DetachChildren ();
			if (OnDestroyed != null)
				OnDestroyed ();
			Destroy (gameObject);
		}
	}
	protected virtual void OnHit (Collision collision) {
		Debug.Log ("Clonk!");
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
