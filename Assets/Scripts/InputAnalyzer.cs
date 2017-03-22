using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAnalyzer : MonoBehaviour {
	private Texture2D tex;
	private Renderer ren;
	private int size;
	private Color[] colors = new Color[3];
	
	void Start () {
		ren = GetComponent<MeshRenderer> ();
		tex = ren.material.mainTexture as Texture2D;
		size = tex.width;
		tex.SetPixels (colors, 0);
	}
	
	void FixedUpdate () {
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
		}
		// actually apply all SetPixels, don't recalculate mip levels
		tex.Apply (false);
		ren.material.mainTexture = tex;
	}
}
