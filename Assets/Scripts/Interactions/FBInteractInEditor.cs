﻿#if UNITY_EDITOR
using UnityEngine;

public class FBInteractInEditor : MonoBehaviour {
	public FBFire fire {
		get;
		set;
	}
	public FBHittable hittable {
		get;
		set;
	}
	public FBButton button {
		get;
		set;
	}
	public FBDoor door {
		get;
		set;
	}
}
#endif