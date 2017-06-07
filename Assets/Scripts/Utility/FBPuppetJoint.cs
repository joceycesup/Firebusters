using UnityEngine;
using System;
using System.Collections;

[RequireComponent (typeof (Rigidbody)), RequireComponent (typeof (Collider))]
public class FBPuppetJoint : MonoBehaviour {
	private Rigidbody attachedObject;
	private CharacterJoint cj;
	private CharacterJointValues cjv;
	private Rigidbody rb;
	public float speed = 4.0f;
	public float maxSpeed = 100.0f;
	public float distanceFactor = 1.0f;
	public float impulseFactor = 1.0f;
	[Range (-1.0f, 1.0f)]
	public float dampenGravity = 0.0f;
	Transform anchor;
	Vector3 deltaAnchor = Vector3.zero;
	Vector3 lastVelocity = Vector3.zero;

	private bool characterJointEnabled = true;
	public float rebindDelay = 0.5f;
	[HideInInspector]
	public float breakSqrDistance = Mathf.Infinity;
	[HideInInspector]
	public float rebindSqrDistance = Mathf.Epsilon;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		cj = GetComponent<CharacterJoint> ();
		attachedObject = cj.connectedBody;
		cjv = new CharacterJointValues (cj);
		anchor = new GameObject (name + "_anchor_" + attachedObject.name).transform;
		anchor.position = transform.position;
		anchor.SetParent (attachedObject.transform);
	}

	void FixedUpdate () {
#if UNITY_EDITOR
		if (maxSpeed < 0.0001f)
			maxSpeed = 0.0001f;
		if (speed < 0.0001f)
			speed = 0.0001f;
		if (distanceFactor < 0.0001f)
			distanceFactor = 0.0001f;
		if (impulseFactor < 0.0001f)
			impulseFactor = 0.0001f;
		if (impulseFactor > 10.0f)
			impulseFactor = 10.0f;
#endif
		float sqrDistance = Vector3.SqrMagnitude (transform.position - anchor.position);
		if (characterJointEnabled) {
			//Debug.Log (Vector3.SqrMagnitude (transform.position - anchor.position));
			if (sqrDistance > breakSqrDistance) {
				BreakJoint ();
			}
		}
		else if (cj == null) {
			if (sqrDistance <= rebindSqrDistance)
				Rebind ();
			else {
				deltaAnchor = anchor.position - transform.position;
				rb.velocity += -lastVelocity + (lastVelocity = deltaAnchor.normalized * Mathf.Min ((deltaAnchor.magnitude / distanceFactor) * speed, maxSpeed));
				rb.AddForce (deltaAnchor * impulseFactor, ForceMode.Impulse);
				rb.AddForce (Physics.gravity * rb.mass * dampenGravity);
#if DEBUG_ENABLED
				Debug.DrawLine (attachedObject.position, anchor.position);
				//Debug.Log (lastVelocity.magnitude + " ; " + (deltaAnchor * impulseFactor).magnitude);
#endif
			}
		}
	}

	void OnJointBreak (float breakForce) {
		//Debug.Log (name + " has just been broken!, force: " + breakForce);
		BreakJoint ();
	}

	void BreakJoint () {
		//#if DEBUG_ENABLED
		//		Debug.Log ("Breaking joint " + name);
		//#endif
		rb.velocity = Vector3.zero;
		Destroy (cj);
		//collider.enabled = false;
		characterJointEnabled = false;
	}

	void Rebind () {
		//#if DEBUG_ENABLED
		//Debug.Log ("Rebinding joint " + name);
		//#endif
		Quaternion tmpRot = transform.rotation;
		transform.rotation = attachedObject.transform.rotation * cjv.relativeRotation;
		cj = cjv.CreateJoint (gameObject, attachedObject);
		transform.rotation = tmpRot;
		//collider.enabled = true;

		float rebindSpeed = Vector3.Distance (cj.connectedAnchor, cjv.connectedAnchor) / rebindDelay;
		StartCoroutine (DoItAfter ((dt) => {
			float factor = (rebindSpeed * dt) / Vector3.Distance (cj.connectedAnchor, cjv.connectedAnchor);
			cjv.Lerp (cj, factor);
		}, () => {
			characterJointEnabled = true;
		}, rebindDelay));
	}

	private IEnumerator DoItAfter (Action<float> a, Action callback, float delay = 0.0f) {
		float endTime = Time.time + delay;
		StartCoroutine (DoWhileThen (
			(dt) => { return Time.time < endTime; },
			a,
			callback
			));
		yield return null;
	}

	private IEnumerator DoWhileThen (Func<float, bool> predicate, Action<float> a, Action callback) {
		do {
			a (Time.deltaTime);
			yield return null;
		} while (predicate (Time.deltaTime));
		callback ();
	}
}
