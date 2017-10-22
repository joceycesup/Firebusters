using System.Collections;
using System;
using UnityEngine;

public class FBCoroutinesLibrary {

	public static IEnumerator DoItLater (Action callback, float delay = 0.0f) {
		yield return new WaitForSeconds (delay);
		callback ();
	}

	public static IEnumerator DoItAfter (Action<float> a, Action callback, float delay = 0.0f) {
		float endTime = Time.time + delay;
		do {
			a (Time.deltaTime);
			yield return null;
		} while (Time.time < endTime);
		callback ();
	}

	public static IEnumerator DoWhileThen (Func<float, bool> predicate, Action<float> a, Action callback) {
		do {
			a (Time.deltaTime);
			yield return null;
		} while (predicate (Time.deltaTime));
		callback ();
	}
}
