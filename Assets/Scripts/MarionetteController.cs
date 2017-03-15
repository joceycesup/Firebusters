using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarionetteController : MonoBehaviour {
	public float maxXAngle = 25.0f;
	public float maxZAngle = 25.0f;
	public float yAngle = 0.0f;
	public float turnRate = 45.0f;
	
	void Update () {
		yAngle -= turnRate * Time.deltaTime * Input.GetAxis ("HorizontalR");
		transform.rotation = Quaternion.Euler (Input.GetAxis ("VerticalL") * maxXAngle, 0.0f, Input.GetAxis ("HorizontalL") * -maxZAngle);
		transform.Rotate (Vector3.up * yAngle, Space.Self);
	}
}
