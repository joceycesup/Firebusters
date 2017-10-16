using UnityEngine;
using System.Collections;

public class PuppetString : MonoBehaviour {
	public GameObject attachedObject;
	private Rigidbody rb;
	public float speed = 4;
	public float length {
		get;
		private set;
	}
	[SerializeField]
	[Range (0f, 50000f)]
	float springConstant = 10000f;
	[SerializeField]
	[Range (0f, 50000f)]
	float viscousDampingCoefficient = 10000f;

	public float relaxedLength = -1.0f;
	private Vector3 lastPos;

	public bool absoluteAnchor = false;

	//public string controllerExtension = "L";

	void Start () {
		if (!attachedObject)
			return;
		rb = attachedObject.GetComponent<Rigidbody> ();
#if !UNITY_EDITOR
		LineRenderer lr = GetComponent<LineRenderer> ();
		if (lr != null)
			Destroy (lr);
		MeshRenderer mr = GetComponent<MeshRenderer> ();
		if (mr != null)
			mr.enabled = false;
#endif
		if (absoluteAnchor)
			relaxedLength = 0.0f;
		else if (relaxedLength <= 0.0f)
			relaxedLength = Vector3.Magnitude (transform.position - attachedObject.transform.position);
	}

	void FixedUpdate () {
		if (!attachedObject)
			return;
		if (rb) {
			if (absoluteAnchor) {
				attachedObject.transform.position = transform.position;
			}
			else {
				//rb.velocity = Vector3.zero;
				/*
				acceleration = (rb.velocity - lastVelocity) / Time.fixedDeltaTime;
				lastVelocity = rb.velocity;//*/
				float magnitude = Vector3.Magnitude (transform.position - attachedObject.transform.position);
				if (magnitude > relaxedLength) {
					//spring
					rb.AddForce (Vector3.Normalize (transform.position - attachedObject.transform.position) * Time.fixedDeltaTime * springConstant * (magnitude - relaxedLength), ForceMode.Force);
					//dampening
					if (viscousDampingCoefficient > 0f) {
						rb.AddForce (-viscousDampingCoefficient * Vector3.Project (attachedObject.transform.position - lastPos, transform.position - attachedObject.transform.position) * Time.fixedDeltaTime, ForceMode.Force);
					}
#if UNITY_EDITOR
					Debug.DrawLine (transform.position, attachedObject.transform.position, new Color (1.0f, 0.1f, 0.1f, 0.5f));
#endif
				}
				else {
#if UNITY_EDITOR
					Debug.DrawLine (transform.position, attachedObject.transform.position, new Color (0.1f, 1.0f, 0.1f, 0.5f));
#endif
				}
			}
		}
		lastPos = attachedObject.transform.position;
	}
}
