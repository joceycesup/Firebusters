using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarionetteController : MonoBehaviour {
	public float maxXAngle = 25.0f;
	public float maxZAngle = 25.0f;
	public float yAngle = 0.0f;
	public float turnRate = 45.0f;
	public float speed = 4.0f;

	public float maxYAngle = 25.0f;
	private float period = 1.0f;

	[Range (0.01f, 3.0f)]
	public float timeScale = 1.0f;

	public bool autoControl = false;

	void FixedUpdate () {
		Time.timeScale = timeScale;
		if (autoControl) {
			float factor = Mathf.Sin (Time.time * period * Mathf.PI);
			yAngle = maxYAngle * factor;


			transform.Translate (Vector3.up * speed * Time.deltaTime * Input.GetAxis ("Up"), Space.World);
			//transform.Translate (Vector3.forward * speed * Time.deltaTime * Input.GetAxis ("Forward"), Space.World);
			transform.rotation = Quaternion.Euler (0f, 0f, 0f);

			Vector3 rotation = Camera.main.transform.right;
			rotation.y = 0f;
			rotation = Vector3.Normalize (rotation) * (1f - Mathf.Cos (yAngle * 2f * Mathf.Deg2Rad)) * maxXAngle;
			transform.Rotate (rotation, Space.World);

			rotation = Camera.main.transform.forward;
			rotation.y = 0f;
			rotation = Vector3.Normalize (rotation) * Mathf.Sin (yAngle * Mathf.Deg2Rad) * maxZAngle;
			transform.Rotate (rotation, Space.World);

			transform.Rotate (Vector3.up * yAngle, Space.Self);
		}
		else {
			transform.Translate (Vector3.up * speed * Time.deltaTime * Input.GetAxis ("Up"), Space.World);
			//transform.Translate (Vector3.forward * speed * Time.deltaTime * Input.GetAxis ("Forward"), Space.World);
			yAngle += turnRate * Time.deltaTime * Input.GetAxis ("HorizontalR");
			transform.rotation = Quaternion.Euler (0f, 0f, 0f);

			Vector3 rotation = Camera.main.transform.right;
			rotation.y = 0f;
			rotation = Vector3.Normalize (rotation) * Input.GetAxis ("VerticalL") * maxXAngle;
			transform.Rotate (rotation, Space.World);

			rotation = Camera.main.transform.forward;
			rotation.y = 0f;
			rotation = Vector3.Normalize (rotation) * Input.GetAxis ("HorizontalL") * -maxZAngle;
			transform.Rotate (rotation, Space.World);
			//Quaternion.Euler (Input.GetAxis ("VerticalL") * maxXAngle, 0.0f, Input.GetAxis ("HorizontalL") * -maxZAngle);
			transform.Rotate (Vector3.up * yAngle, Space.Self);
		}
	}
}
