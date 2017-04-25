using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBPuppetWalker : MonoBehaviour {
	public float legsSpan = 0.6f;
	public float stepDistance = 1.5f;
	public float centerOffset = -1.0f;

	private bool takingStep = false;

	private bool walkingUpStairs = false;

	public float speed = 4.0f;
	public float horizontalStepHeight = 0.1f;

	public float maxStairsWalkAngle = 45.0f;
	public float turnRateCorrection = 360.0f;

	private Vector3 controllerCorrection = Vector3.zero;
	private bool needsCorrection = false;

	public float distanceOverFloor = 0.001f;
	public float floorDetectionDistance = 0.10f;



	public FBPuppetController toto;
	public FBMotionAnalyzer motion;

	// --------------------------------------------------------------------------
	// TO DO :
	// clipping is still possible on walls
	// walls are not handled correctly on stairs
	// NullReferenceException showing on stairs when raycasting doesn't hit step
	// check if distanceOverFloor is correctly used
	// --------------------------------------------------------------------------

	void Start () {
		Ray ray = new Ray (toto.transform.position, Vector3.down);
		RaycastHit hit;
		if (!Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << 8)) {
			Debug.LogError ("Character is not over walkable ground!!");
			Debug.Break ();
			return;
		}
		Transform foot = toto.leftFoot.transform;
		foot.position = new Vector3 (foot.position.x, hit.point.y + distanceOverFloor, foot.position.z);
		foot = toto.leftFoot.transform;
		foot.position = new Vector3 (foot.position.x, hit.point.y + distanceOverFloor, foot.position.z);
	}

	void Update () {
		Vector3 forward = toto.direction;
		//walking = Input.GetAxis ("Forward");
		//walking = Input.GetAxis ("VerticalL");

		if (motion.walking > 0.0f) {
			transform.Translate (forward * (speed * Time.deltaTime * motion.walking), Space.World);

			if (needsCorrection) {
				float factor = (Time.deltaTime * speed * motion.walking) / controllerCorrection.magnitude;
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
		// #########################################
		//
		// Initialization
		//
		// #########################################
		FBPuppetController.FootState foot = toto.movingFoot;
		Vector3 start = foot.transform.position;
		Vector3 forward = toto.transform.forward;
		Vector3 target = toto.transform.position;
		{
			target.y = toto.fixedFoot.target.y;
			target += (toto.leftFootOnFloor ? 1.0f : -1.0f) * legsSpan * toto.transform.right;
			target += forward * (stepDistance + centerOffset);
		}

		Ray ray = new Ray (start, target - start);
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
			Vector3 oldTarget = target;
			target = Vector3.ProjectOnPlane (target - start, hit.normal);
			target += Vector3.Project (hit.point - start, hit.normal);
			ray = new Ray (start, target);
			target += start;
			controllerCorrection += (target - oldTarget) / 2.0f;
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
			target.y = foot.transform.position.y + (walkingUpStairs ? 2.0f : 1.0f) * stepHeight;
		}

		bool oldWalkingUpStairs = walkingUpStairs;
		ray = new Ray (target, Vector3.down); // check if foot is on walkable
		Debug.DrawRay (ray.origin, ray.direction, Color.green, 1.0f);
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
			StartCoroutine (TakeStep (foot.transform, target, oldWalkingUpStairs ? 2 : 1));
		}
		else {
			StartCoroutine (TakeStep (foot.transform, target, 0));
		}
	}

	IEnumerator TakeStep (Transform foot, Vector3 target, int steps) {
		takingStep = true;

		float stepHeight = target.y - foot.position.y;
		Vector3 startPos = foot.position;
		Vector3 deltaPos = target - foot.position;
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
			distance = new Vector2 (foot.position.x - target.x, foot.position.z - target.z).magnitude;
			Vector3 newPos = Vector3.Lerp (foot.position, target,
				(speed * 2.0f * Time.deltaTime) /
				distance);
			deltaPos = startPos - foot.position;
			deltaPos.y = 0.0f;
			distance = deltaPos.magnitude;
			newPos.y = startPos.y + a * distance * distance + b * distance;
			foot.position = newPos;

			if (foot.position == target) // Vector3 comparison is already approximated
				break;
			yield return null;
		} while (true);
		toto.leftFootOnFloor = !toto.leftFootOnFloor;
		takingStep = false;
	}//*/
}
