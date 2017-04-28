using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (FBPhoneDataHandler))]
public class TMPMotionAnalyzer : MonoBehaviour {

	public enum State {
		Inactive,
		Walk,
		Aim,
		Strike
	}

	public State state = State.Walk;

	private FBPhoneDataHandler sensor;
	public bool usePhoneDataHandler = true;
	public DollWalker dollWalker;

	public Transform controller;

	public float steeringTurnRate = 90.0f;

	private float yRotation = 0.0f;

	//------------------------------------------------------------------------------------------------
	// ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ##
	//------------------------------------------------------------------------------------------------
	// z : pitch : steering
	// x : roll  : walking
	//------------------------------------------------------------------------------------------------
	// ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ## ##
	//------------------------------------------------------------------------------------------------

	public float walking {
		get;
		private set;
	}
	public float steering {
		get;
		private set;
	}
	public float maxRoll = 30.0f;
	public AnimationCurve rollFactor = AnimationCurve.EaseInOut (0.2f, 0.0f, 1.0f, 1.0f);
	public float maxPitch = 30.0f;
	public AnimationCurve pitchFactor = AnimationCurve.EaseInOut (0.2f, 0.0f, 1.0f, 1.0f);

	void Start () {
		sensor = gameObject.GetComponent<FBPhoneDataHandler> ();
		walking = -1.0f;
	}

	void FixedUpdate () {
		switch (state) {
			case State.Inactive:
				break;
			case State.Aim:
				break;
			case State.Walk:
				UpdateWalkValues ();
				break;
			case State.Strike:
				break;
		}
	}

	private void UpdateWalkValues () {
		if (usePhoneDataHandler) {
			steering = Mathf.Sign (sensor.orientation.z) * pitchFactor.Evaluate (Mathf.Abs (sensor.orientation.z) / maxPitch);

			walking = rollFactor.Evaluate (sensor.orientation.x / maxRoll);
			//Debug.Log (walking);
		}
		else {
			float horizontal = Input.GetAxis ("HorizontalL");
			steering = Mathf.Sign (horizontal) * pitchFactor.Evaluate (Mathf.Abs (horizontal));

			walking = rollFactor.Evaluate (Input.GetAxis ("VerticalL"));
		}

		yRotation += steeringTurnRate * steering * Time.fixedDeltaTime;
		controller.rotation = Quaternion.Euler (sensor.orientation.x, yRotation, sensor.orientation.z);

		dollWalker.walking = walking;
	}
}
