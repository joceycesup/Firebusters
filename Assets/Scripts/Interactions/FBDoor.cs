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
		if (mr == null)
			mr = GetComponentInChildren<MeshRenderer> ();
		Rigidbody rb = GetComponent<Rigidbody> ();
		rb.centerOfMass = Vector3.zero;
		//rb.inertiaTensor = Vector3.zero;// new Vector3 (float.Epsilon, float.Epsilon, float.Epsilon);
		rb.inertiaTensorRotation = Quaternion.identity;
		emissiveColor = mr.material.GetColor ("_EmissionColor");
		CanGrabDoorKnob (false);
		if (!isLocked)
			Open (true);
		else
			GetComponent<Rigidbody> ().isKinematic = true;
#if UNITY_EDITOR
		gameObject.AddComponent<FBInteractInEditor> ().door = this;
#endif
	}

	public void CanGrabDoorKnob (bool value) {
		if (mr)
			mr.material.SetColor ("_EmissionColor", emissiveColor * Mathf.LinearToGammaSpace (value ? emissiveWhenGrabbable : 0.0f));
	}

	public void Open (bool start = false) {
#if DEBUG_ENABLED
		Debug.Log ("Door " + name + " try open");
#endif
		if (!isLocked && !start)
			return;
		if (OnOpen != null)
			OnOpen (gameObject);
		gameObject.layer = 13;//Door layer, doesn't interact with VerticalObstacle
		GetComponent<Rigidbody> ().isKinematic = start;
		isLocked = false;
		//Destroy (this);
	}

	private void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.layer != 13 || collision.gameObject.GetComponent<FBDoor> ())
			return;
		GetComponent<Rigidbody> ().isKinematic = true;
	}
}
