using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

	private SerialPort sp;
	private Thread serialThread;

	private bool connected = false;

	private UdpClient _client;
	private IPEndPoint _sender;

	void Start () {
		sp = new SerialPort ("COM" + comNum, 9600);

		IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 1234);
		_client = new UdpClient(ipep);

		_sender = new IPEndPoint(IPAddress.Any, 0);

		Debug.Log ("Connection establishing...");
		connect ();
	}

	void parseValues (string h, string av) {
		string[] split = av.Split (',');
		//Debug.Log (av);
		if (split.Length >= 3) {
			Debug.Log(av);
			if (h.Equals("OR")) {
				_orientation.x = float.Parse (split[2]);
				_orientation.y = float.Parse (split[0]);
				_orientation.z = -float.Parse (split[1]);
				if (_orientation.y > 180.0f)
					_orientation.y -= 360.0f;
				UpdateRotation ();
				UpdateAcceleration ();
			}
			else if (h.Equals("AC")) {
				_acceleration.x = -float.Parse (split[2]);
				_acceleration.y = float.Parse (split[0]);
				_acceleration.z = float.Parse (split[1]);
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
		/*
		if ((sp != null) && (sp.IsOpen)) {
			try {
				byte tmp;
				string data = "";
				string avalues = "";
				string head = "";
				do {
					tmp = (byte) sp.ReadByte ();
					if ((tmp == 62)) {
						head = data;
						data = "";
					}
					else if (tmp == 10) {
						avalues = data;
						data = "";
					}
					else {
						data += ((char) tmp);
					}
				} while (tmp != 10 && tmp != 255);
				parseValues (head, avalues);
			} catch (TimeoutException) {
				Debug.Log ("FBPhoneDataHandler : reached timeout");
			}
		}
		*/

		byte[] data = _client.Receive(ref _sender);

		string message = System.Text.Encoding.UTF8.GetString(data);

		if (message.Equals("FB_PAIRING"))
		{
			string welcome = "Client Paired !";

			byte[] toSend = Encoding.ASCII.GetBytes(welcome);

			_client.Send(toSend, toSend.Length, _sender);
		}
		else
		{
			string[] bits = message.Split('>');
			if(bits.Length != 2)
			{
				Debug.Log("ERROR : stream message from client of incorrect size !");
				return;
			}
			string head = bits[0];
			string values = bits[1];
			parseValues(head, values);
		}
	}

	void connect () {
		Debug.Log ("Connection started");
		/*
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
			Debug.Log ("Port " + comNum + " Opened!");
		} catch (SystemException e) {
			Debug.Log ("Error opening " + comNum + " = " + e.Message);
		}*/
		connected = true;
		serialThread = new Thread (ReadData);
		serialThread.Start ();
	}

	void close () {
		Debug.Log ("Closing port...");

		connected = false;
		try {
			_client.Close();
			sp.Close ();
			Debug.Log ("Port Closed!");
		} catch (SystemException e) {
			Debug.Log ("Error closing = " + e.Message);
		}
	}

	void Update () {
		/*
		if (Input.GetKeyDown ("x")) {
			Debug.Log ("Connection establishing...");
			connect ();
		}
		if (Input.GetKeyDown ("w")) {
			close ();
		}
		*/
	}

	private void OnDestroy () {
		close ();
	}
}
