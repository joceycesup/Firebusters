﻿using UnityEngine;

public class FBPhonesContainer : MonoBehaviour {
	private static FBPhonesContainer _instance;
	public static FBPhonesContainer instance {
		get { return _instance; }
		private set { _instance = value; }
	}

	private FBPhoneDataHandler[] _sensors;
	public static FBPhoneDataHandler[] sensors {
		get { return instance._sensors; }
		private set { instance._sensors = value; }
	}

	void OnEnable () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else
			Destroy (gameObject);
	}

	private void Start () {
		sensors = new FBPhoneDataHandler[2];
		for (int i = 0; i < sensors.Length; ++i) {
			sensors[i] = gameObject.AddComponent<FBPhoneDataHandler> ();
#if SENSODUINO
			sensors[i].comNum = 4 + i;
#else
			sensors[i].id = i;
#endif
		}
	}

	public void Connect () {
		for (int i = 0; i < sensors.Length; ++i) {
			if (sensors[i].connected)
				return;
			sensors[i].connect ();
		}
	}
}
