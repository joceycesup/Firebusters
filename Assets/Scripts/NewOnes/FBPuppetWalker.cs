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

	public float maxStairsWalkAngle = 45.0f;
	public float turnRateCorrection = 360.0f;

	private Vector3 controllerCorrection = Vector3.zero;
	private bool needsCorrection = false;

	public float distanceOverFloor = 0.001f;
	public float floorDetectionDistance = 0.10f;



	private FBPuppetController controller;
	private FBMotionAnalyzer motion;

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
			transform.Translate (controller.transform.forward * (speed * Time.deltaTime * motion.walking), Space.World);

			if (needsCorrection) {
				float factor = (Time.deltaTime * speed * motion.walking) / controllerCorrection.magnitude;
				Vector3 correction = Vector3.Lerp (Vector3.zero, controllerCorrection, factor);
				transform.position = transform.position + correction;
				controllerCorrection -= correction;
				needsCorrection = factor < 1.0f;
				//Debug.Log (Time.time.ToString ("F3") + " : " + correction);
			}

			if (!controller.isMoving) {
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
		// step raycast needs to start from the foot that is in front of the next step
		Vector3 raycastStart = controller.fixedFoot.onStep ? controller.fixedFoot.target : foot.target;
		Vector3 forward = controller.transform.forward;
		Vector3 tmpTarget = controller.transform.position;

		tmpTarget.y = (controller.fixedFoot.onStep ? controller.fixedFoot : controller.movingFoot).target.y;
		tmpTarget += (controller.leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * controller.transform.right;
		tmpTarget += forward * (stepDistance + centerOffset);

		Ray ray = new Ray (footStart, forward); // first raycast needs to start from moving foot
		RaycastHit hit;

		// #########################################
		//
		// Check wall collision
		//
		// Layers reminder :
		// - 8 : Walkable
		// - 9 : VerticalObstacle
		// #########################################

		if (Physics.Raycast (ray, out hit, stepDistance, 1 << 9)) { // hits wall
			Debug.Log ("Hits Wall! : " + hit.transform.name);
			//Debug.Break ();
			Vector3 oldTarget = tmpTarget;
			tmpTarget = Vector3.ProjectOnPlane (tmpTarget - start, hit.normal);
			tmpTarget += Vector3.Project (hit.point - start, hit.normal);
			tmpTarget.y = start.y;
			ray = new Ray (start, tmpTarget);
			tmpTarget += start;
			controllerCorrection += (tmpTarget - oldTarget) / 2.0f;
			needsCorrection = true;
		}

		// #########################################
		//
		// Check for stairs in front of the foot
		//
		// #########################################

		bool hitsStep = Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits walkable -> step

		Debug.DrawRay (ray.origin, ray.direction, Color.red, 1.0f);
		if (hitsStep) {
			Debug.Log ("Hits Step! : " + hit.transform.name);
			Debug.Log ("Ray : " + ray);
			//Debug.Break ();
			Vector3 oldTarget = tmpTarget;
			float stepHeight = hit.collider.bounds.extents.y;

			if (!controller.fixedFoot.onStep) {
				// --------------------------------------------------------------------------
				// taking the first step, may need to correct angle
				// need to correct angle every single step since spiral staircases are supported
				// --------------------------------------------------------------------------
				if (Vector3.Angle (hit.transform.forward, forward) > maxStairsWalkAngle) {
					//StartCoroutine (RotateControllerOnStairs (Vector3.Cross (forward, hit.transform.forward).y * maxStairsWalkAngle + hit.transform.rotation.y));
				}
			}
			else {
				ray = new Ray (start + Vector3.up * stepHeight * 1.5f, tmpTarget - start);
				Physics.Raycast (ray, out hit, stepDistance, 1 << 8); // hits step above
			}

			GameObject stepFootLine = null;
			foreach (Transform child in hit.transform.parent) {
				if (child.gameObject.name == "StepFootLine") {
					stepFootLine = child.gameObject;
					break;
				}
			}

			Bounds bounds = stepFootLine.GetComponent<BoxCollider> ().bounds;
			float distance;
			new Plane (stepFootLine.transform.forward, stepFootLine.transform.position).Raycast (ray, out distance);
			tmpTarget = ray.GetPoint (distance);
			if ((tmpTarget - stepFootLine.transform.position).sqrMagnitude > bounds.extents.x * bounds.extents.x) {
				tmpTarget = stepFootLine.transform.position + Vector3.ClampMagnitude (tmpTarget - stepFootLine.transform.position, bounds.extents.x);
			}
			controllerCorrection += (tmpTarget - oldTarget) / 2.0f;
			needsCorrection = true;
			//tmpTarget.y = foot.transform.position.y + (walkingUpStairs ? 2.0f : 1.0f) * stepHeight;
			tmpTarget.y = foot.target.y + (walkingUpStairs ? 2.0f : 1.0f) * stepHeight;
		}

		ray = new Ray (tmpTarget, Vector3.down); // check if foot is on walkable
		Debug.DrawRay (ray.origin, ray.direction, Color.green, 1.0f);
		if (Physics.Raycast (ray, out hit, floorDetectionDistance, 1 << 8)) { // hits walkable
			foot.onStep = (hit.transform.gameObject.tag == "Step");
			tmpTarget.y = hit.point.y + distanceOverFloor;
		}
		else {
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			//#################################################################################
			foot.onStep = false;
			if (!hitsStep) { // regular walking, like... on a floor... and all that stuff
				Vector3 oldTarget = tmpTarget;
				Vector3 direction = tmpTarget - start;
				tmpTarget = Vector3.down * floorDetectionDistance;
				ray = new Ray (tmpTarget, direction);
				if (Physics.Raycast (ray, out hit, (start - tmpTarget).magnitude + 0.1f, 1 << 8)) { // hits walkable
					tmpTarget = hit.point;
					tmpTarget.y = oldTarget.y;
					controllerCorrection += (tmpTarget - oldTarget) / 2.0f;
					needsCorrection = true;
				}
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
			Debug.Log ("Step target : " + foot.target);
			//Debug.Break ();
			StartCoroutine (TakeStep (foot, controller.fixedFoot.onStep ? 2 : 1));
		}
		else {
			StartCoroutine (TakeStep (foot, 0));
		}
	}

	IEnumerator TakeStep (FBPuppetController.FootState foot, int steps) {
		controller.state = steps == 0 ? FBPuppetController.MovementState.Walking : FBPuppetController.MovementState.ClimbingStep;

		Vector3 startPos = foot.transform.position;
		Vector3 deltaPos = foot.target - foot.transform.position;
		float stepHeight = deltaPos.y;
		deltaPos.y = 0.0f;
		float distance = deltaPos.magnitude;

		float a, b;
		switch (steps) {
			case 1:
				a = -4.0f * stepHeight / (distance * distance);
				b = 3.0f * stepHeight / distance - distance * a * 0.5f;
				break;
			case 2:
				a = -3.0f * stepHeight / (distance * distance);
				b = 2.5f * stepHeight / distance - distance * a * 0.5f;
				break;
			default:
				a = -4.0f * horizontalStepHeight / (distance * distance);
				b = 2.0f * horizontalStepHeight / distance - distance * a * 0.5f;
				break;
		}

		do {
			distance = new Vector2 (foot.transform.position.x - foot.target.x, foot.transform.position.z - foot.target.z).magnitude;
			Vector3 newPos = Vector3.Lerp (foot.transform.position, foot.target,
				(speed * 2.0f * Time.deltaTime) /
				distance);
			distance = Horizontal (startPos, newPos).magnitude;
			newPos.y = startPos.y + a * distance * distance + b * distance;
			foot.transform.position = newPos;

			if (Mathf.Approximately (Horizontal (foot.transform.position, foot.target).magnitude, 0.0f)) {
				foot.transform.position = foot.target;
				break;
			}
			Debug.Log ((foot.transform.position - foot.target).y.ToString ("F5"));
			yield return null;
		} while (true);
		Debug.Log ("Finished step");
		controller.leftFootOnFloor = !controller.leftFootOnFloor;
		controller.state = FBPuppetController.MovementState.Idle;
	}//*/

	private Vector3 Horizontal (Vector3 v1, Vector3 v2) {
		Vector3 res = v1 - v2;
		res.y = 0.0f;
		return res;
	}
}
