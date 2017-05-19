using System;
using System.Collections;
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

	private Vector3 feetForward;
	private Vector3 feetRight;

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
	[SerializeField]
	public FBMotionAnalyzer motion;

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

	//-------------------- tools --------------------

	public Rigidbody tool;
	private Rigidbody toolBottom;
	private Transform toolTip;
	public Vector3 anticipationBottomDirection;
	public Vector3 strikeBottomDirection;
	public Vector3 anticipationBladeDirection;
	public Vector3 strikeBladeDirection;
	public GameObject strikeAnchors;
	public GameObject anticipationAnchors;

	public float extinguisherYRotation;

	public float strikeDuration = 0.5f;
	public float strikeCooldown = 2.5f;

	public float maxRollAim;
	public float drawTurnRate = 90.0f;
	public float bladeForce;
	public float bottomForce;

	private GameObject carriedItem = null;

	//-------------------- Grabbable --------------------

	public Rigidbody leftHand;
	private CharacterJoint leftHandCJ;
	public static float grabDelay = 0.8f;
	private SphereCollider detectionSphere;
	[SerializeField]
	public CharacterJointValues doorKnobReference;
	private CharacterJointValues toolReference;
	public static float letGoDoorDelay = 0.5f;

	//-------------------- Actions --------------------
	private FBAction _actions = FBAction.None;

	public FBAction actions {
		get { return _actions; }
		private set { _actions = value; }
	}

	//-------------------- actions --------------------

	private void OnEnable () {
		motion.OnDraw += Draw;
		motion.OnGrab += Grab;
		motion.OnPickup += Pickup;
		motion.OnSheathe += Sheathe;
		motion.OnStrike += Strike;
		motion.OnThrow += Throw;
	}

	private void OnDisable () {
		motion.OnDraw -= Draw;
		motion.OnGrab -= Grab;
		motion.OnPickup -= Pickup;
		motion.OnSheathe -= Sheathe;
		motion.OnStrike -= Strike;
		motion.OnThrow -= Throw;
	}

	private GameObject CheckForPickable () {
		throw new NotImplementedException ();
		return null;
	}

	private void Grab () {
		if (!actions.TestMask (FBAction.Grab)) {

			Debug.Log ("#########################");
			Debug.Log (" Checking for grabbables ");
			Rigidbody doorKnob = null;
			Collider[] colliders = detectionSphere.OverlapSphere (1);
			foreach (Collider c in colliders) {
				if (c.CompareTag ("DoorKnob")) {
					Debug.Log (c + " : " + Vector3.Dot (c.transform.forward, transform.forward));
					if (Vector3.Dot (c.transform.forward, transform.forward) > 0.0f && doorKnob == null) {
						doorKnob = c.GetComponent<Rigidbody> ();
					}
				}
			}
			if (!doorKnob)
				return;

			Debug.Log ("Started action " + FBAction.Grab);

			motion.ToggleAbilities (FBAction.Grab | FBAction.Draw | FBAction.Strike);
			actions |= FBAction.Grab;
			doorKnob.tag = "Untagged";

			//*
			GrabItem (doorKnobReference, doorKnob, () => {
				StartCoroutine (DoItLater (() => {
					doorKnob.tag = "DoorKnob";
					GrabItem (toolReference, toolBottom, () => {
						actions &= ~FBAction.Grab;
						motion.ToggleAbilities (FBAction.Grab | FBAction.Draw | FBAction.Strike);
						Debug.Log (FBAction.Grab + " available");
					});
				}, 1.0f));
			});/*/
			CharacterJoint doorKnobCJ = null;
			Destroy (leftHandCJ);
			doorKnobCJ = AttachItem (doorKnob.gameObject, doorKnobReference, leftHand, () => {
				StartCoroutine (DoItLater (() => {
					doorKnob.tag = "DoorKnob";
					Destroy (doorKnobCJ);
					GrabItem (toolReference, toolBottom, () => {
						actions &= ~FBAction.Grab;
						motion.ToggleAbilities (FBAction.Grab | FBAction.Draw | FBAction.Strike);
						Debug.Log (FBAction.Grab + " available");
					});
				}, 1.0f));
			});
			//*/
			   /*
			   StartCoroutine (DoItLater (() => {
				   GrabItem (toolReference, toolBottom, () => {
					   actions &= ~FBAction.Grab;
					   motion.ToggleAbilities (FBAction.Grab | FBAction.Draw | FBAction.Strike);
					   Debug.Log (FBAction.Grab + " available");
				   });
			   }, 1.0f));//*/






		}
	}

	private void Draw () {
		if (!actions.TestMask (FBAction.Sheathe) && !actions.TestMask (FBAction.Draw)) {
			Debug.Log ("Started action " + FBAction.Draw);
			//motion.SetAbility (FBAction.Sheathe);
			motion.ToggleAbilities (FBAction.Grab | FBAction.Draw | FBAction.Sheathe | FBAction.Walk | FBAction.Aim);
			extinguisherYRotation = motion.rotation.y;
			actions |= FBAction.Draw;
			tool.isKinematic = true;

			/*
			StartCoroutine (DoItLater (() => {
				actions &= ~FBAction.Draw;
				actions |= FBAction.Aim;
				Debug.Log ("Ended action " + FBAction.Draw + " and started " + FBAction.Aim);
			}, 0.5f));/*/
			//*/
			AkSoundEngine.PostEvent ("Start_Extincteur", toolTip.gameObject);
		}
	}

	private void Sheathe () {
		if (!actions.TestMask (FBAction.Sheathe)) {
			Debug.Log ("Started action " + FBAction.Sheathe);
			//motion.SetAbility (FBAction.Draw);
			motion.ToggleAbilities (FBAction.Grab | FBAction.Draw | FBAction.Sheathe | FBAction.Walk | FBAction.Aim);
			actions &= ~(FBAction.Draw | FBAction.Aim);
			actions |= FBAction.Sheathe;
			StartCoroutine (DoItLater (() => {
				actions &= ~FBAction.Sheathe;
				tool.isKinematic = false;
				tool.constraints = RigidbodyConstraints.None;
				Debug.Log ("Ended action " + FBAction.Sheathe);
			}, -1.0f));
			AkSoundEngine.PostEvent ("Stop_Extincteur", toolTip.gameObject);
			toolTip.gameObject.SetActive (false);
		}
	}

	private void Strike () {
		if (!actions.TestMask (FBAction.Strike)) {
			motion.ToggleAbilities (FBAction.Grab | FBAction.Walk | FBAction.Strike);
			Debug.Log ("Started action " + FBAction.Strike);
			actions |= FBAction.Strike;
			if (anticipationAnchors) {
				anticipationAnchors.SetActive (false);
				strikeAnchors.SetActive (true);
			}
			tool.velocity = Vector3.zero;
			toolTip.tag = "Axe";
			StartCoroutine (DoItAfter ((dt) => {
				Vector3 right = Vector3.Normalize (Vector3.ProjectOnPlane (tool.transform.right, Vector3.up));
				tool.AddForce ((transform.forward * strikeBladeDirection.z + right * strikeBladeDirection.x) * dt * bladeForce, ForceMode.Impulse);
				toolBottom.AddForce ((strikeBottomDirection.x * right + strikeBottomDirection.z * transform.forward) * dt * bottomForce, ForceMode.Impulse);
			}, () => {
				toolTip.tag = "Untagged";
				if (anticipationAnchors) {
					strikeAnchors.SetActive (false);
					anticipationAnchors.SetActive (true);
				}
				motion.ToggleAbilities (FBAction.Grab | FBAction.Walk);
				Debug.Log ("Ended action " + FBAction.Strike);
			}, strikeDuration));
			StartCoroutine (DoItLater (() => {
				actions &= ~FBAction.Strike;
				motion.ToggleAbilities (FBAction.Strike);
				Debug.Log (FBAction.Strike + " available");
			}, strikeCooldown));
		}
	}

	private void Pickup () {
		if (!actions.TestMask (FBAction.Pickup)) {
			if ((carriedItem = CheckForPickable ()) != null) {
				motion.isCarryingItem = true;
				Debug.Log ("Picking " + carriedItem);
				Debug.Log ("Started action " + FBAction.Pickup);
				actions |= FBAction.Pickup;
			}
		}
	}

	private void Throw () {
		if (!actions.TestMask (FBAction.Throw)) {
			if (carriedItem != null) {
				motion.isCarryingItem = false;
				Debug.Log ("Thowing " + carriedItem);
				Debug.Log ("Started action " + FBAction.Throw);
				actions |= FBAction.Throw;
				StartCoroutine (DoItLater (() => {
					actions &= ~FBAction.Throw;
					Debug.Log ("Ended action " + FBAction.Throw);
				}, 0.5f));
			}
		}
	}

	private IEnumerator DoItLater (Action callback, float delay = 0.0f) {
		yield return new WaitForSeconds (delay);
		callback ();
	}

	private IEnumerator DoItAfter (Action<float> a, Action callback, float delay = 0.0f) {
		float endTime = Time.time + delay;
		StartCoroutine (DoWhileThen (
			(dt) => { return Time.time < endTime; },
			a,
			callback
			));
		yield return null;
	}

	private IEnumerator DoWhileThen (Func<float, bool> predicate, Action<float> a, Action callback) {
		do {
			a (Time.deltaTime);
			yield return null;
		} while (predicate (Time.deltaTime));
		callback ();
	}

	private IEnumerator LetGoDoor () {
		float endTime = Time.time + letGoDoorDelay;
		while (Time.time < endTime) {
			if (false) { // porte tournee de tant de degres ou distance puppet a la poignee > a
				break;
			}
			else {
				if (!Mathf.Approximately (motion.walking, 0.0f)) {
					endTime = Time.time + letGoDoorDelay;
				}
			}
			yield return null;
		}
	}

	private void GrabItem (CharacterJointValues reference, Rigidbody target, Action callback) {
		Debug.Log ("GrabItem : " + target);/*
		if (!leftHandCJ)
			return;//*/

		Transform leftHandTransform = leftHand.transform;
		Destroy (leftHandCJ);
		Quaternion tmpRot = leftHandTransform.rotation;
		leftHandTransform.rotation = target.transform.rotation * reference.relativeRotation;
		leftHandCJ = reference.CreateJoint (leftHandTransform.gameObject, target);
		leftHandTransform.rotation = tmpRot;

		float grabSpeed = Vector3.Distance (leftHandCJ.connectedAnchor, reference.connectedAnchor) / grabDelay;
		StartCoroutine (DoItAfter ((dt) => {
			float factor = (grabSpeed * dt) / Vector3.Distance (leftHandCJ.connectedAnchor, reference.connectedAnchor);
			reference.Lerp (leftHandCJ, factor);
		}, () => {
			//reference.Apply (leftHandCJ);

			callback ();
		}, grabDelay));
	}

	private CharacterJoint AttachItem (GameObject source, CharacterJointValues reference, Rigidbody target, Action callback) {
		Debug.Log ("AttachItem : " + target + " to " + source);

		CharacterJoint cj = null;
		Transform originalTransform = source.transform;
		Quaternion tmpRot = originalTransform.rotation;
		originalTransform.rotation = target.transform.rotation * reference.relativeRotation;
		cj = reference.CreateJoint (source, target);
		originalTransform.rotation = tmpRot;

		float grabSpeed = Vector3.Distance (cj.connectedAnchor, reference.connectedAnchor) / grabDelay;
		StartCoroutine (DoItAfter ((dt) => {
			float factor = (grabSpeed * dt) / Vector3.Distance (cj.connectedAnchor, reference.connectedAnchor);
			reference.Lerp (cj, factor);
		}, () => {
			//reference.Apply (cj);

			callback ();
		}, grabDelay));

		return cj;
	}

	//-------------------- game loops --------------------

	private void Awake () {
#if USEKB
		Debug.Log ("Using keyboard");
#endif
		if (motion == null)
			motion = GetComponent<FBMotionAnalyzer> ();
		toolTip = tool.transform.GetChild (0);
		detectionSphere = GetComponent<SphereCollider> ();
		toolBottom = tool.transform.GetChild (1).gameObject.GetComponent<Rigidbody> ();
		leftHandCJ = leftHand.GetComponents<CharacterJoint> ()[1];
		toolReference = new CharacterJointValues (leftHandCJ);
		cameraPosition = camera.transform.parent;
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
		yRotation = transform.rotation.eulerAngles.y;
	}

	void Update () {/*
		if (leftHand) {
			Debug.Log (Quaternion.Angle (leftHand.transform.rotation, toolBottom.transform.rotation));
		}//*/
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
			feetForward = Vector3.Normalize (Vector3.Cross (feet[1].target - feet[0].target, Vector3.up));
			feetRight = Vector3.Cross (Vector3.up, feetForward);
		}
#if UNITY_EDITOR
		Vector3 debugRayStart = (feet[1].target + feet[0].target) / 2.0f;
		Vector3 forwardAnticipate = Vector3.Normalize (feetForward + transform.forward);
		Quaternion anticipateStrikeRot = Quaternion.Euler (0.0f, 135.0f, 0.0f);
		Debug.DrawRay (debugRayStart, feetForward, Color.blue);
		Debug.DrawRay (debugRayStart, forwardAnticipate, Color.magenta);
		Debug.DrawRay (debugRayStart, transform.forward, Color.red);
		Debug.DrawRay (debugRayStart, anticipateStrikeRot * forwardAnticipate, Color.green);
#endif
		camera.transform.LookAt (cameraTarget);
	}

	private void FixedUpdate () {
		if (actions.TestMask (FBAction.Aim)) {
			//Debug.Log ("aiming");
			tool.transform.rotation = Quaternion.Euler (motion.rotation.x, motion.rotation.y - extinguisherYRotation + yRotation, tool.rotation.eulerAngles.z);
		}
		else if (actions.TestMask (FBAction.Draw)) {
			Quaternion aimRotation = Quaternion.Euler (motion.rotation.x, motion.rotation.y - extinguisherYRotation + yRotation, tool.rotation.eulerAngles.z);
			tool.transform.rotation = Quaternion.RotateTowards (tool.transform.rotation, aimRotation, drawTurnRate * Time.fixedDeltaTime);
			float angle = Quaternion.Angle (tool.transform.rotation, aimRotation);
			//Debug.Log (angle + " : " + (drawTurnRate * Time.fixedDeltaTime));
			if (angle < drawTurnRate * Time.fixedDeltaTime) {
				tool.isKinematic = false;
				tool.constraints = RigidbodyConstraints.FreezeRotation;
				actions &= ~FBAction.Draw;
				actions |= FBAction.Aim;
				toolTip.gameObject.SetActive (true);
				Debug.Log ("Ended action " + FBAction.Draw + " and started " + FBAction.Aim);
			}
		}
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
