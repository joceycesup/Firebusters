using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class OcclusionShaderManager : MonoBehaviour {
	public List<Transform> objs;
	List<Vector4> objects;
	public float Width = 1;
	//public Vector4 cam;
	private static OcclusionShaderManager _instance;
	// Use this for initialization
	void OnEnable () {
		_instance = this;
		objects = new List<Vector4> ();
		objects.Clear ();
		foreach (Transform trans in objs)
			objects.Add (trans.position);
	}

	public static OcclusionShaderManager Instance () {
		return _instance;
	}

	// Update is called once per frame
	void Update () {
		if (objs.Count > 0) {
			objects.Clear ();
			foreach (Transform trans in objs)
				objects.Add (trans.position);
			Shader.SetGlobalFloat ("_Distance", Width);
			Shader.SetGlobalVectorArray ("_Objects", objects);
			Shader.SetGlobalInt ("_ObjectsLength", objects.Count);
		}
	}
}
