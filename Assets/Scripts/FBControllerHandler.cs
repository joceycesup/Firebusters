using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBCrossHandler : MonoBehaviour {

	//########## Computed values ##########//

	private FBPuppetController controller;
	private Quaternion feetDirection;

	private void RecalculateFeetDirection () {
		feetDirection = Quaternion.LookRotation (Vector3.Cross (controller.rightFoot.target - controller.leftFoot.target, Vector3.up));
	}

	//########## Update ##########//

	private void Update () {

	}
}
