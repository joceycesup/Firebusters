using System;
using UnityEngine;

[Serializable]
public struct Limits {
	SoftJointLimit lowTwistLimit;
	SoftJointLimit highTwistLimit;
	SoftJointLimit swing1Limit;
	SoftJointLimit swing2Limit;

	public Limits (CharacterJoint cj) {
		lowTwistLimit = cj.lowTwistLimit;
		highTwistLimit = cj.highTwistLimit;
		swing1Limit = cj.swing1Limit;
		swing2Limit = cj.swing2Limit;
	}

	public Limits (Limits other) {
		lowTwistLimit = other.lowTwistLimit;
		highTwistLimit = other.highTwistLimit;
		swing1Limit = other.swing1Limit;
		swing2Limit = other.swing2Limit;
	}

	public void Apply (CharacterJoint cj) {
		cj.lowTwistLimit = lowTwistLimit;
		cj.highTwistLimit = highTwistLimit;
		cj.swing1Limit = swing1Limit;
		cj.swing2Limit = swing2Limit;
	}

	public static Limits Lerp (Limits l1, Limits l2, float factor) {
		Limits res = new Limits (l1);
		res.highTwistLimit = Lerp (l1.highTwistLimit, l2.highTwistLimit, factor);
		res.lowTwistLimit = Lerp (l1.lowTwistLimit, l2.lowTwistLimit, factor);
		res.swing1Limit = Lerp (l1.swing1Limit, l2.swing1Limit, factor);
		res.swing2Limit = Lerp (l1.swing2Limit, l2.swing2Limit, factor);
		return res;
	}

	public static Limits Lerp (CharacterJoint cj, Limits l2, float factor) {
		Limits res = new Limits (cj);
		res.highTwistLimit = Lerp (res.highTwistLimit, l2.highTwistLimit, factor);
		res.lowTwistLimit = Lerp (res.lowTwistLimit, l2.lowTwistLimit, factor);
		res.swing1Limit = Lerp (res.swing1Limit, l2.swing1Limit, factor);
		res.swing2Limit = Lerp (res.swing2Limit, l2.swing2Limit, factor);
		return res;
	}

	private static SoftJointLimit Lerp (SoftJointLimit sjl1, SoftJointLimit sjl2, float factor) {
		SoftJointLimit res = new SoftJointLimit ();
		res.bounciness = Mathf.Lerp (sjl1.bounciness, sjl2.bounciness, factor);
		res.contactDistance = Mathf.Lerp (sjl1.contactDistance, sjl2.contactDistance, factor);
		res.limit = Mathf.Lerp (sjl1.limit, sjl2.limit, factor);
		return res;
	}
}

[Serializable]
public struct CharacterJointValues {
	public GameObject gameObject; // GameObject sur lequel est applique le CharacterJoint
	public Rigidbody ConnectedBody;
	public Vector3 Anchor;
	public Vector3 Axis;
	public bool AutoConfigureConnectedAnchor;
	public Vector3 connectedAnchor;
	public Vector3 SwingAxis;
	public Limits limits;
	public float BreakForce;
	public float BreakTorque;
	public bool EnableCollision;
	public CharacterJoint characterJoint;
	public Quaternion relativeRotation;

	public CharacterJointValues (CharacterJoint cj) {
		//Debug.Log ("Creating CJV on " + cj.gameObject);

		characterJoint = cj;
		gameObject = characterJoint.gameObject;
		ConnectedBody = characterJoint.connectedBody;
		Anchor = characterJoint.anchor;
		Axis = characterJoint.axis;
		AutoConfigureConnectedAnchor = characterJoint.autoConfigureConnectedAnchor;
		connectedAnchor = characterJoint.connectedAnchor;
		SwingAxis = characterJoint.swingAxis;
		limits = new Limits (characterJoint);
		BreakForce = characterJoint.breakForce;
		BreakTorque = characterJoint.breakTorque;
		EnableCollision = characterJoint.enableCollision;

		//relativeRotation = Quaternion.FromToRotation (characterJoint.transform.forward, ConnectedBody.transform.forward);
		relativeRotation = Quaternion.Inverse (ConnectedBody.transform.rotation) * characterJoint.transform.rotation;
	}

	public CharacterJoint Apply (CharacterJoint cj) {
		cj.connectedBody = ConnectedBody;
		cj.anchor = Anchor;
		cj.axis = Axis;
		cj.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
		cj.connectedAnchor = connectedAnchor;
		cj.swingAxis = SwingAxis;
		cj.breakForce = BreakForce;
		cj.breakTorque = BreakTorque;
		cj.enableCollision = EnableCollision;
		limits.Apply (cj);

		return cj;
	}

	public void Lerp (CharacterJoint cj, float factor) {
		cj.connectedAnchor = Vector3.Lerp (cj.connectedAnchor, connectedAnchor, factor);
		Limits.Lerp (cj, limits, factor).Apply (cj);
	}

	public CharacterJoint CreateJoint (GameObject target, Rigidbody body) {
		CharacterJoint characterJoint = target.AddComponent<CharacterJoint> ();

		characterJoint.connectedBody = body;
		characterJoint.anchor = Anchor;
		characterJoint.axis = Axis;
		characterJoint.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
		//characterJoint.connectedAnchor = ConnectedAnchor;
		characterJoint.swingAxis = SwingAxis;
		characterJoint.breakForce = BreakForce;
		characterJoint.breakTorque = BreakTorque;
		characterJoint.enableCollision = EnableCollision;

		return characterJoint;
	}
}