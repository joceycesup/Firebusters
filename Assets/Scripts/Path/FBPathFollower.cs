using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBPathFollower : MonoBehaviour {
	public FBWaypoint currentWaypoint;
	public float speed = 3.0f;
	public float rotateRate = 180.0f;
	public int precision = 5;
	private float currentT = 0.0f;
	private Quaternion targetRotation = Quaternion.Euler (Vector3.zero);

	void Start () {
		if (currentWaypoint) {
			transform.position = currentWaypoint.transform.position;
			ReachWaypoint (currentWaypoint, null);
		}
	}

	void ReachPath (FBPath path) {
		ReachPath (path, 0.0f);
	}

	void ReachPath (FBPath path, float initialDistance) {
		if (!path.open)
			return;
		if (currentWaypoint == path.start) {
			StartCoroutine (FollowPath (path, true, initialDistance));
		}
		else if (currentWaypoint == path.end) {
			StartCoroutine (FollowPath (path, false, initialDistance));
		}
#if UNITY_EDITOR
		else {
			Debug.LogWarning ("Follower is not on a valid path");
		}
#endif
	}

	void ReachWaypoint (FBWaypoint wp, FBPath from, float initialDistance = 0.0f) {
		//Debug.Log ("Reached waypoint " + wp.ToString ());
		currentWaypoint = wp;

		FBPath path = currentWaypoint.GetNextPath (from);

		if (path == null)
			currentWaypoint.OnOpenPath += ReachPath;
		else {
			ReachPath (path, initialDistance);
		}
	}

	IEnumerator FollowPath (FBPath on, bool forward = true, float initialDistance = 0.0f) {
		//Debug.Log ("Following path " + on.Name);
		bool overflow = false;
		float res = 0.0f;
		float distance = initialDistance;
		currentT = forward ? 0.0f : 1.0f;

		while (!overflow) {
			distance += speed * Time.deltaTime;
			res = on.spline.GetT (currentT, distance, precision, out overflow, forward);
			distance = 0.0f;
			if (!overflow) {
				currentT = res;
				transform.position = on.spline.GetPoint (currentT) + on.transform.position;
				Vector3 velocity = on.spline.GetVelocity (currentT);
				if (!forward)
					velocity *= -1.0f;
				if (Vector3.Angle (transform.forward, velocity) < rotateRate * Time.deltaTime) {
					transform.rotation = Quaternion.LookRotation (velocity);
				}
				else {
					transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (velocity), rotateRate * Time.deltaTime);
				}
			}
			yield return null;
		}
		ReachWaypoint (forward ? on.end : on.start, on, res);
	}
}
