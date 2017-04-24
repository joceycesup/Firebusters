using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBFootState {
	public Transform transform {
		get;
		private set;
	}
	public Vector3 target {
		get;
		set;
	}
	public bool onStep {//if not onStep, is onFloor
		get;
		set;
	}

	public FBFootState(Transform t) {
		transform = t;
		target = transform.position;
		onStep = false;
	}
}
