using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent (typeof (FBMotionAnalyzer))]
public class FBPuppetController : MonoBehaviour {
	//-------------------- Feet --------------------
	[SerializeField]
	private FootState[] feet = new FootState[2];
	public FootState leftFoot {
		get { return feet[0]; }
		set { feet[0] = value; }
	}
	public FootState rightFoot {
		get { return feet[1]; }
		set { feet[1] = value; }
	}
	public bool leftFootOnFloor = true;
	public FootState movingFoot {
		get { return feet[leftFootOnFloor ? 1 : 0]; }
		private set { }
	}
	public FootState fixedFoot {
		get { return feet[leftFootOnFloor ? 0 : 1]; }
		private set { }
	}

	//-------------------- general rotation --------------------
	public Vector3 controllerTarget;
	public Vector3 direction;
	private FBMotionAnalyzer motion;

	public float steeringTurnRate = 90.0f;
	private float yRotation = 0.0f;

	//-------------------- camera --------------------

	public new Camera camera;
	public Transform cameraTarget;
	public Transform cameraPosition;

	// Use this for initialization
	void Start () {
		motion = GetComponent<FBMotionAnalyzer> ();
	}

	// Update is called once per frame
	void Update () {
		direction = transform.forward;
		direction.y = 0.0f;
		direction = Vector3.Normalize (direction);

		yRotation += steeringTurnRate * motion.steering * Time.deltaTime;
		transform.rotation = Quaternion.Euler (motion.rotation.x, yRotation, motion.rotation.z);

		camera.transform.LookAt (cameraTarget);
	}

	[System.Serializable]
	public class FootState {
		[SerializeField]
		private Transform _transform;

		public Vector3 target;
		//if not onStep, is onFloor
		public bool onStep;

		public Transform transform { get { return _transform; } private set { _transform = value; } }
		//public Vector3 target { get { return _target; } private set { _target = value; } }

		public FootState (Transform t) {
			transform = t;
			target = transform.position;
			onStep = false;
		}
	}
}
