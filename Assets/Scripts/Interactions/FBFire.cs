using System.Collections;
using UnityEngine;

public class FBFire : MonoBehaviour {
	public delegate void FireEvent (GameObject go);
	public event FireEvent OnPutOut;

	private float putOutTime = 0.5f;
	private Light fireLight;
	public bool beingPutOut {
		get;
		private set;
	}

	private void Start () {
		beingPutOut = false;
		fireLight = GetComponent<Light> ();
		FBHittable hittable = GetComponent<FBHittable> ();
		if (hittable) {
			hittable.destructible = false;
		}
#if UNITY_EDITOR
		gameObject.AddComponent<FBInteractInEditor> ().fire = this;
#endif
	}

	public void PutOut () {
		if (beingPutOut)
			return;
		beingPutOut = true;
		if (OnPutOut != null)
			OnPutOut (gameObject);
		StartCoroutine (PutOutInternal ());
	}

	private IEnumerator PutOutInternal () {
		FBHittable hittable = GetComponent<FBHittable> ();

		Transform t = hittable ? transform.GetChild (0) : transform;
		Vector3 initialScale = t.localScale;
		float endTime = Time.time + putOutTime;
		float factor = 0.0f;
		do {
			factor = (endTime - Time.time) / putOutTime;
			t.localScale = Vector3.Lerp (initialScale, Vector3.zero, 1.0f - factor);
			fireLight.intensity *= factor;
			yield return null;
		} while (factor > 0.0f);

		if (hittable) {
			gameObject.layer = 1;
			hittable.destructible = true;
			Destroy (fireLight);
			Destroy (transform.GetChild (0).gameObject);
			Destroy (this);
		}
		else {
			Destroy (gameObject);
		}
	}
}
