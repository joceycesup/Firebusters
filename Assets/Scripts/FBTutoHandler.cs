using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBTutoHandler : MonoBehaviour {
	public GameObject stairs;
	public FBFire trolleyFire;
	public FBHittable trolley;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		trolleyFire.OnPutOut += EnableTrolley;
		trolley.OnDestroyed += EnableStairs;
	}

	private void EnableTrolley () {
		trolley.destructible = true;
	}

	private void EnableStairs () {
		stairs.SetActive (true);
	}
}
