using UnityEngine;

public class FBGlobalSoundManager : MonoBehaviour {
	private static GameObject _instance;
	public static GameObject instance {
		get;
		private set;
	}

	void Awake () {
		if (instance == null) {
			instance = gameObject;
		}
		else {
			Destroy (gameObject);
		}
	}
}
