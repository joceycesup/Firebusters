using System;
using System.Collections;
using UnityEngine;

[Serializable]
public enum FBAction {
	None = 0x00,
	Walk = 0x01,
	Aim = 0x02,
	Strike = 0x04,
	Draw = 0x08,
	Sheathe = 0x10,
	Pickup = 0x20,
	Throw = 0x40
}

public static class FBActionExtensions {
	public static string ToMaskString (this FBAction fba) {
		string res = "";
		if (fba == FBAction.None) {
			res = "None";
		}
		else {
			foreach (FBAction a in Enum.GetValues (typeof (FBAction))) {
				if (TestMask (fba, a)) {
					if (res.Length > 0)
						res += "|";
					switch (a) {
						case FBAction.Aim:
							res += "Aim";
							break;
						case FBAction.Draw:
							res += "Draw";
							break;
						case FBAction.Pickup:
							res += "Pickup";
							break;
						case FBAction.Sheathe:
							res += "Sheathe";
							break;
						case FBAction.Strike:
							res += "Strike";
							break;
						case FBAction.Throw:
							res += "Throw";
							break;
						case FBAction.Walk:
							res += "Walk";
							break;
						default:
							break;
					}
				}
			}
		}
		return res;
	}

	public static bool TestMask (this FBAction mask, FBAction a) {
		return (mask & a) != 0;
	}
}

[Serializable]
[RequireComponent (typeof (FBPhoneDataHandler))]
public class FBMotionAnalyzer : MonoBehaviour {
	public delegate void ActionEvent ();
	public event ActionEvent OnStrike;
	public event ActionEvent OnDraw;
	public event ActionEvent OnSheathe;
	public event ActionEvent OnPickup;
	public event ActionEvent OnThrow;

	[SerializeField]
	private FBAction _abilities = FBAction.None;
	public FBAction abilities {
		get { return _abilities; }
		set { _abilities = value; }
	}

	public void ToggleAbilities (FBAction a) {
		abilities = abilities ^ a;
		//Debug.Log (abilities.ToMaskString ());
	}

	public bool TestMask (FBAction a) {
		return abilities.TestMask (a);
	}

	private FBPhoneDataHandler sensor;
	public bool usePhoneDataHandler = true;

	//########## analyze ##########
	private bool[] analyzing = { false, false, false };

	//########## actions ##########
	public bool isAxePuppet = true;
	public bool isCarryingItem = false;

	//########## walking ##########
	// x : roll  : walking
	public float walking {
		get;
		private set;
	}

	//########## steering ##########
	// z : pitch : steering
	public float steering {
		get;
		private set;
	}

	//########## controller rotation values ##########
	public float maxRoll = 30.0f;
	public AnimationCurve rollFactor = AnimationCurve.EaseInOut (0.2f, 0.0f, 1.0f, 1.0f);
	public float maxPitch = 30.0f;
	public AnimationCurve pitchFactor = AnimationCurve.EaseInOut (0.2f, 0.0f, 1.0f, 1.0f);

	public Vector3 rotation {
#if UNITY_EDITOR
		get { return usePhoneDataHandler ? sensor.orientation : kbRotation; }
#else
		get { return sensor.orientation; }
#endif
	}
	public Vector3 acceleration {
		get { return sensor.cleanAcceleration; }
	}
#if UNITY_EDITOR
	//----- keyboard rotation -----
	private Vector3 kbRotation;
#endif

	//########## tool values ##########
	public float sheatheDrawMaxDuration = 0.4f;
	public float sheatheDrawInitialAcc = 2.0f;
	public float sheatheDrawFinalAcc = -2.0f;
	public float sheatheDrawAngle = -10.0f;

	public float strikeMaxDuration = 0.4f;
	public float strikeInitialAcc = 2.0f;
	public float strikeFinalAcc = -2.0f;
	public float strikeAngle = -20.0f;

	void Awake () {
		sensor = gameObject.GetComponent<FBPhoneDataHandler> ();
		walking = -1.0f;
	}

	void Update () {
#if UNITY_EDITOR
		if (usePhoneDataHandler) {
#endif
			steering = Mathf.Sign (-sensor.orientation.z) * pitchFactor.Evaluate (Mathf.Abs (sensor.orientation.z) / maxPitch);
			if (TestMask (FBAction.Walk)) {
				UpdateWalkValues ();
			}
			else {
				walking = -1.0f;
			}
			if (TestMask (FBAction.Strike)) {
				if (acceleration.x > strikeInitialAcc)
					StartCoroutine (AnalyzeAccelerometerAxis (0, strikeFinalAcc, false, () => {
						OnStrike ();
					}, strikeMaxDuration, 0, strikeAngle));
			}
			if (TestMask (FBAction.Draw)) {
				if (acceleration.y > sheatheDrawInitialAcc)
					StartCoroutine (AnalyzeAccelerometerAxis (1, sheatheDrawFinalAcc, false, () => {
						OnDraw ();
					}, sheatheDrawMaxDuration, 0, sheatheDrawAngle));
			}
			else if (TestMask (FBAction.Sheathe)) {
				if (acceleration.y > sheatheDrawInitialAcc)
					StartCoroutine (AnalyzeAccelerometerAxis (1, sheatheDrawFinalAcc, false, () => {
						OnSheathe ();
					}, sheatheDrawMaxDuration, 0, sheatheDrawAngle));
			}
			kbRotation = sensor.orientation;
#if UNITY_EDITOR
		}
		else {
			float horizontal = Input.GetAxis ("HorizontalL");
			steering = Mathf.Sign (horizontal) * pitchFactor.Evaluate (Mathf.Abs (horizontal));

			if (TestMask (FBAction.Walk)) {
				UpdateWalkValues ();
			}
			else {
				walking = -1.0f;
			}
			if (TestMask (FBAction.Strike)) {
				if (Input.GetKeyDown ("v"))
					OnStrike ();
			}
			if (TestMask (FBAction.Draw)) {
				if (Input.GetKeyDown ("c"))
					OnDraw ();
			}
			else if (TestMask (FBAction.Sheathe)) {
				if (Input.GetKeyDown ("c"))
					OnSheathe ();
			}
			kbRotation.x -= Input.GetAxis ("VerticalR");
			kbRotation.y += Input.GetAxis ("HorizontalR");
		}
		Vector3 debugFwd = Quaternion.Euler (rotation) * Vector3.forward;
		Debug.DrawRay (Vector3.zero, debugFwd, Color.cyan);
#endif
	}

	private void UpdateWalkValues () {
#if UNITY_EDITOR
		if (usePhoneDataHandler) {
#endif
			Debug.DrawRay (transform.position, sensor.acceleration, Color.red);

			walking = rollFactor.Evaluate (sensor.orientation.x / maxRoll);

#if UNITY_EDITOR
			kbRotation = sensor.orientation;
		}
		else {
			walking = rollFactor.Evaluate (Input.GetAxis ("VerticalL"));
		}
#endif
	}

	// ge = greater or equal
	private IEnumerator AnalyzeAccelerometerAxis (int axis, float stopValue, bool ge, Action callback, float maxDelay, int rotationAxis = -1, float rotationValue = 0.0f) {
		if (!analyzing[axis]) {
			float initialAcc = acceleration[axis];
			float finalAcc = 0.0f;
			float finalRot = 0.0f;

			analyzing[axis] = true;
			float endTime = Time.time + maxDelay;
			bool success = false;
			bool rotationSuccess = true;
			float initialRotation = 0.0f;
			if (rotationAxis >= 0) {
				initialRotation = rotation[rotationAxis];
			}
			float inequalityFactor = ge ? 1.0f : -1.0f;
			while (!success && Time.time < endTime) {
				if (rotationAxis >= 0) {
					finalRot = rotation[rotationAxis] - initialRotation;
					rotationSuccess = (rotationValue >= 0.0f) ? (finalRot >= rotationValue) : (finalRot < rotationValue);
				}
				finalAcc = acceleration[axis];
				if (rotationSuccess && (finalAcc * inequalityFactor >= stopValue * inequalityFactor)) {
					success = true;
				}
				else {
					yield return null;
				}
			}
			float time = Time.time + maxDelay - endTime;
			Debug.Log ("Returned " + success + " on axis " + axis + " in " + time + " seconds" + (rotationAxis >= 0 ? (" with a rotation of " + finalRot) : ""));
			Debug.Log (" Initial acc : " + initialAcc);
			Debug.Log (" Final acc   : " + finalAcc);
			if (success) {
				callback ();
			}
			analyzing[axis] = false;
		}
	}
}
