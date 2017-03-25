using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Threading;

public class PhoneSensor : MonoBehaviour {

    [HideInInspector]
    public Vector3 sensorAxis;

    private SerialPort sp;
	private int bytes;
	private Thread serialThread;

	void Start () {
		sp = new SerialPort("COM6", 9600);
	}

	void parseValues(string av) {

		string[] split = av.Split (',');
	    sensorAxis.x = float.Parse (split [2]);
		sensorAxis.y = float.Parse (split [3]);
		sensorAxis.z = float.Parse(split[4]);
	}

	
	 void recData() {
        if ((sp != null) && (sp.IsOpen)) {
			byte tmp;
			string data = "";
			string avalues="";
			tmp = (byte) sp.ReadByte();
			while(tmp !=255) {
				data+=((char)tmp);
				tmp = (byte) sp.ReadByte();
				if((tmp=='>') && (data.Length > 30)){
					avalues = data;
					parseValues(avalues);
					data="";
				}
			}
		}
	}


	void connect() {
		Debug.Log ("Connection started");
		try {
			sp.Open();
			sp.ReadTimeout = 400;
			sp.Handshake = Handshake.None;
			serialThread = new Thread(recData);
			serialThread.Start ();
			Debug.Log("Port Opened!");
		}catch (SystemException e)
		{
			Debug.Log ("Error opening = "+e.Message);
		}
	
	}


	void Update () { 

		 if (Input.GetKeyDown ("x"))
        {
			Debug.Log("Connection establishing...");
			connect ();
		}

        if (Input.GetKeyDown("w"))
        {
            sp.Close();
            Debug.Log("Port Closed!");
        }

	}


}