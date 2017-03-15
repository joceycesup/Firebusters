using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarionetteController : MonoBehaviour {
	public float maxXAngle = 25.0f;
	public float maxZAngle = 25.0f;
	public float yAngle = 0.0f;
	public float turnRate = 45.0f;
	
	void Update () {
		transform.rotation = Quaternion.Euler (Input.GetAxis ("Vertical") * maxXAngle, 0.0f, Input.GetAxis ("Horizontal") * -maxZAngle);
	}
}
