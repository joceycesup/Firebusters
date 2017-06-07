using UnityEngine;
using System;
//*
[Serializable]
public enum FBHitSound {
	None,
	Metal,
	Wood,
	Ceramic,
	Pan,
	Chandelier,
	Chair
}
[Serializable]
public enum FBAxeSound {
	None,
	Trolley,
	Wood,
	Mailbox
}//*/

public class FBHittable : MonoBehaviour {

	public delegate void HitEvent (GameObject go);
	public event HitEvent OnDestroyed;
	public event HitEvent OnHit;
	public event HitEvent OnHitByAxe;

	public bool destructible = false;
	public FBHitSound hitSound = FBHitSound.None;
	public FBAxeSound axeSound = FBAxeSound.None;
	public static float _maxVelocity;
	[SerializeField]
	public float maxVelocity {
		get { return _maxVelocity; }
		set { _maxVelocity = value; }
	}

	private void Awake () {
		tag = "Hittable";
		//Debug.Log (name);
	}
#if UNITY_EDITOR
	private void Start () {
		gameObject.AddComponent<FBInteractInEditor> ().hittable = this;
	}
#endif

	private void OnCollisionEnter (Collision collision) {
		AkSoundEngine.SetRTPCValue ("Velocite_Props", collision.relativeVelocity.magnitude / maxVelocity, gameObject);
		if (collision.collider.tag == "Axe") {
			HitByAxe (collision);
		}
		else {
			Hit (collision);
		}
	}

	public virtual void HitByAxe (Collision collision = null) {
		//Debug.Log ("Ouille!");
		if (OnHitByAxe != null)
			OnHitByAxe (gameObject);
		PlayAxeSound ();
		DestroyHittable ();
	}
	public virtual void Hit (Collision collision = null) {
		//Debug.Log ("Clonk!");
		if (OnHit != null)
			OnHit (gameObject);
		float velocity = 0.0f;
		if (collision != null)
			velocity = Mathf.Clamp01 (collision.relativeVelocity.magnitude / maxVelocity);
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
			case FBHitSound.Pan:
				AkSoundEngine.PostEvent ("Play_Marionnette_hit_casserole", gameObject);
				break;
			case FBHitSound.Chandelier:
				AkSoundEngine.PostEvent ("Play_Marionnette_hit_chandelier", gameObject);
				break;
			case FBHitSound.Chair:
				AkSoundEngine.PostEvent ("Play_Marionnette_hit_fauteuil", gameObject);
				break;
			case FBHitSound.None:
			default:
				break;
		}
	}

	protected void DestroyHittable () {
		if (destructible) {
			Transform subItemsContainer = transform.GetChild (0);
			if (subItemsContainer.gameObject.activeInHierarchy) {
				for (int i = 0; i < subItemsContainer.childCount; ++i) {
					Rigidbody rb = subItemsContainer.GetChild (i).gameObject.GetComponent<Rigidbody> ();
					if (rb)
						rb.isKinematic = false;
				}
			}
			else {
				subItemsContainer.gameObject.SetActive (true);
			}
			subItemsContainer.DetachChildren ();
			if (OnDestroyed != null)
				OnDestroyed (gameObject);
			Destroy (gameObject);
		}
	}

	void ActiveRigidbody (GameObject go) {
	}
}
