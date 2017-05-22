using UnityEngine;

public class FBTriggerZone : MonoBehaviour {
	public delegate void ColliderEvent (GameObject source, GameObject trigger);
	public event ColliderEvent OnEnter;
	public event ColliderEvent OnExit;

	void OnTriggerEnter (Collider coll) {
		//Debug.Log ("Enter " + coll);
		if (OnEnter != null)
			OnEnter (gameObject, coll.gameObject);
	}

	void OnTriggerExit (Collider coll) {
		//Debug.Log ("Exit " + coll);
		if (OnExit != null)
			OnExit (gameObject, coll.gameObject);
	}
}
