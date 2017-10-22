using System.Collections;
using System;
using UnityEngine;

public class FBPuppetPart : MonoBehaviour {
	public Rigidbody rb {
		get;
		private set;
	}
	private CharacterJointValues cjv;
	public CharacterJoint cj {
		get;
		private set;
	}
	private Collider[] colls;

	void Awake () {
		rb = GetComponent<Rigidbody> ();
		colls = GetComponents<Collider> ();
		if ((cj = GetComponent<CharacterJoint> ()) != null)
			cjv = new CharacterJointValues (cj);
		/*
#if DEBUG_ENABLED
		string s = "Puppet part collider types (" + name + ") : ";
		foreach (Collider coll in colls) {
			s += coll.GetType () + ", ";
		}
		Debug.LogWarning (s);
#endif
		//*/
	}

	public void EnableCollider (bool value) {
		foreach (Collider coll in colls) {
			coll.enabled = value;
		}
	}
}
