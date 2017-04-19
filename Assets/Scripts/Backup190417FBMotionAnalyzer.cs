using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup190417FBMotionAnalyzer : MonoBehaviour {

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
	public float slopeDirection = 0.0f;//if 0, any direction else use sign (-1 ; +1)
	private float lastDelayFactor = 0.0f;

	private float lastAmplitudeFactor = 0.0f;
	private float currentAmplitudeFactor = 0.0f;

	private float lastValue = 0.0f;
	private float currentValue = 0.0f;

	public float lastSpikeTime = 0.0f;

	public float amplitudeThreshold = 0.2f;
	public AnimationCurve amplitudeThresholdFactor;
	public float delayThreshold = 1.0f;
	public AnimationCurve delayThresholdFactor;

	private float amplitudeDecreaseCoeff;
	//------------------------------------------------------------------------------------------------

	void Start () {
		walking = -1.0f;
		amplitudeDecreaseCoeff = -amplitudeThreshold / delayThreshold;
	}

	void FixedUpdate () {
		switch (state) {
			case State.Inactive:
				break;
			case State.Aim:
				break;
			case State.Walk:
				AnalyzeOscillation ();
				break;
			case State.Strike:
				break;
		}
	}

	void AnalyzeOscillation () {
		float value = 0.0f;
		if (sensor) {
			value = sensor.sensorAxis.z;
			if (value > 180.0f)
				value -= 360.0f;
		}
		else
			value = Input.GetAxis ("HorizontalL");
		float lastWalking = walking;

		float amplitudeFactor = amplitudeThresholdFactor.Evaluate (Mathf.Abs (value) / amplitudeThreshold);
		float delayFactor = delayThresholdFactor.Evaluate ((Time.time - lastSpikeTime) / delayThreshold);

		if (delayFactor <= 0.0f) {
			walking = -1.0f;
			slopeDirection = 0.0f;
			lastValue = 0.0f;
			lastAmplitudeFactor = 0.0f;
		}
		//----- waiting significant amplitude -----
		if (slopeDirection == 0.0f) {
			if (amplitudeFactor > 0.0f) {
				if (Mathf.Abs (value) >= Mathf.Abs (lastValue)) {
					lastValue = value;
					lastAmplitudeFactor = amplitudeFactor;
					currentAmplitudeFactor = 0.0f;
					lastSpikeTime = Time.time;
					lastDelayFactor = 1.0f;
				}
				if (value * lastValue < 0.0f) {// if user changes side, wait for the next spike
					currentValue = value;
					slopeDirection = value > 0.0f ? 1.0f : -1.0f;
				}
			}
		}
		//----- waiting next spike -----
		else {
			if (value * lastValue < 0.0f) {// user still on same side
				if (slopeDirection > 0.0f ? (value > currentValue) : (value < currentValue)) {// value si greater than the recorded one
					currentValue = value;
					currentAmplitudeFactor = amplitudeFactor;
					if (delayFactor < lastDelayFactor) {// changing side is forced
						lastValue = currentValue;
						currentValue = 0.0f;
						lastAmplitudeFactor = currentAmplitudeFactor;

						lastDelayFactor = 1.0f;
						slopeDirection = -slopeDirection;
					}
					lastSpikeTime = Time.time;
				}
			}
			else {// user changed side
				lastValue = currentValue;
				currentValue = 0.0f;
				slopeDirection = -slopeDirection;
				lastAmplitudeFactor = currentAmplitudeFactor;
			}

			if (value * lastValue < 0.0f && amplitudeFactor > 0.0f) {// user still on same side
				amplitudeFactor = Mathf.Max (lastAmplitudeFactor += Time.fixedDeltaTime * amplitudeDecreaseCoeff, currentAmplitudeFactor);
			}
			else {
				amplitudeFactor = lastAmplitudeFactor;
			}

			//Debug.Log (amplitudeFactor);
			walking = delayFactor * amplitudeFactor;

			lastDelayFactor = delayFactor;
		}
		//----- end waiting next spike -----

		dollWalker.walking = walking;
	}
}
