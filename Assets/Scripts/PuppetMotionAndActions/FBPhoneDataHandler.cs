﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;

public class FBPhoneDataHandler : MonoBehaviour {
	public int comNum = 4;

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
	private Vector3 _cleanAcceleration;
	public Vector3 cleanAcceleration {
		get { return _cleanAcceleration; }
		private set { _cleanAcceleration = value; }
	}
	private Quaternion accRotation;
	private Vector3 accForward;
	private Vector3 accUp;
	private Vector3 accRight;

	private void UpdateAcceleration () {
		float theoreticalYLength = 9.81f * Mathf.Cos (Vector3.Angle (Vector3.up, accUp) * Mathf.Deg2Rad);
		float theoreticalXLength = 9.81f * Mathf.Cos (Vector3.Angle (accRight.y >= 0.0f ? Vector3.up : Vector3.down, accRight) * Mathf.Deg2Rad);
		float theoreticalZLength = 9.81f * Mathf.Cos (Vector3.Angle (accForward.y >= 0.0f ? Vector3.up : Vector3.down, accForward) * Mathf.Deg2Rad);
		cleanAcceleration = acceleration - new Vector3 (theoreticalXLength, theoreticalYLength, theoreticalZLength);
		//Debug.Log (cleanAcceleration);
	}
	private void UpdateRotation () {
		accRotation = Quaternion.Euler (_orientation);
		accUp = accRotation * Vector3.up;
		accForward = accRotation * Vector3.forward;
		accRight = accRotation * Vector3.right;
	}

	public SerialPort serialPort {
		get;
		set;
	}
	private Thread serialThread;

	public bool connected {
		get;
		private set;
	}

	private void OnEnable () {
		connected = false;
	}

	void Start () {
		serialPort = new SerialPort ("COM" + comNum, 9600);
	}

	void parseValues (string av) {
		string[] split = av.Split (',');
		//Debug.Log (av);
		if (split.Length >= 5) {
			if (split[0].CompareTo ("3") == 0) {
				_orientation.x = float.Parse (split[4]);
				_orientation.y = float.Parse (split[2]);
				_orientation.z = -float.Parse (split[3]);
				if (_orientation.y > 180.0f)
					_orientation.y -= 360.0f;
				UpdateRotation ();
				UpdateAcceleration ();
			}
			else if (split[0].CompareTo ("1") == 0) {
				_acceleration.x = -float.Parse (split[3]);
				_acceleration.y = float.Parse (split[4]);
				_acceleration.z = float.Parse (split[2]);
				UpdateAcceleration ();
			}
		}
	}

	// this method is executed in a new Thread
	private void ReadData () {
		while (connected) {
			recData ();
		}
		Debug.Log ("ReadData Thread completed");
	}

	void recData () {
		if ((serialPort != null) && (serialPort.IsOpen)) {
			try {
				byte tmp;
				string data = "";
				string avalues = "";
				tmp = (byte) serialPort.ReadByte ();
				do {
					tmp = (byte) serialPort.ReadByte ();
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

	public void connect () {
		Debug.Log ("Connection started");
		if (serialPort.PortName.CompareTo ("COM" + comNum) != 0) {
			serialPort = new SerialPort ("COM" + comNum, 9600);
		}
		try {
			serialPort.Open ();
			serialPort.ReadTimeout = 400;
			serialPort.Handshake = Handshake.None;
			connected = true;
			serialThread = new Thread (ReadData);
			serialThread.Start ();
			Debug.Log ("Port " + comNum + " Opened!");
		} catch (SystemException e) {
			Debug.Log ("Error opening " + comNum + " = " + e.Message);
		}
	}

	public void close () {
		Debug.Log ("Closing port...");
		connected = false;
		try {
			serialPort.Close ();
			Debug.Log ("Port Closed!");
		} catch (SystemException e) {
			Debug.Log ("Error closing = " + e.Message);
		}
	}

	public FBPhoneDataHandler SetState (FBPhoneDataHandler other) {
		if (other != this) {
			serialPort = other.serialPort;
			comNum = other.comNum;
			serialThread = other.serialThread;
			connected = other.connected;
		}
		return this;
	}

	private void OnDestroy () {
		close ();
	}
}
