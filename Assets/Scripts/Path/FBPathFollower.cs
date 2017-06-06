using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBPathFollower : MonoBehaviour {
	public FBWaypoint currentWaypoint;
	public float speed = 1.0f;
	public int precision = 5;

	void Start () {
		if (currentWaypoint && currentWaypoint.paths.Count <= 0) {
			currentWaypoint.OnOpenPath += FollowPath;
		}
	}

	void FollowPath (FBPath path) {

	}
}
