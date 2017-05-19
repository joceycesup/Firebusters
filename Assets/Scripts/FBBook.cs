using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FBBook : MonoBehaviour {
	[Serializable]
	public struct BoneInfluence {
		public Transform bone;
		public float influence;
		[HideInInspector]
		public float initialRotation;
	}
	public bool positiveRotation = false;
	public BoneInfluence[] influences;
	private float initialRotation;
	private float rotation;

	void Start () {
		initialRotation = transform.localRotation.eulerAngles.z;
		for (int i = 0; i < influences.Length; ++i) {
			influences[i].influence /= 90.0f;
			influences[i].initialRotation = influences[i].bone.localRotation.eulerAngles.z;
		}
	}

	void FixedUpdate () {
		float lastRotation = rotation;
		rotation = (transform.localRotation.eulerAngles.z - initialRotation);
		if (positiveRotation) {
			if (rotation < 0.0f)
				rotation += 360.0f;
			if (rotation < 0.0f || rotation > 180.0f)
				transform.localRotation = Quaternion.Euler (0.0f, 0.0f, initialRotation);
		}
		else {
			if (rotation > 0.0f)
				transform.localRotation = Quaternion.Euler (0.0f, 0.0f, initialRotation);
		}
		if (!Mathf.Approximately (rotation, lastRotation)) {
			//Debug.Log (gameObject + " : " + rotation);
			foreach (BoneInfluence bi in influences) {
				bi.bone.localRotation = Quaternion.Euler (0.0f, 0.0f,
					rotation * bi.influence + bi.initialRotation
				);
			}
		}
	}
}
