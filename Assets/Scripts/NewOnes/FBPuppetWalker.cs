using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FBMotionAnalyzer))]
[RequireComponent (typeof (FBPuppetController))]
public class FBPuppetWalker : MonoBehaviour {
	public float legsSpan = 0.6f;
	public float stepDistance = 1.5f;
	public float centerOffset = -1.0f;

	public float speed = 4.0f;
	public float horizontalStepHeight = 0.1f;

	public float distanceOverFloor = 0.001f;
	public float floorDetectionDistance = 0.10f;

	private bool isMovingFoot = false;

	private FBPuppetController controller;
	private FBMotionAnalyzer motion;
	private bool horizontalCorrection = false;

	private Transform lastStep;
	private Vector3 lastStepExtents;

	// --------------------------------------------------------------------------
	// TO DO :
	// clipping is still possible on walls
	// walls are not handled correctly on stairs
	// NullReferenceException showing on stairs when raycasting doesn't hit step
	// check if distanceOverFloor is correctly used
	// --------------------------------------------------------------------------

	void Start () {
		controller = GetComponent<FBPuppetController> ();
		motion = GetComponent<FBMotionAnalyzer> ();
		Ray ray = new Ray (controller.transform.position, Vector3.down);
		RaycastHit hit;
		if (!Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << 8)) {
			Debug.LogError ("Character is not over walkable ground!!");
			Debug.Break ();
			return;
		}
		FBPuppetController.FootState foot = controller.leftFoot;
		foot.target = new Vector3 (foot.transform.position.x, hit.point.y + distanceOverFloor, foot.transform.position.z);
		foot = controller.rightFoot;
		foot.target = new Vector3 (foot.transform.position.x, hit.point.y + distanceOverFloor, foot.transform.position.z);
	}

	void Update () {
		//walking = Input.GetAxis ("Forward");
		//walking = Input.GetAxis ("VerticalL");

		if (motion.walking > 0.0f) {
			if (!isMovingFoot) {
				MoveFoot ();
			}
		}
	}

	void MoveFoot () {
		// #########################################
		//
		// Initialization
		//
		// #########################################
		FBPuppetController.FootState foot = controller.movingFoot;
		Vector3 footStart = foot.target;
		Vector3 forward = controller.transform.forward;
		Vector3 tmpTarget = controller.transform.position;

		Ray ray;
		RaycastHit hit;
		float raycastDistance = stepDistance;

		// #########################################
		//
		// Check wall collision
		//
		// Layers reminder :
		// - 8 : Walkable
		// - 9 : VerticalObstacle
		// #########################################
		tmpTarget.y = footStart.y;
		//tmpTarget += (controller.leftFootOnFloor ? legsSpan : -legsSpan) * Vector3.Cross (Vector3.up, forward);
		//tmpTarget += forward * (stepDistance + centerOffset);
		Debug.DrawLine (footStart, tmpTarget, Color.cyan, 10.0f);
		Debug.DrawRay (footStart, forward, Color.red, 10.0f);
		Debug.Log ("-------------------------------------------");
		Debug.Log ("	State : " + controller.state);
		Debug.Log ("	Other foot : " + controller.fixedFoot.onStep);

		if (controller.state != FBPuppetController.MovementState.ClimbingStep) {
			// we don't need to check for walls if the puppet is walking up stairs

			ray = new Ray (footStart, forward); // first raycast needs to start from moving foot
			int i = 2;
			while (Physics.Raycast (ray, out hit, stepDistance, 1 << 9) && i > 0) { // hits wall
				Debug.Log ("Hits Wall! : " + hit.transform.name);
				//Debug.Break ();
				tmpTarget = Vector3.ProjectOnPlane (tmpTarget - footStart, hit.normal); // vector to desired target from start along wall
				tmpTarget += Vector3.Project (hit.point - footStart, hit.normal); // vector to wall from start
				tmpTarget += hit.normal * distanceOverFloor;
				tmpTarget += footStart;
				tmpTarget.y = footStart.y;

				ray = new Ray (footStart, tmpTarget - footStart);
				i--;
			}
			if (i < 2) {
				correctionNeeded = true; //Hit wall, correction needed
				raycastDistance = (tmpTarget - footStart).magnitude;
			}
		}
		else {
			raycastDistance = float.MaxValue;
			if (controller.fixedFoot.onStep)
				ray = new Ray (lastStep.position + Vector3.up * (lastStepExtents.y + distanceOverFloor), lastStep.forward);
			else
				ray = new Ray (footStart, controller.targetDirection);
		}

		// #########################################
		//
		// Check for stairs in front of the foot
		//
		// #########################################

		bool hitsStep = Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits walkable -> step

		if (hitsStep) {
			lastStep = hit.transform;
			lastStepExtents = hit.collider.bounds.extents;
			controller.targetDirection = lastStep.forward;
			controller.useMaxAngleSpan = true;
			Debug.Log ((controller.leftFootOnFloor ? "Right" : "Left") + " foot hits Step! : " + hit.transform.name);

			if (controller.state != FBPuppetController.MovementState.ClimbingStep) {
				controller.state = FBPuppetController.MovementState.ClimbingStep;
				//controller.state = hitsStep ? FBPuppetController.MovementState.ClimbingStep : FBPuppetController.MovementState.Walking;
			}

			GameObject stepFootLine = null;
			foreach (Transform child in hit.transform.parent) {
				if (child.gameObject.name == "StepFootLine") {
					stepFootLine = child.gameObject;
					break;
				}
			}

			ray = new Ray (footStart, tmpTarget - footStart); // we need to reset ray position and direction to avoid getting a bad hit point
			Bounds bounds = stepFootLine.GetComponent<BoxCollider> ().bounds;
			float distance;
			new Plane (stepFootLine.transform.forward, stepFootLine.transform.position).Raycast (ray, out distance);
			tmpTarget = ray.GetPoint (distance);
			if ((tmpTarget - stepFootLine.transform.position).sqrMagnitude > bounds.extents.x * bounds.extents.x) {
				tmpTarget = stepFootLine.transform.position + Vector3.ClampMagnitude (tmpTarget - stepFootLine.transform.position, bounds.extents.x);
			}
			//tmpTarget.y = foot.transform.position.y + (walkingUpStairs ? 2.0f : 1.0f) * stepHeight;
			tmpTarget.y = lastStep.position.y + lastStepExtents.y + distanceOverFloor;
		}

		ray = new Ray (tmpTarget, Vector3.down); // check if foot is on walkable
		Debug.DrawRay (ray.origin, ray.direction, Color.green, 1.0f);
		if (Physics.Raycast (ray, out hit, floorDetectionDistance, 1 << 8)) { // hits walkable
			foot.onStep = (hit.transform.gameObject.tag == "Step");
			tmpTarget.y = hit.point.y + distanceOverFloor;

			// if puppet reached end of stairs, reset controller state
			if (!controller.fixedFoot.onStep && !foot.onStep && controller.state == FBPuppetController.MovementState.ClimbingStep) {
				controller.useMaxAngleSpan = false;
				controller.state = FBPuppetController.MovementState.Walking;
			}
		}
		else {
			Debug.LogWarning ("Foot over nothingness!");
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			foot.onStep = false;

			tmpTarget.y -= floorDetectionDistance;
			Vector3 deltaPos = footStart - tmpTarget;
			ray = new Ray (tmpTarget, deltaPos);
			if (Physics.Raycast (ray, out hit, deltaPos.magnitude, 1 << 8)) { // hits walkable
				tmpTarget = Vector3.ProjectOnPlane (-deltaPos, hit.normal); // vector to desired target from start along wall
				tmpTarget += Vector3.Project (footStart - hit.point, hit.normal); // vector to wall from start
				tmpTarget -= hit.normal * distanceOverFloor;
				tmpTarget += footStart;
				tmpTarget.y = footStart.y;
			}

			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
		}

		foot.target = tmpTarget;
		if (hitsStep) {
			StartCoroutine (TakeStep (foot, controller.fixedFoot.onStep ? 2 : 1));
		}
		else {
			StartCoroutine (TakeStep (foot, 0));
		}
	}

	IEnumerator TakeStep (FBPuppetController.FootState foot, int steps) {
		isMovingFoot = true;

		Vector3 startPos = foot.transform.position;
		Vector3 deltaPos = foot.target - foot.transform.position;
		float stepHeight = deltaPos.y;

		Vector3 controllerStartPos = controller.transform.position;
		Vector3 controllerTargetPos = controllerStartPos;
		if (horizontalCorrection)
			controllerTargetPos += Vector3.Project (deltaPos / 2.0f, controller.transform.forward);
		else
			controllerTargetPos += deltaPos / 2.0f;
		horizontalCorrection = false;

		deltaPos.y = 0.0f;
		float totalDistance = deltaPos.magnitude;
		float distance;

		float a, b;
		switch (steps) {
			case 1:
				a = -4.0f * stepHeight / (totalDistance * totalDistance);
				b = 3.0f * stepHeight / totalDistance - totalDistance * a * 0.5f;
				break;
			case 2:
				a = -3.0f * stepHeight / (totalDistance * totalDistance);
				b = 2.5f * stepHeight / totalDistance - totalDistance * a * 0.5f;
				break;
			default:
				a = -4.0f * horizontalStepHeight / (totalDistance * totalDistance);
				b = 2.0f * horizontalStepHeight / totalDistance - totalDistance * a * 0.5f;
				break;
		}

		float factor = 1.0f;

		//Debug.Log ((controller.leftFootOnFloor ? "left" : "right") + " foot : " + totalDistance);
		do {
			distance = new Vector2 (foot.transform.position.x - foot.target.x, foot.transform.position.z - foot.target.z).magnitude;
			factor = (speed * 2.0f * Time.deltaTime) / distance;
			Vector3 newPos = Vector3.Lerp (foot.transform.position, foot.target,
				factor);
			distance = Horizontal (startPos, newPos).magnitude;
			newPos.y = startPos.y + a * distance * distance + b * distance;
			foot.transform.position = newPos;

			//controller.transform.position = Vector3.Lerp (controllerStartPos, controllerTargetPos, distance / totalDistance);

			if (factor >= 1.0f) {
				foot.transform.position = foot.target;
				break;
			}
			yield return null;
		} while (true);
		Debug.Log ("Finished step");
		controller.leftFootOnFloor = !controller.leftFootOnFloor;
		isMovingFoot = false;
	}//*/

	private Vector3 Horizontal (Vector3 v1, Vector3 v2) {
		Vector3 res = v1 - v2;
		res.y = 0.0f;
		return res;
	}
}
