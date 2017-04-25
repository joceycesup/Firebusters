using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent (typeof (FBMotionAnalyzer))]
public class FBPuppetController : MonoBehaviour {
	//-------------------- Feet --------------------
	[SerializeField]
	private FootStateInternal[] feet = new FootStateInternal[2];
	public FootState leftFoot {
		get { return feet[0]; }
		private set {}
	}
	public FootState rightFoot {
		get { return feet[1]; }
		private set { }
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

	//-------------------- general rotation and position --------------------
	private float controllerHeight;

	private Vector3 _controllerTarget;
	public Vector3 controllerTarget {
		get { return _controllerTarget; }
		set {
			_controllerTarget = value;
			if (feet[0] != null && feet[1] != null) {
				_controllerTarget.y = controllerHeight + (feet[0].transform.position.y + feet[1].transform.position.y) / 2.0f;
			}
		}
	}
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
		if (feet[0] == null || feet[1] == null) {
			Debug.LogError ("Feet are not set!");
			Debug.Break ();
		}
		else {
			controllerTarget = transform.position;
			controllerHeight = controllerTarget.y - (feet[0].transform.position.y + feet[1].transform.position.y) / 2.0f;
		}
	}

	// Update is called once per frame
	void Update () {
		direction = transform.forward;
		direction.y = 0.0f;
		direction = Vector3.Normalize (direction);

		yRotation += steeringTurnRate * motion.steering * Time.deltaTime;
		transform.rotation = Quaternion.Euler (motion.rotation.x, yRotation, motion.rotation.z);

		if (feet[0].changed || feet[1].changed) {
			controllerTarget = controllerTarget;// calls private set and sets correct y according to feet vertical position
		}

		camera.transform.LookAt (cameraTarget);
	}

	[System.Serializable]
	public class FootState {
		[SerializeField]
		private Transform _transform;
		private Vector3 _target;

		public Vector3 target {
			get { return _target; }
			set { _target = value; _changed = true; }
		}
		//if not onStep, is onFloor
		public bool onStep = false;

		public Transform transform { get { return _transform; } protected set { _transform = value; target = _transform.position; } }
		//public Vector3 target { get { return _target; } private set { _target = value; } }

		protected bool _changed;

		protected FootState () { }
	}

	[System.Serializable]
	private class FootStateInternal : FootState {
		public bool changed {
			get {
				if (_changed) {
					_changed = false;
					return true;
				}
				else { return false; }
			}
			set { _changed = value; }
		}

		public FootStateInternal (Transform t) {
			transform = t;
		}
	}

	public void CreateFoot(Transform t, bool left) {
		if (t == null) {
			Debug.LogWarning ("Transform is null!");
			return;
		}
		feet[left ? 0 : 1] = new FootStateInternal (t);
	}
}
