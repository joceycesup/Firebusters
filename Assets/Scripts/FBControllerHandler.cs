using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBControllerHandler : MonoBehaviour {

	//########## Definitive values ##########//

	public Transform controller;
	public Transform leftFootAnchor;
	public Transform rightFootAnchor;

	public float maxSpinAngle = 72.0f; // max angle between feet and controller, if exceeded feet spin to ajust

	private float relativeHeight; // vertical distance between anchors and controller

	//########## Changing values ##########//

	public Vector3 controllerTarget {
		get { return controllerTarget; }
		set {
			value.y = controllerTarget.y;
			controllerTarget = value;
		}
	}
	public Vector3 leftFootTarget {
		get { return leftFootTarget; }
		set {
			leftFootTarget = value;
			RecalculateControllerHeight ();
			RecalculateFeetDirection ();
		}
	}
	public Vector3 rightFootTarget {
		get { return rightFootTarget; }
		set {
			rightFootTarget = value;
			RecalculateControllerHeight ();
			RecalculateFeetDirection ();
		}
	}

	//########## Computed values ##########//

	private Quaternion controllerDirection;
	private Quaternion feetDirection;

	private void RecalculateControllerHeight () {
		controllerTarget = new Vector3 (controllerTarget.x, (leftFootTarget.y + rightFootTarget.y) / 2.0f + relativeHeight, controllerTarget.z);
	}

	private void RecalculateFeetDirection () {
		feetDirection = Quaternion.LookRotation (Vector3.Cross (rightFootTarget - leftFootTarget, Vector3.up));
	}

	private void RecalculateControllerDirection () {
		Vector3 forward = controller.forward;
		forward.y = 0.0f;
		controllerDirection = Quaternion.LookRotation (forward);
	}

	//########## Awake ##########//

	private void Awake () {
		if (controller && leftFootAnchor && rightFootAnchor) {
			controllerTarget = controller.position;
			leftFootTarget = leftFootAnchor.position;
			rightFootTarget = rightFootAnchor.position;
			relativeHeight = controllerTarget.y - (leftFootTarget.y + rightFootTarget.y) / 2.0f;
		}
		else {
			Debug.LogAssertion ("Controller and both anchors need to be specified");
			Debug.Break ();
		}
	}

	//########## Update ##########//

	private void Update () {

	}
}
