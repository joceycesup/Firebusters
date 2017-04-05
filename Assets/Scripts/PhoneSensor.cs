﻿using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;

public class PhoneSensor : MonoBehaviour {

	public int comNum = 1;

	//[HideInInspector]
	public Vector3 calibratedRotationE;
	public Quaternion calibratedRotationQ;
	private bool calibrated = false;
	public Vector3 sensorAxis;

	private SerialPort sp;
	private int bytes;
	private Thread serialThread;

	private bool connected = false;

	public Transform target;

	private float angleCorrection = -180.0f;

	void Start () {
		sp = new SerialPort ("COM" + comNum, 9600);
	}

	void parseValues (string av) {
		string[] split = av.Split (',');
		if (split.Length == 5) {
			sensorAxis.x = float.Parse (split[3]);//x=y (but should be x=-y)
			sensorAxis.y = float.Parse (split[4]);//y=z
			sensorAxis.z = -float.Parse (split[2]);//z=-x
			sensorAxis *= angleCorrection;
			if (!calibrated) {
				calibratedRotationQ = Quaternion.Inverse (Quaternion.Euler (sensorAxis.x, 0.0f, sensorAxis.z));
				calibratedRotationE = sensorAxis;
				calibrated = true;
				//transform.localRotation = Quaternion.Euler (sensorAxis * 180.0f);
				//calibratedRotationQ = Quaternion.FromToRotation (transform.forward, Vector3.forward);
				//transform.GetChild (0).rotation = Quaternion.Euler (Vector3.zero);
			}
			//*
			//sensorAxis.y += 180.0f;
			transform.localRotation = Quaternion.Euler (sensorAxis.x, 0.0f, sensorAxis.z);
			//target.rotation = calibratedRotationQ * transform.localRotation;
			target.rotation = Quaternion.Euler (sensorAxis.x, 0.0f, sensorAxis.z);
			//Debug.Log ("rotation set");
			/*/
			sensorAxis -= calibratedRotationE;
			transform.localRotation = Quaternion.Euler (sensorAxis * -180.0f);//*/
		}
	}


	void recData () {
		if ((sp != null) && (sp.IsOpen)) {
			try {
				byte tmp;
				string data = "";
				string avalues = "";
				tmp = (byte) sp.ReadByte ();//ignore '>' character
				do {
					tmp = (byte) sp.ReadByte ();
					//Debug.Log (((char) tmp) + " ; " + ((int)tmp));
					if ((tmp == 10)) {// && (data.Length > 30)) {
						avalues = data;
						//parseValues (avalues);
						data = "";
					}
					data += ((char) tmp);
				} while (tmp != 10 && tmp != 255);
				//Debug.Log ("final : " + avalues);
				parseValues (avalues);
			} catch (TimeoutException e) {
				//Debug.Log ("nothing to read");
				//connected = false;
				//Debug.Log (e.Message);
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