using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;

public class PhoneSensor : MonoBehaviour {

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

	void Start () {
		sp = new SerialPort ("COM" + comNum, 9600);
	}

	void parseValues (string av) {
		//*
		string[] split = av.Split (',');
		if (split.Length >= 3) {
			sensorAxis.x = (invert[1] ? -1.0f : 1.0f) * float.Parse (split[1]);//x=y (but should be x=-y)
			sensorAxis.y = (invert[2] ? -1.0f : 1.0f) * float.Parse (split[2]);//y=z
			sensorAxis.z = (invert[3] ? -1.0f : 1.0f) * float.Parse (split[0]);//z=-x
			sensorAxis *= angleCorrection;
			if (!calibrated) {
				calibratedRotationE = sensorAxis;
				calibrated = true;
				//transform.localRotation = Quaternion.Euler (sensorAxis * 180.0f);
				//calibratedRotationQ = Quaternion.FromToRotation (transform.forward, Vector3.forward);
				//transform.GetChild (0).rotation = Quaternion.Euler (Vector3.zero);
			}

			{// steering
				sensorAxis.y -= calibratedRotationE.y;
				if (sensorAxis.y > 180.0f)
					sensorAxis.y -= 360.0f;
				if (sensorAxis.y < -180.0f)
					sensorAxis.y += 360.0f;
				//float steering = steeringCurve.Evaluate (Mathf.Abs (sensorAxis.y) / steeringMax) * Mathf.Sign (sensorAxis.y);
				float steering = steeringCurve.Evaluate (Mathf.Abs (Input.GetAxis ("HorizontalL")) / steeringMax) * Mathf.Sign (Input.GetAxis ("HorizontalL"));
				steering *= Time.fixedDeltaTime * steeringTurnRate;
				//yRotation += steering;
				//Debug.Log (sensorAxis.y);
			}// end of steering

			transform.localRotation = Quaternion.Euler (sensorAxis.x, yRotation, sensorAxis.z);
			//Debug.Log ("rotation set");
		}
		/*/
		Debug.Log (av);
		//*/
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
			} catch (TimeoutException e) {
				//Debug.Log ("nothing to do here...");
			}
		}
	}

	void connect () {
		Debug.Log ("Connection started");
		try {
			sp.Open ();
			//sp.ReadTimeout = (int) (Time.fixedDeltaTime * 1000.0f);
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

	void Update () {
		float steering = steeringCurve.Evaluate (Mathf.Abs (Input.GetAxis ("HorizontalL"))) * Mathf.Sign (Input.GetAxis ("HorizontalL"));
		steering *= Time.deltaTime * steeringTurnRate;
		//Debug.Log (Input.GetAxis ("HorizontalL").ToString ("F3") + " ;" + yRotation.ToString ("F3") + " ; " + steering.ToString ("F3") + " ; " + steeringCurve.Evaluate (Mathf.Abs (Input.GetAxis ("HorizontalL"))).ToString ("F3"));
		yRotation += steering;
		if (Input.GetKeyDown ("x")) {
			Debug.Log ("Connection establishing...");
			connect ();
		}
		if (connected)
			recData ();
		if (Input.GetKeyDown ("w")) {
			Debug.Log ("Closing port...");
			connected = false;
			try {
				sp.Close ();
				Debug.Log ("Port Closed!");
			} catch (SystemException e) {
				Debug.Log ("Error closing = " + e.Message);
			}
		}
		if (invert[0])
			target.rotation = Quaternion.Euler (sensorAxis.z, yRotation, sensorAxis.x);
		else
			target.rotation = Quaternion.Euler (sensorAxis.x, yRotation, sensorAxis.z);
	}

	private void OnDestroy () {
		connected = false;
		Debug.Log ("Closing port...");
		try {
			sp.Close ();
			Debug.Log ("Port Closed!");
		} catch (SystemException e) {
			Debug.Log ("Error closing = " + e.Message);
		}
	}
}