using UnityEngine;
using System;

[RequireComponent (typeof (FBPhoneDataHandler))]
public class FBMotionAnalyzer : MonoBehaviour {
	public bool isAxePuppet = true;
	public bool isCarryingItem = false;

	public enum Action {
		Walk = 0x01,
		Aim = 0x02,
		Strike = 0x04,
		Draw = 0x08,
		Sheathe = 0x10,
		Pickup = 0x20,
		Throw = 0x40
	}

	private Action _abilities = Action.Walk;
	public Action abilities {
		get;
		set;
	}
	public bool SetAbility (Action a) {
		if (TestMask (a))
			return false;
		switch (a) {
			case Action.Walk:
				break;
			case Action.Aim:
				if (isAxePuppet || isCarryingItem)
					break;
				_abilities = Action.Sheathe | Action.Aim;
				break;
			case Action.Strike:
				if (!isAxePuppet || isCarryingItem)
					break;
				_abilities = Action.Pickup | Action.Throw | Action.Walk | Action.Strike;
				break;
			case Action.Draw:
				if (isAxePuppet)
					break;
				_abilities = Action.Walk | Action.Draw | Action.Pickup | Action.Throw;
				break;
			case Action.Sheathe:
				if (isAxePuppet || isCarryingItem)
					break;
				_abilities = Action.Sheathe | Action.Aim;
				break;
			case Action.Pickup:
			case Action.Throw:
				_abilities = Action.Walk | (isAxePuppet ? Action.Strike : Action.Draw) | Action.Pickup | Action.Throw;
				break;
			default:
				break;
		}
		return true;
	}
	public void SetAbilities (Action na) {
		foreach (Action a in Enum.GetValues (typeof (Action))) {
			if (TestMask (na, a))
				SetAbility (a);
		}
	}

	public bool TestMask (Action a) {
		return TestMask (abilities, a);
	}

	public bool TestMask (Action mask, Action a) {
		return (mask & a) != 0;
	}

	private FBPhoneDataHandler sensor;
	public bool usePhoneDataHandler = true;

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
		get { return sensor.sensorAxis; }
	}

	void Awake () {
		sensor = gameObject.GetComponent<FBPhoneDataHandler> ();
		walking = -1.0f;
	}

	void Update () {
		if (TestMask (Action.Walk)) {
			UpdateWalkValues ();
		}
		if (TestMask (Action.Pickup|Action.Throw)) {
			UpdateWalkValues ();
		}
	}

	private void UpdateWalkValues () {
		if (usePhoneDataHandler) {
			steering = Mathf.Sign (sensor.sensorAxis.z) * pitchFactor.Evaluate (Mathf.Abs (sensor.sensorAxis.z) / maxPitch);

			walking = rollFactor.Evaluate (sensor.sensorAxis.x / maxRoll);
			//Debug.Log (walking);
		}
		else {
			float horizontal = Input.GetAxis ("HorizontalL");
			steering = Mathf.Sign (horizontal) * pitchFactor.Evaluate (Mathf.Abs (horizontal));

			walking = rollFactor.Evaluate (Input.GetAxis ("VerticalL"));
		}
	}
}
