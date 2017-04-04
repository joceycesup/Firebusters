using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasisChange : MonoBehaviour {
	public Transform reflexion;
	private Quaternion initialRotation;

	void Start () {
		initialRotation = Quaternion.Inverse(transform.rotation);
	}

	void Update () {
		reflexion.rotation = initialRotation * transform.rotation;
	}
}
