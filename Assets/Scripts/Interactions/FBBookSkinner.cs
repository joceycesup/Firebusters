using UnityEngine;

public class FBBookSkinner : MonoBehaviour {

	private void Start () {
		GetComponent<SkinnedMeshRenderer> ().sharedMaterial = FBBookMaterials.Get ();
		Destroy (this);
	}
}
