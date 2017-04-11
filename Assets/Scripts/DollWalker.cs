using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollWalker : MonoBehaviour {
	public float legsSpan = 0.6f;
	public float stepDistance = 1.5f;
	public float centerOffset = -1.0f;
	public Transform controller;
	public Transform leftFootAnchor;
	public Transform rightFootAnchor;

	private bool leftFootOnFloor = true;
	private bool takingStep = false;

	private bool walkingUpStairs = false;

	public float speed = 4.0f;

	public float walking = 0.0f;

	void Start () {

	}

	void Update () {
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		//walking = Input.GetAxis ("Forward");

		if (walking > 0.0f) {
			transform.Translate (forward * (speed * Time.deltaTime * walking), Space.World);
			if (!takingStep) {
				StartCoroutine ("Step2");
			}
		}
	}

	IEnumerator Step () {
		takingStep = true;
		//Debug.Log ("taking step " + (leftFootOnFloor ? "right" : "left"));
		do {
			Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
			Vector3 target = controller.position;
			target.y = (leftFootOnFloor ? leftFootAnchor : rightFootAnchor).position.y;
			target += (leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * Vector3.Normalize (Vector3.ProjectOnPlane (controller.right, Vector3.up));
			target += forward * (stepDistance + centerOffset);

			(leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position = Vector3.Lerp ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target,
				(speed * 2.0f * Time.deltaTime) /
				Vector3.Distance ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target));
			if ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position == target)
				break;
			yield return null;
		} while (true);
		//Debug.Log ("finished step " + (leftFootOnFloor ? "right" : "left"));
		leftFootOnFloor = !leftFootOnFloor;
		takingStep = false;
	}

	IEnumerator Step2 () {
		takingStep = true;
		//Debug.Log ("taking step " + (leftFootOnFloor ? "right" : "left"));
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		Vector3 target = controller.position;
		target.y = (leftFootOnFloor ? leftFootAnchor : rightFootAnchor).position.y;
		target += (leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * Vector3.Normalize (Vector3.ProjectOnPlane (controller.right, Vector3.up));
		target += forward * (stepDistance + centerOffset);

		Ray ray = new Ray ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, stepDistance, 1 << 10)) {//hits stairs
			walkingUpStairs = true;

			float stepHeight = hit.collider.bounds.extents.y;
			float stepDepth = hit.collider.bounds.extents.z * 2.0f;

			//1 step coefficients
			float a1 = -4.0f * stepHeight / (stepDepth * stepDepth);
			float b1 = 3.0f * stepHeight / stepDepth - stepDepth * a1 / 2.0f;
			//2 steps coefficients
			float a2 = -1.5f * stepHeight / (stepDepth * stepDepth);
			float b2 = 2.5f * stepHeight / stepDepth - stepDepth * a2;

			//si premiere marche, rectifier angle avant de determiner target
			
			float distance;
			new Plane (hit.transform.forward, hit.transform.position).Raycast (ray, out distance);
			target = ray.GetPoint (distance);
			target.y += stepHeight;
			Debug.Log (target);
			Debug.Break ();
		}

		if (!walkingUpStairs) {
			do {
				forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
				target = controller.position;
				target.y = (leftFootOnFloor ? leftFootAnchor : rightFootAnchor).position.y;
				target += (leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * Vector3.Normalize (Vector3.ProjectOnPlane (controller.right, Vector3.up));
				target += forward * (stepDistance + centerOffset);

				(leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position = Vector3.Lerp ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target,
					(speed * 2.0f * Time.deltaTime) /
					Vector3.Distance ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target));
				if ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position == target)
					break;
				yield return null;
			} while (true);
			//Debug.Log ("finished step " + (leftFootOnFloor ? "right" : "left"));
			leftFootOnFloor = !leftFootOnFloor;
		}
		takingStep = false;
	}

	IEnumerator RotateCameraForStairs () {
		return null;
	}
}
