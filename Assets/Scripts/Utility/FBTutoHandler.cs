using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBTutoHandler : MonoBehaviour {
	public GameObject stairs;
	public FBFire trolleyFire;
	public FBHittable trolley;

	// Use this for initialization
	void Start () {
		FBGlobals.ResetCursor ();
		trolleyFire.OnPutOut += EnableTrolley;
		trolley.OnDestroyed += EnableStairs;
	}

	private void EnableTrolley (GameObject go) {
		trolley.destructible = true;
	}

	private void EnableStairs (GameObject go) {
		if (stairs)
			stairs.SetActive (true);
	}
}
