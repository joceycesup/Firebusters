using UnityEngine;
using System.Collections;

public class PuppetController : MonoBehaviour {
	private Transform controllerTransform;
	public float turnRate = 90;
	public float tiltRate = 90;
	public float pitchRate = 90;
	public float speed = 5;

	void Awake () {
		controllerTransform = transform.GetChild (1);
	}
	
	// Update is called once per frame
	void Update () {/*
		if (controllerExtension.Length > 0)
			transform.position += Vector3.Normalize (new Vector3 (Input.GetAxis ("Horizontal" + controllerExtension), 0f, Input.GetAxis ("Vertical" + controllerExtension))) * speed * Time.deltaTime;//*/
		transform.Rotate (0, Input.GetAxis ("HorizontalL") * turnRate * Time.deltaTime, 0);
		transform.Translate (0, 0, Input.GetAxis ("VerticalL") * speed * Time.deltaTime);
		controllerTransform.Rotate (0, 0, -Input.GetAxis ("HorizontalR") * tiltRate * Time.deltaTime);
		controllerTransform.Rotate (Input.GetAxis ("VerticalR") * pitchRate * Time.deltaTime, 0, 0);
	}
}
