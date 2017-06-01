using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class FBPuppetJoint : MonoBehaviour {
	public Rigidbody attachedObject;
	private Rigidbody rb;
	public float speed = 4.0f;
	private float distanceFactor = 1.0f;
	public AnimationCurve resetPosSpeedMultiplier = AnimationCurve.EaseInOut (0.0f, 0.0f, 1.0f, 1.0f);
	Transform anchor;
	Vector3 deltaAnchor = Vector3.zero;
	Vector3 lastVelocity = Vector3.zero;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		anchor = new GameObject (name + "_anchor_" + attachedObject.name).transform;
		anchor.position = transform.position;
		anchor.SetParent (attachedObject.transform);
	}

	void FixedUpdate () {
		Debug.DrawLine (attachedObject.position, anchor.position);
		deltaAnchor = anchor.position - transform.position;
		float factor = deltaAnchor.magnitude;
		factor = (factor * factor) / distanceFactor;
		Debug.Log (lastVelocity.magnitude);
		rb.velocity += -lastVelocity + (lastVelocity = deltaAnchor.normalized * (factor * speed));
	}
}
