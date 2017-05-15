using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MoveTextureOffset : MonoBehaviour {
	private MeshRenderer mr;
	private Material mat;
	public Vector2 offsets = new Vector2 ();
	private List<float> coucou;

	void OnEnable () {
		//Debug.Log ("Enable MoveTextureOffset on " + gameObject);
		mr = GetComponent<MeshRenderer> ();
		if (mat == null) {
			mat = new Material (mr.sharedMaterial);
			mr.sharedMaterial = mat;
		}
	}

	void Update () {
		mat.mainTextureOffset = offsets;
	}
}
