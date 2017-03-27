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
	public int analyzedValues = 8;
	private int valuesOffset = 0;
	private float[] values;
	private float[] wave_amplitudes;
	private float[] wave_periods;
	private int waves = 8;
	private int wavesOffset = 0;

	private float lastSpikeTime = 0.0f;
	private float lastDelta = 0.0f;

	void Start () {
		size = Mathf.ClosestPowerOfTwo (size);
		values = new float[size];
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
		values[valuesOffset] = Input.GetAxis ("HorizontalL");
		int analyzeOffset = valuesOffset - analyzedValues;
		if (analyzeOffset < 0)
			analyzeOffset += size;
		float delta = 0.0f;
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//make a lastDeltas
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------
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

			int value = size / 2 - (int) (values[valuesOffset] * size / 2);
			if (valuesOffset > 0) {
				int lastValue = size / 2 - (int) (values[valuesOffset - 1] * size / 2);
				drawLine (valuesOffset - 1, lastValue, valuesOffset, value, Color.white);
			}
			else {
				tex.SetPixel (valuesOffset, value, Color.white);
			}
			tex.SetPixel (valuesOffset, size / 2, Color.red);
			tex.Apply (false);
		}
		valuesOffset++;
		if (valuesOffset >= size)
			valuesOffset = 0;
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
