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

	public float maxStairsWalkAngle = 45.0f;
	public float turnRateCorrection = 360.0f;

	private Vector3 controllerCorrection = Vector3.zero;
	private bool needsCorrection = false;

	public float distanceOverFloor = 0.001f;
	public float floorDetectionDistance = 0.10f;


	// --------------------------------------------------------------------------
	// TO DO :
	// clipping is still possible on walls
	// walls are not handled correctly on stairs
	// NullReferenceException showing on stairs when raycasting doesn't hit step
	// check if distanceOverFloor is correctly used
	// --------------------------------------------------------------------------



	void Start () {
		//Time.timeScale = 0.5f;
		Ray ray = new Ray (controller.position, Vector3.down);
		RaycastHit hit;
		if (!Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << 8)) {
			Debug.LogError ("Character is not over walkable ground!!");
			Debug.Break ();
			return;
		}
		leftFootAnchor.position = new Vector3 (leftFootAnchor.position.x, hit.point.y + distanceOverFloor, leftFootAnchor.position.z);
		rightFootAnchor.position = new Vector3 (rightFootAnchor.position.x, hit.point.y + distanceOverFloor, rightFootAnchor.position.z);
	}

	void Update () {
		Vector3 forward = Vector3.Normalize (Vector3.ProjectOnPlane (controller.forward, Vector3.up));
		//walking = Input.GetAxis ("Forward");
		walking = Input.GetAxis ("VerticalL");

		if (walking > 0.0f) {
			transform.Translate (forward * (speed * Time.deltaTime * walking), Space.World);

			if (needsCorrection) {
				float factor = (Time.deltaTime * speed * walking) / controllerCorrection.magnitude;
				Vector3 correction = Vector3.Lerp (Vector3.zero, controllerCorrection, factor);
				transform.position = transform.position + correction;
				controllerCorrection -= correction;
				needsCorrection = factor < 1.0f;
				//Debug.Log (Time.time.ToString ("F3") + " : " + correction);
			}

			if (!takingStep) {
				Step ();
			}
		}
	}

	void Step () {
		Transform foot = (leftFootOnFloor ? rightFootAnchor : leftFootAnchor);
		Vector3 start = foot.position;
		Vector3 forward;
		Vector3 target = controller.position;
		{
			Vector3 tmp = controller.forward;
			tmp.y = 0.0f;
			forward = Vector3.Normalize (tmp);
			tmp = controller.right;
			tmp.y = 0.0f;
			target.y = foot.position.y;
			target += (leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * Vector3.Normalize (tmp);
			target += forward * (stepDistance + centerOffset);
		}

		Ray ray = new Ray (start, target - start);
		RaycastHit hit;

		// #########################################
		// Layers :
		// - 8  : Walkable
		// - 9  : StepFootLine
		// - 10 : VerticalObstacle
		// #########################################

		if (Physics.Raycast (ray, out hit, stepDistance, 1 << 10)) { // hits wall
			Debug.Log ("Hits Wall! : " + hit.transform.name);
			//Debug.Break ();
			Vector3 oldTarget = target;
			target = Vector3.ProjectOnPlane (target - start, hit.normal);
			target += Vector3.Project (hit.point - start, hit.normal);
			ray = new Ray (start, target);
			target += start;
			controllerCorrection += (target - oldTarget) / 2.0f;
			needsCorrection = true;
		}

		bool hitsStep = Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits walkable -> step

		if (hitsStep) {
			Debug.Log ("Hits Step! : " + hit.transform.name);
			//Debug.Break ();
			Vector3 oldTarget = target;
			float stepHeight = hit.collider.bounds.extents.y;

			if (!walkingUpStairs) {
				// --------------------------------------------------------------------------
				// taking the first step, may need to correct angle
				// need to correct angle every single step since spiral staircases are supported
				// --------------------------------------------------------------------------
				if (Vector3.Angle (hit.transform.forward, forward) > maxStairsWalkAngle) {
					//StartCoroutine (RotateControllerOnStairs (Vector3.Cross (forward, hit.transform.forward).y * maxStairsWalkAngle + hit.transform.rotation.y));
				}
			}
			else {
				ray = new Ray (start + Vector3.up * stepHeight * 1.5f, target - start);
				Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits step above
			}

			GameObject stepFootLine = null;
			foreach (Transform child in hit.transform.parent) {
				if (child.gameObject.name == "StepFootLine") {
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
			controllerCorrection += (target - oldTarget) / 2.0f;
			needsCorrection = true;
			target.y = foot.position.y + (walkingUpStairs ? 2.0f : 1.0f) * stepHeight;
		}

		bool oldWalkingUpStairs = walkingUpStairs;
		ray = new Ray (target, Vector3.down); // check if foot is on walkable
		if (Physics.Raycast (ray, out hit, floorDetectionDistance, 1 << 8)) { // hits walkable
			walkingUpStairs = (hit.transform.gameObject.tag == "Step");
		}
		else {
			walkingUpStairs = false;
			if (!hitsStep) { // regular walking, like... on a floor... and all that stuff
				Vector3 oldTarget = target;
				Vector3 direction = target - start;
				target = Vector3.down * floorDetectionDistance;
				ray = new Ray (target, direction);
				if (Physics.Raycast (ray, out hit, (start - target).magnitude + 0.1f, 1 << 8)) { // hits walkable
					target = hit.point;
					target.y = oldTarget.y;
					controllerCorrection += (target - oldTarget) / 2.0f;
					needsCorrection = true;
				}
			}
		}

		if (hitsStep) {
			Debug.Log ("Step target : " + target);
			//Debug.Break ();
			StartCoroutine (ClimbStep (foot, target, !oldWalkingUpStairs));
		}
		else {
			StartCoroutine (TakeStep (foot, target));
		}
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
			b = 3.0f * stepHeight / distance - distance * a * 0.5f;
		}
		else {
			// 2 steps coefficients
			a = -3.0f * stepHeight / (distance * distance);
			b = 2.5f * stepHeight / distance - distance * a * 0.5f;
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

		leftFootOnFloor = !leftFootOnFloor;
		takingStep = false;
		Debug.Log ("Reached next step");
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
