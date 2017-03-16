using UnityEngine;
using System.Collections;

[RequireComponent(typeof (LineRenderer))]
public class PuppetString : MonoBehaviour {
	private LineRenderer lr;
	public GameObject attachedObject;
	private Rigidbody rb;
	public float speed = 4;
	public float length {
		get;
		private set;
	}
	[SerializeField]
	[Range(0f, 100000f)]
	float springConstant = 10000f;
	[SerializeField]
	[Range(0f, 100000f)]
	float viscousDampingCoefficient = 10000f;

	private float relaxedLength;
	private Vector3 lastPos;

	//public string controllerExtension = "L";

	void Start () {
		if (!attachedObject)
			return;
		rb = attachedObject.GetComponent<Rigidbody> ();
		lr = GetComponent<LineRenderer> ();
		relaxedLength = Vector3.Magnitude (transform.position - attachedObject.transform.position);
	}

	void Update () {
		if (!attachedObject)
			return;
		lr.SetPosition (0, transform.position);
		lr.SetPosition (1, attachedObject.transform.position);
	}
	
	void FixedUpdate () {
		if (rb) {
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
				lr.SetColors (Color.red, Color.red);
			} else {
				lr.SetColors (Color.green, Color.green);
			}
		}
		lastPos = attachedObject.transform.position;
	}
}
