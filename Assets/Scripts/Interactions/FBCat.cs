using UnityEngine;

public class FBCat : MonoBehaviour {
	FBPuppetController[] chars;

	void Start () {
		AkSoundEngine.PostEvent ("Play_Cat", gameObject);
		chars = FindObjectsOfType<FBPuppetController> ();
	}

	void Update () {
		if (Time.timeScale <= 0.0f)
			return;
		float value = float.MaxValue;
		foreach (FBPuppetController characatar in chars) {
			float distance = Vector3.Distance (transform.position, characatar.transform.position);
			if (distance < value)
				value = distance;
		}
		AkSoundEngine.SetRTPCValue ("Cattenuation", value, gameObject);
		//Debug.Log ("Cattenuation_RTPC : " + value.ToString ("F2"));
	}
}
