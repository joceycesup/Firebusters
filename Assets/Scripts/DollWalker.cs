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

	private bool walkingUpStairs;

	public float speed = 4.0f;

	public float walking = 0.0f;

	public float maxStairsWalkAngle = 45.0f;
	public float turnRateCorrection = 360.0f;

	void Start () {

	}

	void Update () {
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		//walking = Input.GetAxis ("Forward");

		if (walking > 0.0f) {
			transform.Translate (forward * (speed * Time.deltaTime * walking), Space.World);
			if (!takingStep) {
				StartCoroutine ("Step");
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
			if ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position == target) // Vector3 comparison is already approximated
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

		Ray ray = new Ray ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target - (leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position);
		RaycastHit hit;
		bool hitsStep = Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits stairs

		if (hitsStep) {
			float stepHeight = hit.collider.bounds.extents.y;

			//depends on the step distance
			float stepDepth = hit.collider.bounds.extents.z * 2.0f;
			//------------------------------

			float a, b;
			if (walkingUpStairs) {
				// 2 steps coefficients
				a = -1.5f * stepHeight / (stepDepth * stepDepth);
				b = 2.5f * stepHeight / stepDepth - stepDepth * a;
			}
			else {
				// 1 step coefficients
				a = -4.0f * stepHeight / (stepDepth * stepDepth);
				b = 3.0f * stepHeight / stepDepth - stepDepth * a / 2.0f;

				// --------------------------------------------------------------------------
				// taking the first step, may need to correct angle
				// need to correct angle every single step since spiral staircases are supported
				// --------------------------------------------------------------------------
				if (Vector3.Angle (hit.transform.forward, forward) > maxStairsWalkAngle) {
					StartCoroutine (RotateControllerOnStairs (Vector3.Cross (forward, hit.transform.forward).y * maxStairsWalkAngle + hit.transform.rotation.y));
				}

				// --------------------------------------------------------------------------
				// need to correct distanceFactor as well to avoid errors on y axis
				// also needed if second step... step counter? check distance to target?
				// raycasting under foot is also an option since it also would correct height
				// --------------------------------------------------------------------------
			}

			float startY = (leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position.y;


			float distance;
			new Plane (hit.transform.forward, hit.transform.position).Raycast (ray, out distance);
			target = ray.GetPoint (distance);


			Debug.Log (target);
			Debug.Break ();



			do {
				(leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position = Vector3.Lerp ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target,
					(speed * 2.0f * Time.deltaTime) /
					Vector3.Distance ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target));
				if ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position == target) // Vector3 comparison is already approximated
					break;
				yield return null;
			} while (true);

			ray = new Ray ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, Vector3.down);
			// --------------------------------------------------------------------------
			// change stepDistance to something better
			// --------------------------------------------------------------------------
			walkingUpStairs = Physics.Raycast (ray, out hit, stepDistance, 1 << 8);


			//Debug.Log ("finished step " + (leftFootOnFloor ? "right" : "left"));
			leftFootOnFloor = !leftFootOnFloor;
		}
		else {
			do {
				(leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position = Vector3.Lerp ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target,
					(speed * 2.0f * Time.deltaTime) /
					Vector3.Distance ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position, target));
				if ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position == target) // Vector3 comparison is already approximated
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

	protected IEnumerator RotateControllerOnStairs (float targetRot) {
		float newAngle = float.MaxValue;

		while (true) {
			newAngle = Mathf.MoveTowardsAngle (transform.rotation.eulerAngles.z, targetRot, turnRateCorrection * Time.deltaTime);
			controller.rotation = Quaternion.Euler (0f, 0f, newAngle);
			if (Mathf.Abs (Mathf.DeltaAngle (targetRot, newAngle)) > 1f) {
				yield return null;
			}
			else {
				break;
			}
		}
	}
}
