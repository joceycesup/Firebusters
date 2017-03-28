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
	private int analyzedValues = 8;
	private int valuesOffset = 0;
	private float[] values;
	private float[] slopes;
	private float[] wave_amplitudes;
	private float[] wave_periods;
	private int waves = 8;
	private int wavesOffset = 0;

	private float lastSpikeTime = 0.0f;
	private float lastDelta = 0.0f;

	//------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------
	private float delayThreshold = 1.0f;
	private float amplitudeThreshold = 0.2f;
	//------------------------------------------------------------------------------------------------
	//------------------------------------------------------------------------------------------------

	public Transform controller;

	void Start () {
		Debug.Log (Time.fixedDeltaTime);
		size = Mathf.ClosestPowerOfTwo (size);
		values = new float[size];
		slopes = new float[size];
		wave_amplitudes = new float[waves];
		wave_periods = new float[waves];
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

	void FixedUpdate () {
		if (controller)
			values[valuesOffset] = controller.localRotation.eulerAngles.z;
		else
			values[valuesOffset] = Input.GetAxis ("HorizontalL");
		int analyzeOffset = valuesOffset - analyzedValues;
		if (analyzeOffset < 0)
			analyzeOffset += size;
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		float delta = 0.0f;
		float absDelta = 0.0f;
		for (int i = 0; i < analyzedValues; ++i, ++analyzeOffset) {
			if (analyzeOffset + 1 >= size) {
				analyzeOffset = 0;
				delta += (values[0] - values[size - 1]);
				absDelta += Mathf.Abs (values[0] - values[size - 1]);
			}
			else {
				delta += (values[analyzeOffset + 1] - values[analyzeOffset]);
				absDelta += Mathf.Abs (values[analyzeOffset + 1] - values[analyzeOffset]);
			}
		}
		delta /= analyzedValues;
		absDelta /= analyzedValues;
		if (delta * lastDelta < 0.0f) {/*
			if ((Time.time - lastSpikeTime) >= delayThreshold && absDelta < amplitudeThreshold)
				lastSpikeTime = Time.time;//*/
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
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		if (ren) {
			drawGraph (ref values, Color.white);
			drawGraph (ref slopes, Color.yellow);
			tex.SetPixel (valuesOffset, size / 2, Color.red);
			tex.Apply (false);
		}
		//------------------------------------------------------------------------------------------------
		valuesOffset++;
		if (valuesOffset >= size)
			valuesOffset = 0;
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
		drawLine (valuesOffset, 0, valuesOffset, size, Color.black);
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
