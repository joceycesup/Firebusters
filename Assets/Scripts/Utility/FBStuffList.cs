using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBStuffList : MonoBehaviour {
	public bool destroyOnStart = false;
	public GameObject[] stuff;

	void Start () {
		if (destroyOnStart)
			Destroy (this);
	}
}
