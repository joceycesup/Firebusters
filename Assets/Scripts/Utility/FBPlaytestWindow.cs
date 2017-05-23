using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class FBPlaytestWindow : MonoBehaviour {
	public Rect windowRect0 = new Rect (20, 20, 300, 200);
	private bool showWindow = true;
	private bool connected = false;
	public Rect margin = Rect.zero;
	public Vector2 indent = Vector2.zero;
	public float gap = 5.0f;
	public Texture boxBorder;

	private FBMotionAnalyzer MariusMotion;
	private FBMotionAnalyzer LouisMotion;

	private Rect bounds;
	private Stack<Rect> rectBuffer = new Stack<Rect> ();
	private void PushBounds () { rectBuffer.Push (bounds); }
	private void PopBounds () { bounds = rectBuffer.Pop (); }
	private Rect GetBounds (float height, bool offsetH = true) {
		Rect res = bounds;
		res.height = height;
		if (offsetH) {
			bounds.y += height + gap;
			bounds.height -= height + gap;
		}
		return res;
	}
	void BeginBox (float height, string label = "") {
		if (label != null && label.Length > 0)
			GUI.Label (GetBounds (20.0f), label);
		bool tmp = GUI.enabled;
		GUI.enabled = false;
		GUI.Box (GetBounds (height), GUIContent.none, GUI.skin.textArea);
		GUI.enabled = tmp;
		PushBounds ();
		bounds = new Rect (bounds.x + indent.x, bounds.y - height + gap, bounds.width - indent.y - indent.x, height - 2.0f * gap);
	}
	void EndBox () { PopBounds (); }

	void Awake () {
#if PLAYTEST
		connected = false;
#else
		Destroy (this);
#endif
	}

#if PLAYTEST
	void OnEnable () {
		FBMotionAnalyzer[] motions = FindObjectsOfType<FBMotionAnalyzer> ();
		foreach (FBMotionAnalyzer motion in motions) {
			if (motion.isAxePuppet)
				MariusMotion = motion;
			else
				LouisMotion = motion;
		}
	}
#endif

	void Update () {
		if (Input.GetKeyDown ("m")) {
			showWindow = !showWindow;
		}
		Cursor.visible = showWindow;
	}

	void OnGUI () {
		if (showWindow) {
			windowRect0 = GUI.Window (0, windowRect0, DoMyWindow, "Playtests controls");
		}
	}
	/*
	void DoMyWindow (int windowID) {
		bounds = new Rect (GUI.skin.window.border.left + margin.x, GUI.skin.window.border.top + margin.y, windowRect0.width - margin.x - margin.width - GUI.skin.window.border.horizontal, windowRect0.height - margin.y - margin.height - GUI.skin.window.border.vertical);
		if (!Application.isPlaying)
			GUI.enabled = false;
		if (GUI.Button (GetBounds (20.0f), "Reload scene")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		if (GUI.Button (GetBounds (20.0f), (connected ? "Stop" : "Start") + " Connection")) {
			if (connected) {
				MariusMotion.sensor.close ();
				LouisMotion.sensor.close ();
			}
			else {
				MariusMotion.sensor.connect ();
				LouisMotion.sensor.connect ();
			}
			connected = !connected;
		}
		if (!Application.isPlaying)
			GUI.enabled = true;

		{
			BeginBox (80.0f, "Phone :");
			MariusMotion.usePhoneDataHandler = GUI.Toggle (GetBounds (20.0f), MariusMotion.usePhoneDataHandler, "Marius use phone");
			LouisMotion.usePhoneDataHandler = GUI.Toggle (GetBounds (20.0f), LouisMotion.usePhoneDataHandler, "Louis use phone");
			EndBox ();
		}

		GUI.DragWindow ();
	}/*/
	void DoMyWindow (int windowID) {
		bounds = new Rect (GUI.skin.window.border.left + margin.x, GUI.skin.window.border.top + margin.y, windowRect0.width - margin.x - margin.width - GUI.skin.window.border.horizontal, windowRect0.height - margin.y - margin.height - GUI.skin.window.border.vertical);
		GUILayout.BeginArea (bounds);
		if (!Application.isPlaying)
			GUI.enabled = false;
		if (GUILayout.Button ("Reload scene")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		if (GUILayout.Button ((connected ? "Stop" : "Start") + " Connection")) {
			if (connected) {
				MariusMotion.sensor.close ();
				LouisMotion.sensor.close ();
			}
			else {
				MariusMotion.sensor.connect ();
				LouisMotion.sensor.connect ();
			}
			connected = !connected;
		}
		if (!Application.isPlaying)
			GUI.enabled = true;

		GUILayout.Label ("Marius");
		{
			bool tmp = GUI.enabled;
			GUI.enabled = false;
			GUILayout.BeginVertical (GUI.skin.textArea);
			GUI.enabled = tmp;
			MariusMotion.usePhoneDataHandler = GUILayout.Toggle (MariusMotion.usePhoneDataHandler, "Marius use phone");
			GUILayout.EndVertical ();
		}
		GUILayout.Label ("Louis");
		{
			bool tmp = GUI.enabled;
			GUI.enabled = false;
			GUILayout.BeginVertical (GUI.skin.textArea);
			GUI.enabled = tmp;
			LouisMotion.usePhoneDataHandler = GUILayout.Toggle (LouisMotion.usePhoneDataHandler, "Louis use phone");
			/*
			string intStr = GUILayout.TextField (LouisMotion.sensor.comNum.ToString ());
			if (intStr.CompareTo (LouisMotion.sensor.comNum.ToString ()) != 0) {
				int res = 0;
				if (int.TryParse (intStr, out res)) {
					LouisMotion.sensor.comNum = res;
				}
			}//*/

			GUILayout.EndVertical ();
		}
		GUILayout.EndArea ();
		GUI.DragWindow ();
	}//*/
	/*
	garder la connection pdt le reload
	voir si on a le retour du portable
	afficher le port com

	//*/
}
