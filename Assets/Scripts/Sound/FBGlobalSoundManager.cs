using UnityEngine;

public class FBGlobalSoundManager : MonoBehaviour {
	private static FBGlobalSoundManager _instance;
	public static GameObject instance {
		get { return _instance.gameObject; }
	}
	public AnimationCurve _puppetSoundCurve;
	public static AnimationCurve puppetSoundCurve { get { return _instance._puppetSoundCurve; } }
	public float _puppetMaxVelocity;
	public static float puppetMaxVelocity { get { return _instance._puppetMaxVelocity; } }

	void Awake () {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Destroy (gameObject);
		}
	}
}
