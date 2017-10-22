using UnityEngine;

public class FBGlobals : MonoBehaviour {
	private static FBGlobals instance;

	public enum CursorLayer {
		All = -1,
		Menu = 0x02,
		Pause = 0x04,
		DebugPanel = 0x08
	}

	private static int cursor = 0;

	public static void SetCursorLayer (CursorLayer layer, bool value) {
		SetCursorLayer ((int) layer, value);
	}

	private static void SetCursorLayer (int layer, bool value) {
		if (value)
			cursor |= layer;
		else
			cursor &= ~layer;
		Cursor.visible = cursor != 0;
	}

	public static void ResetCursor (CursorLayer layer) {
		ResetCursor ();
		SetCursorLayer (layer, true);
	}

	public static void ResetCursor () {
		SetCursorLayer (CursorLayer.All, false);
	}

	void OnEnable () {
		Debug.Log ((int) CursorLayer.All);
		Debug.Log ((int) CursorLayer.Menu);
		Debug.Log ((int) CursorLayer.Pause);
		Debug.Log ((int) CursorLayer.DebugPanel);
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else
			Destroy (gameObject);
	}
}
