using UnityEngine;

public class FBShatterable: FBHittable {

	public override void Hit (Collision collision = null) {
		base.Hit (collision);
		if (collision == null || collision.gameObject.layer == 8)
			DestroyHittable ();
	}
}
