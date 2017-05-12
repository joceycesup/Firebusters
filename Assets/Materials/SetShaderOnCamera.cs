using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetShaderOnCamera : MonoBehaviour {
	private Camera cam;
	public Shader shader;
	public string renderType;

	void OnEnable () {
		if (shader) {
			if (renderType == null)
				renderType = "";
			GetComponent<Camera> ().SetReplacementShader (shader, renderType.Length <= 0 ? "RenderType" : renderType);
		}
	}
	void OnDisable () {
		GetComponent<Camera> ().ResetReplacementShader ();
	}
}
