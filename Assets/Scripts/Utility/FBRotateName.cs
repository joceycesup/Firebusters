using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBRotateName : MonoBehaviour {
	public Transform target;
	public float distance = 4.0f;

	void Start () {
		transform.position = target.position + (transform.position - target.position).normalized * distance;
		transform.rotation = Quaternion.LookRotation (target.position - transform.position, target.up) * Quaternion.Euler (-90.0f, 0.0f, 0.0f);
	}
}
