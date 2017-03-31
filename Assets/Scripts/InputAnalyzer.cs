using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputAnalyzer : MonoBehaviour {
	private Texture2D tex;
	private Renderer ren;
	public Text output;
	[Range (128, 1024)]
	private int size = 512;
	private int valuesOffset = 0;
	private float[] values;

	private float lastDelta = 0.0f;

	//------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------
	private float lastDelayFactor = 0.0f;

	private float lastAmplitudeFactor = 0.0f;
	private float currentAmplitudeFactor = 0.0f;

	private float lastValue = 0.0f;
	private float currentValue = 0.0f;

	private float lastSpikeTime = 0.0f;

	public float amplitudeThreshold = 0.2f;
	public AnimationCurve amplitudeThresholdFactor;
	public float delayThreshold = 1.0f;
	public AnimationCurve delayThresholdFactor;

	private float amplitudeDecreaseCoeff;
	//------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------

	public Transform controller;

	public float walking {
		get;
		private set;
	}
	public float slopeDirection = 0.0f;//if 0, any direction else use sign (-1 ; +1)

	void Start () {
		walking = -1.0f;
		amplitudeDecreaseCoeff = -amplitudeThreshold / delayThreshold;

		size = Mathf.ClosestPowerOfTwo (size);
		values = new float[size];
		ren = GetComponent<MeshRenderer> ();
		if (ren) {
			tex = new Texture2D (size, size, TextureFormat.RGB24, false);
			//tex.SetPixels (colors, 0);
			//Debug.Log(tex.format);
			Color[] cols = tex.GetPixels (0);
			for (int i = 0; i < cols.Length; ++i) {
				cols[i] = Color.black;
			}
			for (int x = 0; x < size; ++x) {
				tex.SetPixel (x, size / 2, Color.red);
			}
			ren.material.mainTexture = tex;
		}
	}
	//*
	void FixedUpdate () {
		float value = 0.0f;
		if (controller)
			value = controller.localRotation.eulerAngles.z;
		else
			value = Input.GetAxis ("HorizontalL");
		values[valuesOffset] = value;
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
					//walking = amplitudeFactor;
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




			if (value * lastValue < 0.0f) {// user still on same side
				if (amplitudeFactor > 0.0f) {
					amplitudeFactor = Mathf.Max (lastAmplitudeFactor += Time.fixedDeltaTime * amplitudeDecreaseCoeff, currentAmplitudeFactor);
				}
				else {
					amplitudeFactor = lastAmplitudeFactor;
				}
			}
			else {
				amplitudeFactor = lastAmplitudeFactor;
			}

			Debug.Log (amplitudeFactor);
			walking = delayFactor * amplitudeFactor;

			lastDelayFactor = delayFactor;
		}
		//----- end waiting next spike -----

		output.text = walking.ToString ();

		//------------------------------------------------------------------------------------------------
		if (ren) {
			clearLine ();
			drawGraph (ref values, Color.white);
			drawGraph (lastWalking * 0.9f, walking * 0.9f, Color.green);
			tex.SetPixel (valuesOffset, size / 2, Color.gray);
			tex.Apply (false);
		}
		//------------------------------------------------------------------------------------------------
		valuesOffset++;
		if (valuesOffset >= size)
			valuesOffset = 0;
	}/*/
	void FixedUpdate () {
		if (controller)
			values[valuesOffset] = controller.localRotation.eulerAngles.z;
		else
			values[valuesOffset] = Input.GetAxis ("HorizontalL");
		int analyzeOffset = valuesOffset - analyzedValues;
		if (analyzeOffset < 0)
			analyzeOffset += size;
		//------------------------------------------------------------------------------------------------
		float delta = 0.0f;
		for (int i = 0; i < analyzedValues; ++i, ++analyzeOffset) {
			if (analyzeOffset + 1 >= size) {
				analyzeOffset = 0;
				delta += (values[0] - values[size - 1]);
			}
			else {
				delta += (values[analyzeOffset + 1] - values[analyzeOffset]);
			}
		}
		delta /= analyzedValues;
		slopes[valuesOffset] = delta;
		if (delta * lastDelta < 0.0f) {
			output.text = (Time.time - lastSpikeTime).ToString ();
			wave_periods[wavesOffset] = Time.time - lastSpikeTime;
			lastSpikeTime = Time.time;

			wave_amplitudes[wavesOffset] = values[valuesOffset];

			wavesOffset++;
			if (wavesOffset >= waves)
				wavesOffset = 0;
		}
		lastDelta = delta;
		//------------------------------------------------------------------------------------------------
		if (ren) {
			clearLine ();
			drawGraph (ref values, Color.white);
			drawGraph (ref slopes, Color.yellow);
			tex.SetPixel (valuesOffset, size / 2, Color.red);
			tex.Apply (false);
		}
		//------------------------------------------------------------------------------------------------
		valuesOffset++;
		if (valuesOffset >= size)
			valuesOffset = 0;
	}//*/

	private void clearLine () {
		drawLine (valuesOffset, 0, valuesOffset, size, Color.black);
	}

	private void drawGraph (ref float[] array, Color color) {
		/*
		Color[] colors = new Color[3];
		colors[0] = Color.red;
		colors[1] = Color.green;
		colors[2] = Color.blue;
		int mipCount = Mathf.Min (3, tex.mipmapCount);
		// tint each mip level
		for (int mip = 0; mip < mipCount; ++mip) {
			Color[] cols = tex.GetPixels (mip);
			for (int i = 0; i < cols.Length; ++i) {
				cols[i] = colors[mip];
			}
			tex.SetPixels (cols, mip);
		}/*/
		if (valuesOffset < size - 1)
			drawLine (valuesOffset + 1, 0, valuesOffset + 1, size, Color.green);

		int value = size / 2 - (int) (array[valuesOffset] * size / 2);
		if (valuesOffset > 0) {
			int lastValue = size / 2 - (int) (array[valuesOffset - 1] * size / 2);
			drawLine (valuesOffset - 1, lastValue, valuesOffset, value, color);
		}
		else {
			tex.SetPixel (valuesOffset, value, color);
		}
	}

	private void drawGraph (float f0, float f1, Color color) {
		int value = size / 2 - (int) (f1 * size / 2);
		if (valuesOffset > 0) {
			int lastValue = size / 2 - (int) (f0 * size / 2);
			drawLine (valuesOffset - 1, lastValue, valuesOffset, value, color);
		}
		else {
			tex.SetPixel (valuesOffset, value, color);
		}
	}

	private void drawLine (int x0, int y0, int x1, int y1, Color color) {
		float dx = x1 - x0;
		float dy = y1 - y0;
		if (dy == 0.0f) {
			tex.SetPixel (x1, y1, color);
		}
		else {
			float derr = Mathf.Abs (dx / dy);
			float err = derr - 0.5f;
			int x = x0;
			for (int y = y0; y != y1; y += (dy < 0) ? -1 : 1) {
				tex.SetPixel (x, y, color);
				err = err + derr;
				if (err >= 0.5f) {
					x++;
					err -= 1.0f;
				}
			}
		}
	}
}
