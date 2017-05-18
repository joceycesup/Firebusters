using UnityEngine;

public static class ColliderExtension {

	public static Collider[] OverlapBox (this BoxCollider bc, int layerMask = Physics.AllLayers) {
		return Physics.OverlapBox (bc.transform.rotation * Vector3.Scale (bc.center, bc.transform.lossyScale) + bc.transform.position, Vector3.Scale (bc.size / 2.0f, bc.transform.lossyScale), bc.transform.rotation, layerMask);
	}

	public static Collider[] OverlapSphere (this SphereCollider sc, int layerMask = Physics.AllLayers) {
		return Physics.OverlapSphere (sc.transform.rotation * Vector3.Scale (sc.center, sc.transform.lossyScale) + sc.transform.position, sc.radius * sc.transform.lossyScale.x, layerMask);
	}
}