using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FBMotionAnalyzer))]
[RequireComponent (typeof (FBPuppetController))]
public class FBCrossHandler : MonoBehaviour {
	private FBPuppetController controller;
	private FBMotionAnalyzer motion;

	public Transform cross;

	private Transform localAccelerationTip;

	void Awake () {
		localAccelerationTip = new GameObject ().transform;
		localAccelerationTip.parent = cross;
		controller = GetComponent<FBPuppetController> ();
		motion = GetComponent<FBMotionAnalyzer> ();
	}

	private void Update () {
		cross.localRotation = Quaternion.Euler (motion.rotation.x, 0.0f, motion.rotation.z);
		localAccelerationTip.localPosition = Vector3.up * motion.acceleration.y;
		Debug.DrawLine (transform.position, localAccelerationTip.position, Color.red);
	}
}
