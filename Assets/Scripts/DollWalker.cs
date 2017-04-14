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

	private Vector3 controllerCorrection = Vector3.zero;
	private bool needsCorrection = false;

	void Start () {

	}

	void Update () {
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		//walking = Input.GetAxis ("Forward");

		if (walking > 0.0f) {
			transform.Translate (forward * (speed * Time.deltaTime * walking), Space.World);
			// --------------------------------------------------------------------------
			// in TakeStep coroutine, move by controllerCorrection
			// int Step#, add half the correction when hitting wall to controllerCorrection
			// --------------------------------------------------------------------------

			if (needsCorrection) {
				float factor = (Time.deltaTime * speed * walking) / controllerCorrection.magnitude;
				Vector3 correction = Vector3.Lerp (Vector3.zero, controllerCorrection, factor);
				transform.position = transform.position + correction;
				controllerCorrection -= correction;
				needsCorrection = factor < 1.0f;
				Debug.Log (Time.time.ToString ("F3") + " : " + correction);
			}

			if (!takingStep) {
				Step3 ();
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

	void Step2 () {
		if (takingStep)
			return;

		Vector3 start = (leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position;
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		Vector3 target = controller.position;
		//target.y = (leftFootOnFloor ? leftFootAnchor : rightFootAnchor).position.y;
		target += (leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * Vector3.Normalize (Vector3.ProjectOnPlane (controller.right, Vector3.up));
		target += forward * (stepDistance + centerOffset);

		Ray ray = new Ray (start, target - start);
		RaycastHit hit;

		// #########################################
		// Layers :
		// - 8  : Walkable
		// - 9  : StepFootLine
		// - 10 : VerticalObstacle
		// #########################################

		if (Physics.Raycast (ray, out hit, stepDistance, 1 << 10)) { // hits wall
			Vector3 tmp = target;
			target = Vector3.ProjectOnPlane (hit.normal, target - start);
			target += Vector3.Project (hit.point - start, hit.normal);
			ray = new Ray (start, target);
			target += start;
			controllerCorrection += (target - tmp) / 2.0f;
			needsCorrection = true;
		}

		bool hitsStep = Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits stairs

		if (hitsStep) {
			float stepHeight = hit.collider.bounds.extents.y;

			//depends on the step distance
			float stepDepth = hit.collider.bounds.extents.z * 2.0f;
			//------------------------------

			if (!walkingUpStairs) {
				// --------------------------------------------------------------------------
				// taking the first step, may need to correct angle
				// need to correct angle every single step since spiral staircases are supported
				// --------------------------------------------------------------------------
				if (Vector3.Angle (hit.transform.forward, forward) > maxStairsWalkAngle) {
					//StartCoroutine (RotateControllerOnStairs (Vector3.Cross (forward, hit.transform.forward).y * maxStairsWalkAngle + hit.transform.rotation.y));
				}
			}else {
				// --------------------------------------------------------------------------
				// need to detect the step above and not the step directly in front of the foot
				// --------------------------------------------------------------------------
				ray = new Ray (start + Vector3.up*stepHeight, target-start);
				Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits step above
			}

			GameObject stepFootLine = null;
			foreach (Transform child in hit.transform.parent) {
				if (child.gameObject.layer == 9) {
					stepFootLine = child.gameObject;
					break;
				}
			}
			if (!stepFootLine)
				Debug.Break ();

			Bounds bounds = stepFootLine.GetComponent<BoxCollider> ().bounds;
			float distance;
			new Plane (stepFootLine.transform.forward, stepFootLine.transform.position).Raycast (ray, out distance);
			target = ray.GetPoint (distance);
			if ((target - stepFootLine.transform.position).sqrMagnitude > bounds.extents.x * bounds.extents.x) {
				target = stepFootLine.transform.position + Vector3.ClampMagnitude (target - stepFootLine.transform.position, bounds.extents.x);
			}
			target.y = (leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position.y + (walkingUpStairs ? 2.0f : 1.0f) * stepHeight;



			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// keep walkingUpStairs value in avriable here
			// insert raycast for walkingUpStairs here
			// use old value in coroutine call
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------
			// --------------------------------------------------------------------------


			StartCoroutine (ClimbStep ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor), target, !walkingUpStairs));

		}
		else {
			StartCoroutine (TakeStep ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor), target));
		}
	}

	void Step3 () {
		if (takingStep)
			return;

		Vector3 start = (leftFootOnFloor ? rightFootAnchor : leftFootAnchor).position;
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		Vector3 target = controller.position;
		target.y = start.y;
		target += (leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * Vector3.Normalize (Vector3.ProjectOnPlane (controller.right, Vector3.up));
		target += forward * (stepDistance + centerOffset);

		Ray ray = new Ray (start, target - start);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, stepDistance, 1 << 10)) { // hits wall
			Vector3 tmp = target;
			target = Vector3.ProjectOnPlane (hit.normal, target - start);
			target += Vector3.Project (hit.point - start, hit.normal);
			ray = new Ray (start, target);
			target += start;
			controllerCorrection += (target - tmp) / 2.0f;
			needsCorrection = true;
		}
		StartCoroutine (TakeStep ((leftFootOnFloor ? rightFootAnchor : leftFootAnchor), target));
	}

	IEnumerator TakeStep (Transform foot, Vector3 target) {
		takingStep = true;
		do {
			foot.position = Vector3.Lerp (foot.position, target,
				(speed * 2.0f * Time.deltaTime) /
				Vector3.Distance (foot.position, target));
			if (foot.position == target) // Vector3 comparison is already approximated
				break;
			yield return null;
		} while (true);
		leftFootOnFloor = !leftFootOnFloor;
		takingStep = false;
	}

	IEnumerator ClimbStep (Transform foot, Vector3 target, bool oneStep) {
		Vector3 startPos = foot.position;
		float stepHeight = target.y - foot.position.y;
		Vector3 deltaPos = target - foot.position;
		deltaPos.y = 0.0f;
		float distance = deltaPos.magnitude;

		float a, b;
		if (oneStep) {
			// 1 step coefficients
			a = -4.0f * stepHeight / (distance * distance);
			b = 3.0f * stepHeight / distance - distance * a / 2.0f;
		}
		else {
			// 2 steps coefficients
			a = -1.5f * stepHeight / (distance * distance);
			b = 2.5f * stepHeight / distance - distance * a;
		}



		takingStep = true;
		do {
			Vector3 newPos = Vector3.Lerp (foot.position, target,
				(speed * 2.0f * Time.deltaTime) /
				Vector3.Distance (foot.position, target));
			deltaPos = startPos - foot.position;
			deltaPos.y = 0.0f;
			distance = deltaPos.magnitude;
			newPos.y = startPos.y + a * distance * distance + b * distance;
			foot.position = newPos;

			if (foot.position == target) // Vector3 comparison is already approximated
				break;
			yield return null;
		} while (true);



		// --------------------------------------------------------------------------
		// change stepDistance to something better
		// check if there is floor under foot
		// --------------------------------------------------------------------------
		//walkingUpStairs = Physics.Raycast (ray, out hit, stepDistance, 1 << 8);



		leftFootOnFloor = !leftFootOnFloor;
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
