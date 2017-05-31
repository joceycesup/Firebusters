using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class FBPlaytestWindow : MonoBehaviour {
	public Rect windowRect0 = new Rect (20, 20, 300, 200);
	private bool showWindow = true;
	public Rect margin = Rect.zero;

	private FBMotionAnalyzer MariusMotion;
	private FBMotionAnalyzer LouisMotion;

	private Rect bounds;

	private string intFieldFocus = "";
	private string intFieldValue = "";

	private static int intFieldID = 0;
	private static bool instanciated = false;

	void Awake () {
#if !PLAYTEST
		Destroy (this);
#endif
	}

	void SetPhoneDataHandlers () {
		//Debug.LogWarning ("SetPhoneDataHandlers " + instanciated);
		FBMotionAnalyzer[] motions = FindObjectsOfType<FBMotionAnalyzer> ();
		foreach (FBMotionAnalyzer motion in motions) {
			if (motion.isAxePuppet)
				MariusMotion = motion;
			else
				LouisMotion = motion;
		}
		if (!instanciated) {
			instanciated = true;
		}
	}
	void SetPhoneDataHandlers (Scene s1, Scene s2) {
		SetPhoneDataHandlers ();
	}

#if PLAYTEST
	private void OnEnable () {
		SceneManager.activeSceneChanged += SetPhoneDataHandlers;
		if (Application.isPlaying) {
			if (!instanciated) {
				gameObject.name = "FBPlaytestsWindow_" + Time.time.ToString ("F3");
				DontDestroyOnLoad (gameObject);
			}
			else
				Destroy (gameObject);
		}
	}

	private void OnDisable () {
		SceneManager.activeSceneChanged -= SetPhoneDataHandlers;
	}

	private void Start () {
		//Debug.LogWarning ("Start " + Application.isPlaying);
		if (!Application.isPlaying)
			SetPhoneDataHandlers ();
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
	void DoMyWindow (int windowID) {
		intFieldID = 0;
		bounds = new Rect (GUI.skin.window.border.left + margin.x, GUI.skin.window.border.top + margin.y, windowRect0.width - margin.x - margin.width - GUI.skin.window.border.horizontal, windowRect0.height - margin.y - margin.height - GUI.skin.window.border.vertical);
		GUILayout.BeginArea (bounds);

		if (Application.isEditor)
			GUI.enabled = false;
		if (GUILayout.Button ("Exit Game"))
			Application.Quit ();
		if (Application.isEditor)
			GUI.enabled = true;

		if (!Application.isPlaying)
			GUI.enabled = false;
		if (GUILayout.Button ("Reload scene")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		if (!Application.isPlaying)
			GUI.enabled = true;

		Color tmpBGColor;
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Phone");
		if (Application.isPlaying) {
			if (MariusMotion.sensor.connected == LouisMotion.sensor.connected) {
				if (GUILayout.Button ((MariusMotion.sensor.connected ? "Stop" : "Start") + " Both")) {
					if (MariusMotion.sensor.connected) {
						MariusMotion.sensor.close ();
						LouisMotion.sensor.close ();
					}
					else {
						MariusMotion.sensor.connect ();
						LouisMotion.sensor.connect ();
					}
				}
			}
		}
		else {
			GUI.enabled = false;
			GUILayout.Button ("Start Both");
			GUI.enabled = true;
		}
		GUILayout.EndHorizontal ();

		if (Application.isPlaying) {
			bool tmpEnabled = GUI.enabled;
			GUI.enabled = false;
			GUILayout.BeginVertical (GUI.skin.textArea);
			GUI.enabled = tmpEnabled;
			GUILayout.Label ("Marius");
			tmpBGColor = GUI.backgroundColor;
			GUI.backgroundColor = MariusMotion.sensor.connected ? Color.green : Color.red;
			{
				tmpEnabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.BeginVertical (GUI.skin.textArea);
				GUI.enabled = tmpEnabled;
				MariusMotion.usePhoneDataHandler = GUILayout.Toggle (MariusMotion.usePhoneDataHandler, "Marius use phone");

				GUILayout.BeginHorizontal ();
#if SENSODUINO
				GUILayout.Label ("COM : ");
				int tmp = IntField (MariusMotion.sensor.comNum, GUILayout.Width (20));
				if (tmp != MariusMotion.sensor.comNum) {
					MariusMotion.sensor.comNum = tmp;
#else
				GUILayout.Label ("ID : ");
				int tmp = IntField (MariusMotion.sensor.id, GUILayout.Width (20));
				if (tmp != MariusMotion.sensor.id) {
					MariusMotion.sensor.id = tmp;
#endif
					if (MariusMotion.sensor.connected) {
						MariusMotion.sensor.close ();
						MariusMotion.sensor.connect ();
					}
				}
				if (GUILayout.Button ((MariusMotion.sensor.connected ? "Stop" : "Start") + " Connection")) {
					if (MariusMotion.sensor.connected)
						MariusMotion.sensor.close ();
					else
						MariusMotion.sensor.connect ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
			}
			GUI.backgroundColor = LouisMotion.sensor.connected ? Color.green : Color.red;
			GUILayout.Label ("Louis");
			{
				tmpEnabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.BeginVertical (GUI.skin.textArea);
				GUI.enabled = tmpEnabled;
				LouisMotion.usePhoneDataHandler = GUILayout.Toggle (LouisMotion.usePhoneDataHandler, "Louis use phone");

				GUILayout.BeginHorizontal ();
#if SENSODUINO
				GUILayout.Label ("COM : ");
				int tmp = IntField (LouisMotion.sensor.comNum, GUILayout.Width (20));
				if (tmp != LouisMotion.sensor.comNum) {
					LouisMotion.sensor.comNum = tmp;
#else
				GUILayout.Label ("ID : ");
				int tmp = IntField (LouisMotion.sensor.id, GUILayout.Width (20));
				if (tmp != LouisMotion.sensor.id) {
					LouisMotion.sensor.id = tmp;
#endif
					if (LouisMotion.sensor.connected) {
						LouisMotion.sensor.close ();
						LouisMotion.sensor.connect ();
					}
				}
				if (GUILayout.Button ((LouisMotion.sensor.connected ? "Stop" : "Start") + " Connection")) {
					if (LouisMotion.sensor.connected)
						LouisMotion.sensor.close ();
					else
						LouisMotion.sensor.connect ();
				}
				GUILayout.EndHorizontal ();
				GUI.backgroundColor = tmpBGColor;

				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
		}

		GUILayout.Label ("Displays : " + Display.displays.Length);

		GUILayout.EndArea ();
		GUI.DragWindow ();
	}

	int IntField (int src, params GUILayoutOption[] options) {
		string id = "IntField" + intFieldID;
		GUI.SetNextControlName (id);
		bool hasFocus = (GUI.GetNameOfFocusedControl ().CompareTo (id) == 0);
		if (hasFocus && GUI.GetNameOfFocusedControl ().CompareTo (intFieldFocus) != 0) {
			intFieldValue = src.ToString ();
			intFieldFocus = id;
		}
		string srcStr = hasFocus ? intFieldValue : src.ToString ();
		int parse = 0;
		string intStr = GUILayout.TextField (srcStr, options);
		if (intStr.CompareTo (srcStr) != 0) {
			if (intStr.Length == 0 || int.TryParse (intStr, out parse) && hasFocus) {
				intFieldValue = intStr;
			}
		}
		if (Event.current.isKey && Event.current.keyCode == KeyCode.Return) {
			if (int.TryParse (intFieldValue, out parse) && hasFocus) {
				src = parse;
				intFieldFocus = "";
				GUIUtility.keyboardControl = 0;
				Event.current.Use ();
			}
		}

		intFieldID++;
		return src;
	}//*/
	 /*
	 garder la connection pdt le reload
	 voir si on a le retour du portable
	 afficher le port com

	 //*/
}
