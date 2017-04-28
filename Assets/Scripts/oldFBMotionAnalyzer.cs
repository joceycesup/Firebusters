using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldFBMotionAnalyzer : MonoBehaviour {

	public enum State {
		Inactive,
		Walk,
		Aim,
		Strike
	}

	public State state = State.Inactive;

	//------------------------------------------------------------------------------------------------

	public FBPhoneDataHandler sensor;

	//------------------------------------------------------------------------------------------------
	public DollWalker dollWalker;
	public float walking {
		get;
		private set;
	}
	//------------------------------------------------------------------------------------------------

	void Start () {
	}
	
	void FixedUpdate () {
		switch (state) {
			case State.Inactive:
				break;
			case State.Aim:
				break;
			case State.Walk: {
					float value = 0.0f;
					if (sensor) {
						value = sensor.orientation.z;
						if (value > 180.0f)
							value -= 360.0f;
					}
					else
						value = Input.GetAxis ("HorizontalL");
					//walkingOscillation.AnalyzeOscillation (value);

					//dollWalker.walking = walkingOscillation.factor;
				}
				break;
			case State.Strike:
				break;
		}
	}
}
