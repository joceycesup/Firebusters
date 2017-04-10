using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;

public class FBPhoneDataHandler : MonoBehaviour {

	public int comNum = 1;

	//[HideInInspector]
	public Vector3 calibratedRotationE;
	private bool calibrated = false;
	public Vector3 sensorAxis;

	private SerialPort sp;
	private int bytes;
	private Thread serialThread;

	private bool connected = false;

	public Transform target;

	private float angleCorrection = 180.0f;

	public bool[] invert = { false, false, false, false };

	public float steeringMax = 45.0f;
	public AnimationCurve steeringCurve;
	public float steeringTurnRate = 45.0f;
	private float yRotation = 0.0f;

	private float lastDataReceivedTime;

	private bool readingData = false;

	void Start () {
		sp = new SerialPort ("COM" + comNum, 9600);
	}

	void parseValues (string av) {
		string[] split = av.Split (',');
		Debug.Log (av);
		if (split.Length >= 5) {
			if (split[0].CompareTo ("11") == 0) {
				sensorAxis.x = (invert[1] ? -1.0f : 1.0f) * float.Parse (split[3]);
				sensorAxis.y = (invert[2] ? -1.0f : 1.0f) * float.Parse (split[4]);
				sensorAxis.z = (invert[3] ? -1.0f : 1.0f) * float.Parse (split[2]);
				sensorAxis *= angleCorrection;
				if (!calibrated) {
					calibratedRotationE = sensorAxis;
					calibrated = true;
				}

				{// steering
					sensorAxis.y -= calibratedRotationE.y;
					if (sensorAxis.y > 180.0f)
						sensorAxis.y -= 360.0f;
					if (sensorAxis.y < -180.0f)
						sensorAxis.y += 360.0f;
					float steering = steeringCurve.Evaluate (Mathf.Abs (Input.GetAxis ("HorizontalL")) / steeringMax) * Mathf.Sign (Input.GetAxis ("HorizontalL"));
					steering *= Time.fixedDeltaTime * steeringTurnRate;
				}// end of steering

				transform.localRotation = Quaternion.Euler (sensorAxis.x, yRotation, sensorAxis.z);
			}
			else if (split[0].CompareTo ("3") == 0) {
				sensorAxis.x = (invert[1] ? -1.0f : 1.0f) * float.Parse (split[3]);
				sensorAxis.y = (invert[2] ? -1.0f : 1.0f) * float.Parse (split[2]);
				sensorAxis.z = (invert[3] ? -1.0f : 1.0f) * float.Parse (split[4]);
				if (!calibrated) {
					calibratedRotationE = sensorAxis;
					calibrated = true;
				}

				{// steering
					sensorAxis.y -= calibratedRotationE.y;
					if (sensorAxis.y > 180.0f)
						sensorAxis.y -= 360.0f;
					float steering = steeringCurve.Evaluate (Mathf.Abs (sensorAxis.y) / steeringMax) * Mathf.Sign (sensorAxis.y);
					//float steering = steeringCurve.Evaluate (Mathf.Abs (Input.GetAxis ("HorizontalL")) / steeringMax) * Mathf.Sign (Input.GetAxis ("HorizontalL"));
					steering *= Time.fixedDeltaTime * steeringTurnRate;
					yRotation += steering;
				}// end of steering

				transform.localRotation = Quaternion.Euler (sensorAxis.x, yRotation, sensorAxis.z);
			}
		}
	}

	private IEnumerator ReadData () {
		Debug.Log ("FBPhoneDataHandler : reading data...");
		recData ();
		readingData = false;
		yield return null;
	}

	void recData () {
		if ((sp != null) && (sp.IsOpen)) {
			try {
				byte tmp;
				string data = "";
				string avalues = "";
				tmp = (byte) sp.ReadByte ();
				do {
					tmp = (byte) sp.ReadByte ();
					if ((tmp == 10)) {
						avalues = data;
						data = "";
					}
					data += ((char) tmp);
				} while (tmp != 10 && tmp != 255);
				parseValues (avalues);
				Debug.Log ("FBPhoneDataHandler : parsed data");
			} catch (TimeoutException e) {
				Debug.Log ("FBPhoneDataHandler : reached timeout");
			}/*
			Debug.Log (Time.time - lastDataReceivedTime);
			lastDataReceivedTime = Time.time;//*/
		}
	}

	void connect () {
		Debug.Log ("Connection started");
		if (sp.PortName.CompareTo ("COM" + comNum) != 0) {
			sp = new SerialPort ("COM" + comNum, 9600);
		}
		try {
			sp.Open ();
			sp.ReadTimeout = 400;
			sp.Handshake = Handshake.None;
			serialThread = new Thread (recData);
			serialThread.Start ();
			Debug.Log ("Port Opened!");
			connected = true;
		} catch (SystemException e) {
			Debug.Log ("Error opening = " + e.Message);
		}
	}

	void close () {
		Debug.Log ("Closing port...");
		connected = false;
		try {
			sp.Close ();
			Debug.Log ("Port Closed!");
		} catch (SystemException e) {
			Debug.Log ("Error closing = " + e.Message);
		}
	}

	void Update () {
		float steering = steeringCurve.Evaluate (Mathf.Abs (Input.GetAxis ("HorizontalL"))) * Mathf.Sign (Input.GetAxis ("HorizontalL"));
		steering *= Time.deltaTime * steeringTurnRate;
		yRotation += steering;
		if (Input.GetKeyDown ("x")) {
			Debug.Log ("Connection establishing...");
			connect ();
		}
		if (connected) {
			if (readingData) {
				Debug.Log ("FBPhoneDataHandler : skip read");
			}
			else {
				readingData = true;
				UnityMainThreadDispatcher.Instance ().Enqueue (ReadData ());
			}
		}
		if (Input.GetKeyDown ("w")) {
			close ();
		}
		if (invert[0])
			target.rotation = Quaternion.Euler (sensorAxis.z, yRotation, sensorAxis.x);
		else
			target.rotation = Quaternion.Euler (sensorAxis.x, yRotation, sensorAxis.z);
	}

	private void OnDestroy () {
		close ();
	}
}
