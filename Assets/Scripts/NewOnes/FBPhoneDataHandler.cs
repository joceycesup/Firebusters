using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;

public class FBPhoneDataHandler : MonoBehaviour {
	public int comNum = 4;

	private Vector3 calibratedRotationE;
	private bool calibrated = false;
	private Vector3 _orientation;
	public Vector3 orientation {
		get { return _orientation; }
		private set { _orientation = value; }
	}
	private Vector3 _acceleration;
	public Vector3 acceleration {
		get { return _acceleration; }
		private set { _acceleration = value; }
	}

	private SerialPort sp;
	private Thread serialThread;

	private bool connected = false;

	void Start () {
		sp = new SerialPort ("COM" + comNum, 9600);
	}

	void parseValues (string av) {
		string[] split = av.Split (',');
		//Debug.Log (av);
		if (split.Length >= 5) {
			if (split[0].CompareTo ("3") == 0) {
				_orientation.x = float.Parse (split[4]);
				_orientation.y = float.Parse (split[2]);
				_orientation.z = -float.Parse (split[3]);
				if (!calibrated) {
					calibratedRotationE = _orientation;
					if (calibratedRotationE.y > 180.0f)
						calibratedRotationE.y -= 360.0f;
					calibrated = true;
				}

				{// steering
					_orientation.y -= calibratedRotationE.y;
					if (_orientation.y > 180.0f)
						_orientation.y -= 360.0f;
				}// end of steering
			}else if (split[0].CompareTo ("1") == 0) {
				_acceleration.x = -float.Parse (split[3]);
				_acceleration.y = float.Parse (split[4]);
				_acceleration.z = float.Parse (split[2]);
			}
		}
	}

	private void ReadData () {
		while (connected) {
			recData ();
		}
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
			} catch (TimeoutException) {
				//Debug.Log ("FBPhoneDataHandler : reached timeout");
			}
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
			connected = true;
			serialThread = new Thread (ReadData);
			serialThread.Start ();
			Debug.Log ("Port Opened!");
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
		if (Input.GetKeyDown ("x")) {
			Debug.Log ("Connection establishing...");
			connect ();
		}
		if (Input.GetKeyDown ("w")) {
			close ();
		}
	}

	private void OnDestroy () {
		close ();
	}
}
