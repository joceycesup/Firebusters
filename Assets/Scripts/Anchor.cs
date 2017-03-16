using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (LineRenderer))]
public class Anchor : MonoBehaviour {
	private Rigidbody rb;
	private LineRenderer lr;
	public ForceMode mode;
	public Transform stringAnchor;
	[SerializeField]
	[Range(10000f, 100000f)]
	float factor = 10000f;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		lr = GetComponent<LineRenderer> ();
	}

	void Update () {
		lr.SetPosition (0, transform.position);
		lr.SetPosition (1, stringAnchor.position);
	}

	void FixedUpdate () {
		if (stringAnchor) {
			//rb.velocity = Vector3.zero;
			rb.AddForce ((stringAnchor.position - transform.position) * Time.fixedDeltaTime * factor, mode);
		}
		//rb.AddForce(Vector3.up*Input.GetAxis("Vertical")*500);
	}
}
