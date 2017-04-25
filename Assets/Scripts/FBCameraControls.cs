using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class FBCameraControls : MonoBehaviour {
	private Camera _camera;

	public Transform target;
	public Transform position;
	public Transform direction;

	private Vector3 forward;
	private Vector3 relativePosition;
	private Vector3 relativeTarget;

	void Awake () {
		_camera = GetComponent<Camera> ();
		relativePosition = Vector3.ProjectOnPlane (position.position - direction.position, direction.right);
		relativeTarget = Vector3.ProjectOnPlane (target.position - direction.position, direction.right);
	}

	void Update () {
		forward = direction.forward;
		forward.y = 0.0f;
		forward = Vector3.Normalize (forward);

		transform.position = direction.position + forward * relativePosition.z + Vector3.up * relativePosition.y;

		Vector3 tmp = direction.position + forward * relativeTarget.z + Vector3.up * relativeTarget.y;

		transform.rotation = Quaternion.LookRotation (tmp - transform.position);
	}
}
