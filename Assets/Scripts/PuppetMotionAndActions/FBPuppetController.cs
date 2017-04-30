using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FBFootState {
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

	protected FBFootState () { }
}

//[ExecuteInEditMode]
[RequireComponent (typeof (FBMotionAnalyzer))]
public class FBPuppetController : MonoBehaviour {
	//-------------------- MovementState --------------------
	public enum MovementState {
		Idle = 0x01,//0001
		ClimbingStep = 0x02,//0010
		SpinningAround = 0x05,//0101
		Walking = 0x06 //0110
	}
	public MovementState state = MovementState.Idle;
	public bool insMoving { get { return ((int) state & 2) != 0; } }

	//-------------------- Actions --------------------
	private FBAction _actions = FBAction.None;
	public FBAction actions {
		get { return _actions; }
		private set { _actions = value; }
	}

	//-------------------- Feet --------------------
	[SerializeField]
	private FootStateInternal[] feet = new FootStateInternal[2];
	public FBFootState leftFoot {
		get { return feet[0]; }
		private set { }
	}
	public FBFootState rightFoot {
		get { return feet[1]; }
		private set { }
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
	private FBMotionAnalyzer motion;

	public float steeringTurnRate = 90.0f;
	private float yRotation = 0.0f;

	public bool useMaxAngleSpan = false;
	public float maxAngleSpan = 40.0f;
	private Vector3 _targetDirection;
	public Vector3 targetDirection {
		get { return _targetDirection; }
		set {
			Vector3 tmp = value;
			tmp.y = 0.0f;
			_targetDirection = Vector3.Normalize (tmp);
		}
	}

	//-------------------- camera --------------------

	public new Camera camera;
	public Transform cameraTarget;
	public Transform cameraPosition;

	//-------------------- pickable and items --------------------

	private GameObject carriedItem = null;

	//-------------------- actions --------------------

	private void OnEnable () {
		motion.OnDraw += Draw;
		motion.OnPickup += Pickup;
		motion.OnSheathe += Sheathe;
		motion.OnStrike += Strike;
		motion.OnThrow += Throw;
	}

	private void OnDisable () {
		motion.OnDraw -= Draw;
		motion.OnPickup -= Pickup;
		motion.OnSheathe -= Sheathe;
		motion.OnStrike -= Strike;
		motion.OnThrow -= Throw;
	}

	private GameObject CheckForPickable () {
		return null;
	}

	private void Draw () {
		if (!FBMotionAnalyzer.TestMask (actions, FBAction.Sheathe) && !FBMotionAnalyzer.TestMask (actions, FBAction.Draw)) {
			Debug.Log ("Started action " + FBAction.Draw);
			motion.SetAbility (FBAction.Sheathe);
			actions |= FBAction.Draw;
			StartCoroutine (StopAction (FBAction.Draw, 0.5f));
		}
	}

	private void Sheathe () {
		if (!FBMotionAnalyzer.TestMask (actions, FBAction.Sheathe) && !FBMotionAnalyzer.TestMask (actions, FBAction.Draw)) {
			Debug.Log ("Started action " + FBAction.Sheathe);
			motion.SetAbility (FBAction.Draw);
			actions |= FBAction.Sheathe;
			StartCoroutine (StopAction (FBAction.Sheathe, 0.5f));
		}
	}

	private void Strike () {
		if (!FBMotionAnalyzer.TestMask (actions, FBAction.Strike)) {
			Debug.Log ("Started action " + FBAction.Strike);
			actions |= FBAction.Strike;
			StartCoroutine (StopAction (FBAction.Strike, 0.5f));
		}
	}

	private void Pickup () {
		if (!FBMotionAnalyzer.TestMask (actions, FBAction.Pickup)) {
			if ((carriedItem = CheckForPickable ()) != null) {
				motion.isCarryingItem = true;
				Debug.Log ("Picking " + carriedItem);
				Debug.Log ("Started action " + FBAction.Pickup);
				actions |= FBAction.Pickup;
			}
		}
	}

	private void Throw () {
		if (!FBMotionAnalyzer.TestMask (actions, FBAction.Throw)) {
			if (carriedItem != null) {
				motion.isCarryingItem = false;
				Debug.Log ("Thowing " + carriedItem);
				Debug.Log ("Started action " + FBAction.Throw);
				actions |= FBAction.Throw;
				StartCoroutine (StopAction (FBAction.Throw, 0.5f));
			}
		}
	}

	private IEnumerator StopAction (FBAction a, float delay = 0.0f) {
		if (a != FBAction.None) {
			float endTime = Time.time + delay;
			while (Time.time < endTime) {
				yield return null;
			}
			actions &= ~a;
			Debug.Log ("Ended action " + a);
		}
	}

	//-------------------- game loops --------------------

	private void Awake () {
		motion = GetComponent<FBMotionAnalyzer> ();
	}

	void Start () {
		targetDirection = transform.forward;
		if (feet[0] == null || feet[1] == null) {
			Debug.LogError ("Feet are not set!");
			Debug.Break ();
		}
		else {
			controllerTarget = transform.position;
			controllerHeight = controllerTarget.y - (feet[0].transform.position.y + feet[1].transform.position.y) / 2.0f;
		}
	}

	void Update () {
		yRotation += steeringTurnRate * motion.steering * Time.deltaTime;
		Quaternion rotation = Quaternion.Euler (0.0f, yRotation, 0.0f);
		if (useMaxAngleSpan) {
			Vector3 currentDirection = rotation * Vector3.forward;
			float angle = Vector3.Angle (targetDirection, currentDirection);
			if (angle > maxAngleSpan) {
				Vector3 correctedDirection = Quaternion.Euler (0.0f, (Vector3.Cross (targetDirection, currentDirection).y < 0.0f ? -1.0f : 1.0f) * maxAngleSpan, 0.0f) * targetDirection;
				rotation = Quaternion.LookRotation (currentDirection);
				transform.rotation = Quaternion.Slerp (rotation, Quaternion.LookRotation (correctedDirection), (steeringTurnRate * Time.deltaTime) / (angle - maxAngleSpan));
			}
			else {
				transform.rotation = rotation;
			}
			yRotation = transform.rotation.eulerAngles.y;
		}
		else {
			transform.rotation = rotation;
			targetDirection = transform.forward;
		}
		feet[0].transform.rotation = feet[1].transform.rotation = rotation;

		if (feet[0].changed || feet[1].changed) {
			controllerTarget = controllerTarget;// calls private set and sets correct y according to feet vertical position
		}

		camera.transform.LookAt (cameraTarget);
	}

	//-------------------- footstate --------------------

	[System.Serializable]
	private class FootStateInternal : FBFootState {
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

	public void CreateFoot (Transform t, bool left) {
		if (t == null) {
			Debug.LogWarning ("Transform is null!");
			return;
		}
		feet[left ? 0 : 1] = new FootStateInternal (t);
	}
}
