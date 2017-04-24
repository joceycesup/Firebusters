using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FBMotionAnalyzer))]
public class FBPuppetController : MonoBehaviour {
	private FBFootState[] feet = new FBFootState[2];
	public FBFootState leftFoot {
		get { return feet[0]; }
		set { feet[0] = value; }
	}
	public FBFootState rightFoot {
		get { return feet[1]; }
		set { feet[1] = value; }
	}
	public bool leftFootOnFloor = true;
	public FBFootState movingFoot {
		get { return feet[leftFootOnFloor ? 1 : 0]; }
		private set { }
	}
	public FBFootState fixedFoot {
		get { return feet[leftFootOnFloor ? 0 : 1]; }
		private set { }
	}

	public Vector3 controllerTarget;
	public Vector3 direction;
	private FBMotionAnalyzer motion;

	// Use this for initialization
	void Start () {
		motion = GetComponent<FBMotionAnalyzer> ();
	}

	// Update is called once per frame
	void Update () {

	}
}
