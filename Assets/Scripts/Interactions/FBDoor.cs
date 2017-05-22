using UnityEngine;

public class FBDoor : MonoBehaviour {
	public delegate void DoorEvent (GameObject source);
	public event DoorEvent OnOpen;

	public void Open () {
		OnOpen (gameObject);
	}
}
