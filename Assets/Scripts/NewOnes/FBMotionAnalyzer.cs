using UnityEngine;

[RequireComponent (typeof (FBPhoneDataHandler))]
public class FBMotionAnalyzer : MonoBehaviour {

	public enum State {
		Inactive,
		Walk,
		Aim,
		Strike
	}

	public State state = State.Walk;

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
		switch (state) {
			case State.Inactive:
				break;
			case State.Aim:
				break;
			case State.Walk:
				UpdateWalkValues ();
				goto case State.Strike;
			case State.Strike:
				break;
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
