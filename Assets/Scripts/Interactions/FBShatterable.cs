using UnityEngine;

public class FBShatterable : FBHittable {
	public bool dispersible = false;

	public override void Hit (Collision collision = null) {
		base.Hit (collision);
		if (collision == null || collision.gameObject.layer == 8 || dispersible)
			DestroyHittable ();
	}

	public void Disperse (Vector3 force = new Vector3 (), ForceMode forceMode = ForceMode.Impulse) {
		DestroyHittable (force, forceMode);
	}

#if UNITY_EDITOR
	public override string ToString () {
		return "FBShatterable " + name + " (is " + (dispersible ? "" : "not") + " dispersible) : ";
	}
#endif
}
