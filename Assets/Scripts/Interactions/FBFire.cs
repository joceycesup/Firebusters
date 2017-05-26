using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBFire : MonoBehaviour {
	public delegate void FireEvent (GameObject go);
	public event FireEvent OnPutOut;

	private float putOutTime = 0.5f;
	private Light fireLight;

	private void Start () {
		fireLight = GetComponent<Light> ();
	}

	public void PutOut () {
		if (OnPutOut != null)
			OnPutOut (gameObject);
		StartCoroutine (PutOutInternal ());
	}

	private IEnumerator PutOutInternal () {
		Vector3 initialScale = transform.localScale;
		float endTime = Time.time + putOutTime;
		float factor = 0.0f;
		do {
			factor = (endTime - Time.time) / putOutTime;
			transform.localScale = Vector3.Lerp (initialScale, Vector3.zero, 1.0f - factor);
			fireLight.intensity *= factor;
			yield return null;
		} while (factor > 0.0f);
		Destroy (gameObject);
	}
}
