using UnityEngine;

public class FBCharacterJointDisabler {
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

		private static SoftJointLimit Lerp (SoftJointLimit sjl1, SoftJointLimit sjl2, float factor) {
			SoftJointLimit res = new SoftJointLimit ();
			res.bounciness = Mathf.Lerp (sjl1.bounciness, sjl2.bounciness, factor);
			res.contactDistance = Mathf.Lerp (sjl1.contactDistance, sjl2.contactDistance, factor);
			res.limit = Mathf.Lerp (sjl1.limit, sjl2.limit, factor);
			return res;
		}
	}

	public GameObject gameObject {
		get;
		private set;
	}
	private Rigidbody ConnectedBody;
	private Vector3 Anchor;
	private Vector3 Axis;
	private bool AutoConfigureConnectedAnchor;
	public Vector3 connectedAnchor {
		get;
		private set;
	}
	private Vector3 SwingAxis;
	public Limits limits {
		get;
		private set;
	}
	private float BreakForce;
	private float BreakTorque;
	private bool EnableCollision;

	public void CopyJointValues (CharacterJoint characterJoint) {
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
	}

	public CharacterJoint CreateJoint (GameObject target = null, Rigidbody body = null) {
		CharacterJoint characterJoint = (target != null ? target : gameObject).AddComponent<CharacterJoint> ();

		characterJoint.connectedBody = body != null ? body : ConnectedBody;
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