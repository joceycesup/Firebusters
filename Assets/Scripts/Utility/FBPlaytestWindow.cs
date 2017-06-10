using UnityEngine;
using System;
using System.Collections;
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
		showWindow = false;
	}
#endif

	void Update () {
#if KONAMI
		KonamiCode ("bite", () => {
			showWindow = !showWindow;
		});
		KonamiCode ("llz", () => {
			SceneManager.LoadSceneAsync (0);
		});
		KonamiCode ("llu", () => {
			SceneManager.LoadSceneAsync (1);
		});
#else
		if (Input.GetKeyDown ("m")) {
			showWindow = !showWindow;
		}
#endif
		Cursor.visible = MariusMotion == null ? true : showWindow;
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
			if (FBPhonesContainer.sensors[1].connected == FBPhonesContainer.sensors[0].connected) {
				if (GUILayout.Button ((FBPhonesContainer.sensors[1].connected ? "Stop" : "Start") + " Both")) {
					if (FBPhonesContainer.sensors[1].connected) {
						FBPhonesContainer.sensors[1].close ();
						FBPhonesContainer.sensors[0].close ();
					}
					else {
						FBPhonesContainer.sensors[1].connect ();
						FBPhonesContainer.sensors[0].connect ();
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
			GUI.backgroundColor = FBPhonesContainer.sensors[1].connected ? Color.green : Color.red;
			{
				tmpEnabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.BeginVertical (GUI.skin.textArea);
				GUI.enabled = tmpEnabled;
				if (MariusMotion)
					MariusMotion.usePhoneDataHandler = GUILayout.Toggle (MariusMotion.usePhoneDataHandler, "Marius use phone");

				GUILayout.BeginHorizontal ();
#if SENSODUINO
				GUILayout.Label ("COM : ");
				int tmp = IntField (FBPhonesContainer.sensors[1].comNum, GUILayout.Width (20));
				if (tmp != FBPhonesContainer.sensors[1].comNum) {
					FBPhonesContainer.sensors[1].comNum = tmp;
#else
				GUILayout.Label ("ID : ");
				int tmp = IntField (FBPhonesContainer.sensors[1].id, GUILayout.Width (20));
				if (tmp != FBPhonesContainer.sensors[1].id) {
					FBPhonesContainer.sensors[1].id = tmp;
#endif
					if (FBPhonesContainer.sensors[1].connected) {
						FBPhonesContainer.sensors[1].close ();
						FBPhonesContainer.sensors[1].connect ();
					}
				}
				if (GUILayout.Button ((FBPhonesContainer.sensors[1].connected ? "Stop" : "Start") + " Connection")) {
					if (FBPhonesContainer.sensors[1].connected)
						FBPhonesContainer.sensors[1].close ();
					else
						FBPhonesContainer.sensors[1].connect ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
			}
			GUI.backgroundColor = FBPhonesContainer.sensors[0].connected ? Color.green : Color.red;
			GUILayout.Label ("Louis");
			{
				tmpEnabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.BeginVertical (GUI.skin.textArea);
				GUI.enabled = tmpEnabled;
				if (LouisMotion)
					LouisMotion.usePhoneDataHandler = GUILayout.Toggle (LouisMotion.usePhoneDataHandler, "Louis use phone");

				GUILayout.BeginHorizontal ();
#if SENSODUINO
				GUILayout.Label ("COM : ");
				int tmp = IntField (FBPhonesContainer.sensors[0].comNum, GUILayout.Width (20));
				if (tmp != FBPhonesContainer.sensors[0].comNum) {
					FBPhonesContainer.sensors[0].comNum = tmp;
#else
				GUILayout.Label ("ID : ");
				int tmp = IntField (FBPhonesContainer.sensors[0].id, GUILayout.Width (20));
				if (tmp != FBPhonesContainer.sensors[0].id) {
					FBPhonesContainer.sensors[0].id = tmp;
#endif
					if (FBPhonesContainer.sensors[0].connected) {
						FBPhonesContainer.sensors[0].close ();
						FBPhonesContainer.sensors[0].connect ();
					}
				}
				if (GUILayout.Button ((FBPhonesContainer.sensors[0].connected ? "Stop" : "Start") + " Connection")) {
					if (FBPhonesContainer.sensors[0].connected)
						FBPhonesContainer.sensors[0].close ();
					else
						FBPhonesContainer.sensors[0].connect ();
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

#if KONAMI
	private void KonamiCode (string code, Action callback, float timeBetweenKeys = 0.5f) {
		if (Input.GetKeyDown (code.Substring (0, 1))) {
			StartCoroutine (CKonamiCode (code, callback, timeBetweenKeys));
		}
	}
	private IEnumerator CKonamiCode (string code, Action callback, float timeBetweenKeys = 0.5f) {
		float endTime = Time.time + timeBetweenKeys;
		int currentKey = 1;
		while (Time.time <= endTime) {
			if (currentKey >= code.Length)
				break;
			if (Input.GetKeyDown (code.Substring (currentKey, 1))) {
				currentKey++;
				endTime = Time.time + timeBetweenKeys;
			}
			yield return null;
		}
		if (currentKey >= code.Length)
			callback ();
	}
#endif
}
