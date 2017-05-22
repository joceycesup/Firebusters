using UnityEngine;

public class FBStuffList : MonoBehaviour {
	public GameObject[] stuff;
#if !UNITY_EDITOR
	void Awake () {
			Destroy (this);
	}
#endif
}
