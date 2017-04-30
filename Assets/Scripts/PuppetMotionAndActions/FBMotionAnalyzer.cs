using UnityEngine;
using System;

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

[RequireComponent (typeof (FBPhoneDataHandler))]
public class FBMotionAnalyzer : MonoBehaviour {
	public delegate void ActionEvent ();
	public event ActionEvent OnStrike;
	public event ActionEvent OnDraw;
	public event ActionEvent OnSheathe;
	public event ActionEvent OnPickup;
	public event ActionEvent OnThrow;
	
#pragma warning disable 0414
	[SerializeField]
	private FBAction _abilities = FBAction.None;
	public FBAction abilities {
		get { return _abilities; }
		set { _abilities = value; }
	}
	public bool SetAbility (FBAction a) {
		if (TestMask (a))
			return false;
		switch (a) {
			case FBAction.Walk:
				break;
			case FBAction.Aim:
				if (isAxePuppet || isCarryingItem)
					break;
				_abilities = FBAction.Sheathe | FBAction.Aim;
				break;
			case FBAction.Strike:
				if (!isAxePuppet || isCarryingItem)
					break;
				_abilities = FBAction.Pickup | FBAction.Throw | FBAction.Walk | FBAction.Strike;
				break;
			case FBAction.Draw:
				if (isAxePuppet)
					break;
				_abilities = FBAction.Walk | FBAction.Draw | FBAction.Pickup | FBAction.Throw;
				break;
			case FBAction.Sheathe:
				if (isAxePuppet || isCarryingItem)
					break;
				_abilities = FBAction.Sheathe | FBAction.Aim;
				break;
			case FBAction.Pickup:
			case FBAction.Throw:
				_abilities = FBAction.Walk | (isAxePuppet ? FBAction.Strike : FBAction.Draw) | FBAction.Pickup | FBAction.Throw;
				break;
			default:
				break;
		}
		return true;
	}
	public void SetAbilities (FBAction na) {
		if (na != FBAction.None)
			foreach (FBAction a in Enum.GetValues (typeof (FBAction))) {
				if (TestMask (na, a))
					SetAbility (a);
			}
	}

	public bool TestMask (FBAction a) {
		return TestMask (abilities, a);
	}

	public static bool TestMask (FBAction mask, FBAction a) {
		return (mask & a) != 0;
	}

	private FBPhoneDataHandler sensor;
	public bool usePhoneDataHandler = true;

	//---------- actions ----------
	public bool isAxePuppet = true;
	public bool isCarryingItem = false;

	//---------- walking ----------
	// x : roll  : walking
	public float walking {
		get;
		private set;
	}

	//---------- steering ----------
	// z : pitch : steering
	public float steering {
		get;
		private set;
	}

	//---------- controller rotation values ----------
	public float maxRoll = 30.0f;
	public AnimationCurve rollFactor = AnimationCurve.EaseInOut (0.2f, 0.0f, 1.0f, 1.0f);
	public float maxPitch = 30.0f;
	public AnimationCurve pitchFactor = AnimationCurve.EaseInOut (0.2f, 0.0f, 1.0f, 1.0f);

	public Vector3 rotation {
		get { return sensor.orientation; }
	}
	public Vector3 acceleration {
		get { return sensor.cleanAcceleration; }
	}

	void Awake () {
		sensor = gameObject.GetComponent<FBPhoneDataHandler> ();
		walking = -1.0f;
		Debug.Log (System.Convert.ToString ((int) FBAction.Strike, 2));
		Debug.Log (System.Convert.ToString ((int) abilities, 2));
	}

	void Update () {
		if (TestMask (FBAction.Walk)) {
			UpdateWalkValues ();
		}
		if (TestMask (FBAction.Strike)) {
			if (acceleration.x > 2.0f) {
				OnStrike ();
			}
		}
		if (TestMask (FBAction.Draw)) {
			if (acceleration.y > 2.0f) {
				OnDraw ();
			}
		}
		else if (TestMask (FBAction.Sheathe)) {
			if (acceleration.y > 2.0f) {
				OnSheathe ();
			}
		}
	}

	private void UpdateWalkValues () {
		if (usePhoneDataHandler) {
			Debug.DrawRay (transform.position, sensor.acceleration, Color.red);
			steering = Mathf.Sign (-sensor.orientation.z) * pitchFactor.Evaluate (Mathf.Abs (sensor.orientation.z) / maxPitch);

			walking = rollFactor.Evaluate (sensor.orientation.x / maxRoll);
		}
		else {
			float horizontal = Input.GetAxis ("HorizontalL");
			steering = Mathf.Sign (horizontal) * pitchFactor.Evaluate (Mathf.Abs (horizontal));

			walking = rollFactor.Evaluate (Input.GetAxis ("VerticalL"));
		}
	}
}
