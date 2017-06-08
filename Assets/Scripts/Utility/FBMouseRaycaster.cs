using UnityEngine;

public class FBMouseRaycaster : MonoBehaviour {
	public Camera cam;


	void Update () {
		RaycastHit hit;
		if (Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hit)) {
			FBButton button = hit.collider.gameObject.GetComponent<FBButton> ();
			if (button) {
				if (Input.GetMouseButtonDown (0)) {
					button.Click ();
				}
				else {
					button.Hover ();
				}
			}
		}
	}
}
