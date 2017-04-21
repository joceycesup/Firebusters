using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMPMotionAnalyzer : MonoBehaviour {

	public enum State {
		Inactive,
		Walk,
		Aim,
		Strike
	}

	public State state = State.Walk;

	public TMPPhoneDataHandler sensor;
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
		walking = -1.0f;
	}

	void FixedUpdate () {
		switch (state) {
			case State.Inactive:
				break;
			case State.Aim:
				break;
			case State.Walk:
				UpdateValues ();
				break;
			case State.Strike:
				break;
		}
	}

	private void UpdateValues () {
		steering = Mathf.Sign (sensor.sensorAxis.z) * pitchFactor.Evaluate (Mathf.Abs (sensor.sensorAxis.z) / maxPitch);
		yRotation -= steeringTurnRate * steering * Time.fixedDeltaTime;

		walking = rollFactor.Evaluate (Mathf.Abs (sensor.sensorAxis.x) / maxRoll);

		controller.rotation = Quaternion.Euler (sensor.sensorAxis.x, yRotation, sensor.sensorAxis.z);

		dollWalker.walking = walking;
		//Debug.Log (walking);
	}
}
