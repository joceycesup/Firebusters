using UnityEngine;
using UnityEngine.SceneManagement;

public class FBPause : MonoBehaviour {
	private static GameObject instance;
	private float fixedDeltaTime;

	private void Awake () {
		fixedDeltaTime = Time.fixedDeltaTime;
		if (instance == null)
			instance = gameObject;
		Debug.LogWarning ("FBPause initialized");
		gameObject.SetActive (false);
	}

	private void Update () {
		if (Input.GetKeyDown (KeyCode.Escape))
			ExitPause ();
	}

	private void OnEnable () {
		FBGlobals.SetCursorLayer (FBGlobals.CursorLayer.Pause, true);
		Time.fixedDeltaTime = 0.0f;
		Time.timeScale = 0.0f;
	}

	private void OnDisable () {
		FBGlobals.SetCursorLayer (FBGlobals.CursorLayer.Pause, false);
		Time.fixedDeltaTime = fixedDeltaTime;
		Time.timeScale = 1.0f;
	}

	public void ExitPause () {
		gameObject.SetActive (false);
	}

	public void GotoMenu () {
		SceneManager.LoadSceneAsync (0);
	}

	public static void Pause () {
		if (instance != null)
			instance.SetActive (true);
	}
}
