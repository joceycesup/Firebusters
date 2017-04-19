using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class CameraControls : MonoBehaviour {
	private Camera camera;
	public GameObject target;
	[Range (1f, 10f)]
	public float distance;
	[Range (-5f, 5f)]
	public float height = 0f;
	public bool locked;
	public bool iso;
	public Transform lookObject;
	public float rotateSpeed = 90.0f;
	private Vector3 forward;

	void Start () {
		target.transform.position += Vector3.up * height;
		camera = GetComponent<Camera> ();
		forward = target.transform.forward;
		transform.position = target.transform.position - (iso ? 10.0f : distance) * forward;
		if (lookObject != null) {
			transform.rotation = Quaternion.LookRotation (lookObject.position - transform.position);
		}
		else {
			transform.rotation = target.transform.rotation;
		}
		if (locked)
			transform.parent = target.transform;
		if (iso) {
			camera.orthographic = true;
			camera.orthographicSize = 2.0f;
		}
	}

	void Update () {
		if (!locked) {
			transform.position = target.transform.position - distance * forward;
		}
		target.transform.Rotate (target.transform.right, Input.GetAxis ("VerticalR") * rotateSpeed * Time.deltaTime);
	}
}
