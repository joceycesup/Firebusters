using UnityEngine;
using UnityEditor;

[RequireComponent (typeof (Rigidbody)), RequireComponent (typeof (Collider)), RequireComponent (typeof (CharacterJoint))]
public class FBPuppetJoint : MonoBehaviour {
	public Rigidbody attachedObject;
	private CharacterJoint cj;
	private CharacterJointValues cjv;
	private Rigidbody rb;
	private Collider collider;
	public float speed = 4.0f;
	public float maxSpeed = 100.0f;
	public float distanceFactor = 1.0f;
	public float impulseFactor = 1.0f;
	[Range (-1.0f, 1.0f)]
	public float dampenGravity = -1.0f;
	Transform anchor;
	Vector3 deltaAnchor = Vector3.zero;
	Vector3 lastVelocity = Vector3.zero;

	private bool characterJointEnabled = true;
	[HideInInspector]
	public float breakSqrDistance = Mathf.Infinity;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		cj = GetComponent<CharacterJoint> ();
		collider = GetComponent<Collider> ();
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
#if DEBUG_ENABLED
		if (characterJointEnabled) {
			//Debug.Log (Vector3.SqrMagnitude (transform.position - anchor.position));
			if (Vector3.SqrMagnitude (transform.position - anchor.position) > breakSqrDistance) {
				cj.breakForce = -1.0f;
				collider.enabled = false;
				characterJointEnabled = false;
			}
			return;
		}
#endif
		deltaAnchor = anchor.position - transform.position;
		rb.velocity += -lastVelocity + (lastVelocity = deltaAnchor.normalized * Mathf.Min ((deltaAnchor.magnitude / distanceFactor) * speed, maxSpeed));
		rb.AddForce (deltaAnchor * impulseFactor, ForceMode.Impulse);
		rb.AddForce (Physics.gravity * rb.mass * dampenGravity);
#if DEBUG_ENABLED
		Debug.DrawLine (attachedObject.position, anchor.position);
		//Debug.Log (lastVelocity.magnitude + " ; " + (deltaAnchor * impulseFactor).magnitude);
#endif
	}

	void OnJointBreak (float breakForce) {
		Debug.Log (name + " has just been broken!, force: " + breakForce);
		characterJointEnabled = false;
	}
}
