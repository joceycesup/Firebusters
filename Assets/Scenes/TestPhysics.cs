using UnityEngine;

public static class BoxColliderExtension {

	public static Collider[] OverlapBox(this BoxCollider bc, int layerMask = Physics.AllLayers) {
		return Physics.OverlapBox (bc.transform.rotation * Vector3.Scale (bc.center, bc.transform.lossyScale) + bc.transform.position, Vector3.Scale (bc.size / 2.0f, bc.transform.lossyScale), bc.transform.rotation);
	}
}

public class TestPhysics : MonoBehaviour {
	private BoxCollider bc;

	void Start () {
		bc = GetComponent<BoxCollider> ();
	}

	void Update () {
		if (Input.GetKeyDown ("e")) {
			Debug.Log ("############################");
			Debug.Log (" Testing Physics.OverlapBox ");
			//Collider[] colls = Physics.OverlapBox (transform.rotation * Vector3.Scale (bc.center, transform.lossyScale) + transform.position, Vector3.Scale (bc.size / 2.0f, transform.lossyScale), transform.rotation);
			Collider[] colls = bc.OverlapBox ();
			foreach (Collider coll in colls) {
				Debug.Log (coll);
			}
		}
	}
}
