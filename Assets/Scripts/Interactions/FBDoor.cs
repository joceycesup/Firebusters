using UnityEngine;

public class FBDoor : MonoBehaviour {
	public delegate void DoorEvent (GameObject source);
	public event DoorEvent OnOpen;
	private MeshRenderer mr;
	private Color emissiveColor;
	public float emissiveWhenGrabbable = 0.8f;
	public bool isLocked = true;

	void Start () {
		mr = GetComponent<MeshRenderer> ();
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.centerOfMass = Vector3.zero;
		rb.inertiaTensor = Vector3.zero;
		rb.inertiaTensorRotation = Quaternion.identity;
		emissiveColor = mr.material.GetColor ("_EmissionColor");
		CanGrabDoorKnob (false);
		if (!isLocked)
			Open ();
	}

	public void CanGrabDoorKnob (bool value) {
		if (mr)
			mr.material.SetColor ("_EmissionColor", emissiveColor * Mathf.LinearToGammaSpace (value ? emissiveWhenGrabbable : 0.0f));
	}

	public void Open () {
#if DEBUG_ENABLED
		Debug.Log ("Door " + name + " try open");
#endif
		if (!isLocked)
			return;
		GetComponent<Rigidbody> ().isKinematic = false;
		if (OnOpen != null)
			OnOpen (gameObject);
		isLocked = false;
		//Destroy (this);
	}
}
