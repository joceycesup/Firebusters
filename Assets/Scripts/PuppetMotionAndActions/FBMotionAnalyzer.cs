using System;
using System.Collections;
using UnityEngine;
#if DEBUG_ENABLED
using UnityEngine.UI;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public enum FBRotationAxis {
	None = -1,
	Roll = 0,
	Yaw = 1,
	Pitch = 2
}
[Serializable]
public enum FBAccAxis {
	X = 0,
	Y = 1,
	Z = 2
}

[Serializable]
public struct FBAccelerationMotion {
	public string name;
	public FBAccAxis accAxis;
	public float maxDuration;
	public float initialAcc;
	public float finalAcc;
	public FBRotationAxis rotAxis;
	public float angle;
#if UNITY_EDITOR
	public bool showInInspector;
#endif
}

#if UNITY_EDITOR
public static class FBAccelerationMotionExtensions {
	public static FBAccelerationMotion GUIField (this FBAccelerationMotion motion) {
		EditorGUI.indentLevel++;
		motion.accAxis = (FBAccAxis) EditorGUILayout.EnumPopup (motion.name + " acceleration axis", motion.accAxis);
		motion.maxDuration = EditorGUILayout.FloatField (motion.name + " max duration", motion.maxDuration);
		motion.initialAcc = EditorGUILayout.FloatField (motion.name + " initial acceleration", motion.initialAcc);
		motion.finalAcc = EditorGUILayout.FloatField (motion.name + " final acceleration", motion.finalAcc);
		motion.rotAxis = (FBRotationAxis) EditorGUILayout.EnumPopup (motion.name + " rotaiton axis", motion.rotAxis);
		if (motion.rotAxis != FBRotationAxis.None)
			motion.angle = EditorGUILayout.FloatField (motion.name + " angle", motion.angle);
		EditorGUI.indentLevel--;
		return motion;
	}
}
#endif

[Serializable]
public enum FBAction {
	None = 0x00,
	Walk = 0x01,
	Aim = 0x02,
	Strike = 0x04,
	Draw = 0x08,
	Sheathe = 0x10,
	Grab = 0x20,
	Pickup = 0x40,
	Throw = 0x80
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
						case FBAction.Grab:
							res += "Grab";
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
//[RequireComponent (typeof (FBPhoneDataHandler))]
public class FBMotionAnalyzer : MonoBehaviour {
	public delegate void ActionEvent ();
	public event ActionEvent OnStrike;
	public event ActionEvent OnDraw;
	public event ActionEvent OnSheathe;
	public event ActionEvent OnGrab;
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

	public FBPhoneDataHandler sensor {
#if DEBUG_ENABLED
		get { return FBPhonesContainer.sensors == null ? null : FBPhonesContainer.sensors[isAxePuppetIndex]; }
#else
		get { return FBPhonesContainer.sensors[isAxePuppetIndex]; }
#endif
	}
	public bool usePhoneDataHandler = true;
	public bool useKbRight = true;

	//########## analyze ##########
	private bool[] analyzing = { false, false, false };

	//########## actions ##########
	private int isAxePuppetIndex;
	[SerializeField]
	private bool _isAxePuppet;
	public bool isAxePuppet {
		get { return _isAxePuppet; }
		set { _isAxePuppet = value; isAxePuppetIndex = value ? 1 : 0; }
	}
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
#if USEKB
#if DEBUG_ENABLED
		get { return usePhoneDataHandler ? (sensor != null ? sensor.orientation : Vector3.zero) : kbRotation; }
#else
		get { return usePhoneDataHandler ? sensor.orientation : kbRotation; }
#endif
#else
		get { return sensor.orientation; }
#endif
	}
	public Vector3 acceleration {
#if DEBUG_ENABLED
		get { return sensor != null ? sensor.cleanAcceleration : Vector3.zero; }
#else
		get { return sensor.cleanAcceleration; }
#endif
	}
	//----- keyboard rotation -----
	private Vector3 kbRotation;

	//########## tool values ##########
	[SerializeField]
	public FBAccelerationMotion toolMotion;
	[SerializeField]
	public FBAccelerationMotion grabMotion;
	/*
	public float sheatheDrawMaxDuration = 0.4f;
	public float sheatheDrawInitialAcc = 2.0f;
	public float sheatheDrawFinalAcc = -2.0f;
	public float sheatheDrawAngle = -10.0f;

	public float strikeMaxDuration = 0.4f;
	public float strikeInitialAcc = 2.0f;
	public float strikeFinalAcc = -2.0f;
	public float strikeAngle = -20.0f;//*/
#if DEBUG_ENABLED
	private Text debugText;
#endif

#if UNITY_EDITOR
	//########## debug #########
	public bool showSuccessDebug = true;
	public bool showFailureDebug = true;
#endif

	void Awake () {
#if DEBUG_ENABLED
		debugText = GameObject.Find ("TextDebug").GetComponent<Text> ();
#endif
		walking = 0.0f;
		isAxePuppet = isAxePuppet;//sets isAxePuppetIndex
	}

	void Update () {
#if DEBUG_ENABLED
		if (sensor == null) {
			debugText.gameObject.SetActive (true);
			Debug.Log ("Phone " + (isAxePuppet ? "Marius" : "Louis") + " not found");
			debugText.text = "Phone not found";
			return;
		}
		else {
			debugText.gameObject.SetActive (false);
		}
#endif
#if PLAYTEST
		if (Input.GetKeyDown ("p"))
			usePhoneDataHandler = !usePhoneDataHandler;
#endif

		if (usePhoneDataHandler) {
			steering = Mathf.Sign (-sensor.orientation.z) * pitchFactor.Evaluate (Mathf.Abs (sensor.orientation.z) / maxPitch);
			if (TestMask (FBAction.Walk)) {
				UpdateWalkValues ();
			}
			if (!Mathf.Approximately (walking, 0.0f))
				steering *= Mathf.Sign (walking);
			else {
				walking = 0.0f;
			}
			if (TestMask (FBAction.Strike)) {
				DetectMotionStart (toolMotion, () => OnStrike ());
			}
			if (TestMask (FBAction.Draw)) {
				DetectMotionStart (toolMotion, () => { walking = 0.0f; OnDraw (); });
			}
			else if (TestMask (FBAction.Sheathe)) {
				DetectMotionStart (toolMotion, () => OnSheathe ());
			}
			if (TestMask (FBAction.Grab)) {
				DetectMotionStart (grabMotion, () => OnGrab ());
			}
#if USEKB
			kbRotation = sensor.orientation;
#endif
		}
		else {
			float horizontal = Input.GetAxis ("Horizontal" + (useKbRight ? "R" : "L"));
			steering = Mathf.Sign (horizontal) * pitchFactor.Evaluate (Mathf.Abs (horizontal));

			if (TestMask (FBAction.Walk)) {
				UpdateWalkValues ();
			}
			else {
				walking = 0.0f;
			}
			if (TestMask (FBAction.Strike)) {
				if (Input.GetButtonDown ("Strike"))
					OnStrike ();
			}
			if (TestMask (FBAction.Draw)) {
				if (Input.GetButtonDown ("DrawSheathe"))
					OnDraw ();
			}
			else if (TestMask (FBAction.Sheathe)) {
				if (Input.GetButtonDown ("DrawSheathe"))
					OnSheathe ();
			}
			if (TestMask (FBAction.Grab)) {
				if (Input.GetButtonDown ("Grab"))
					OnGrab ();
			}
			kbRotation.x = Mathf.Clamp (kbRotation.x - Input.GetAxis ("VerticalE"), -70.0f, 70.0f);
			kbRotation.y += Input.GetAxis ("HorizontalE");
		}
#if DEBUG_ENABLED
		//Vector3 debugFwd = Quaternion.Euler (rotation) * Vector3.forward;
		//Debug.DrawRay (Vector3.zero, debugFwd, Color.cyan);
#endif
	}

	private bool DetectMotionStart (FBAccelerationMotion am, Action callback) {
		bool start = am.initialAcc > 0.0f ?
			acceleration[(int) am.accAxis] > am.initialAcc :
			acceleration[(int) am.accAxis] < am.initialAcc;
		if (start) {
			StartCoroutine (AnalyzeAccelerationMotion (am, callback));
		}
		return start;
	}

	private void UpdateWalkValues () {
		if (usePhoneDataHandler) {
			Debug.DrawRay (transform.position, sensor.acceleration, Color.red);

			walking = Mathf.Sign (sensor.orientation.x) * rollFactor.Evaluate (sensor.orientation.x / maxRoll);
			//walking = Mathf.Sign (sensor.orientation.x) * rollFactor.Evaluate (Mathf.Abs (sensor.orientation.x / maxRoll));

#if USEKB
			kbRotation = sensor.orientation;
#endif
		}
		else {
			walking = Input.GetAxis ("Vertical" + (useKbRight ? "R" : "L"));
			walking = Mathf.Sign (walking) * rollFactor.Evaluate (Mathf.Abs (walking));
		}
	}

	private IEnumerator AnalyzeAccelerationMotion (FBAccelerationMotion motion, Action callback) {
		int axis = (int) motion.accAxis;
		int rAxis = (int) motion.rotAxis;
		if (!analyzing[axis]) {
			float initialAcc = acceleration[axis];
			float finalAcc = 0.0f;
			float finalRot = 0.0f;

			analyzing[axis] = true;
			float endTime = Time.time + motion.maxDuration;
			bool success = false;
			bool rotationSuccess = true;
			float initialRotation = 0.0f;
			if (motion.rotAxis != FBRotationAxis.None) {
				rotationSuccess = false;
				initialRotation = rotation[rAxis];
			}
			while (!success && Time.time < endTime) {
				if (motion.rotAxis != FBRotationAxis.None) {
					finalRot = rotation[rAxis] - initialRotation;
					rotationSuccess = (motion.angle >= 0.0f) ? (finalRot >= motion.angle) : (finalRot < motion.angle);
				}
				finalAcc = acceleration[axis];
				//Debug.Log (" coucou acc   : " + finalAcc);
				if (rotationSuccess && (motion.finalAcc < 0.0f ? finalAcc <= motion.finalAcc : finalAcc >= motion.finalAcc)) {
					success = true;
				}
				else {
					yield return null;
				}
			}
			if (success) {
				callback ();
			}
#if UNITY_EDITOR
			if ((showSuccessDebug && success) || (showFailureDebug && !success)) {
				float time = Time.time - (endTime - motion.maxDuration);
				Debug.Log ("#####################################################################################");
				Debug.Log ("Returned " + success + " on axis " + axis + " in " + time + " seconds" + (motion.rotAxis != FBRotationAxis.None ? (" with a rotation of " + finalRot) : ""));
				Debug.Log (" Initial acc : " + initialAcc);
				Debug.Log (" Final acc   : " + finalAcc);
			}
#endif
			analyzing[axis] = false;
		}
	}
}
