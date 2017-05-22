﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBMailBox : FBHittable {
	public float throwForce = 200.0f;
	public float torqueForce = 200.0f;

	protected override void Hit (Collision collision) {
		base.Hit (collision);
	}

	protected override void HitByAxe (Collision collision) {
		base.HitByAxe (collision);
		//Debug.Log (collision.relativeVelocity.magnitude);
		if (transform.childCount > 0) {
			Transform drawer = transform.GetChild (Random.Range (0, transform.childCount));
			drawer.parent = null;
			Rigidbody rb = drawer.gameObject.AddComponent<Rigidbody> ();
			rb.constraints = RigidbodyConstraints.FreezePositionY;
			drawer.gameObject.AddComponent<FBDrawer> ().torqueForce = torqueForce;
			StartCoroutine (ThrowDrawer (rb));
		}
	}

	private IEnumerator ThrowDrawer (Rigidbody rb) {
		rb.AddForce (transform.forward * -throwForce);
		yield return null;
	}
}
