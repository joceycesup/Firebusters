using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBOscillationAnalyzer {
	public float factor {
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
	
	public FBOscillationAnalyzer () {
		factor = -1.0f;
		amplitudeDecreaseCoeff = -amplitudeThreshold / delayThreshold;
	}

	void AnalyzeOscillation (float value) {
		float lastFactor = factor;

		float amplitudeFactor = amplitudeThresholdFactor.Evaluate (Mathf.Abs (value) / amplitudeThreshold);
		float delayFactor = delayThresholdFactor.Evaluate ((Time.time - lastSpikeTime) / delayThreshold);

		if (delayFactor <= 0.0f) {
			factor = -1.0f;
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
			factor = delayFactor * amplitudeFactor;

			lastDelayFactor = delayFactor;
		}
		//----- end waiting next spike -----
	}
}
