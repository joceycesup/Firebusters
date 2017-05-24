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

	private GameObject MariusPhoneSafe; //GameObject in which we keep our phone data handlers
	private GameObject LouisPhoneSafe;

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
			//Debug.LogWarning ("Setting phone data handlers");
			DontDestroyOnLoad (MariusPhoneSafe = new GameObject ("MariusPhoneSafe_" + Time.time.ToString ("F3")));
			DontDestroyOnLoad (LouisPhoneSafe = new GameObject ("LouisPhoneSafe_" + Time.time.ToString ("F3")));

			instanciated = true;
		}
		else {
			try {
				MariusMotion.GetSensor ().SetState (MariusPhoneSafe.GetComponent<FBPhoneDataHandler> ());
				Destroy (MariusPhoneSafe.GetComponent<FBPhoneDataHandler> ());
				LouisMotion.GetSensor ().SetState (LouisPhoneSafe.GetComponent<FBPhoneDataHandler> ());
				Destroy (LouisPhoneSafe.GetComponent<FBPhoneDataHandler> ());
			} catch (System.NullReferenceException e) {
			}
		}
	}
	void SetPhoneDataHandlers (Scene s1, Scene s2) {
		SetPhoneDataHandlers ();
	}

	void SavePhoneDataHandlers () {
		if (MariusPhoneSafe != null) {
			//Debug.LogWarning ("Setting safe states on " + gameObject.name);
			MariusPhoneSafe.AddComponent<FBPhoneDataHandler> ().SetState (MariusMotion.GetSensor ());
			MariusMotion.GetSensor ().serialPort = null;
			LouisPhoneSafe.AddComponent<FBPhoneDataHandler> ().SetState (LouisMotion.GetSensor ());
			LouisMotion.GetSensor ().serialPort = null;
		}
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
	/*
	private void OnDestroy () {
		Debug.LogWarning ("Destroying " + gameObject.name);
	}//*/

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
		intFieldID = 0;
		bounds = new Rect (GUI.skin.window.border.left + margin.x, GUI.skin.window.border.top + margin.y, windowRect0.width - margin.x - margin.width - GUI.skin.window.border.horizontal, windowRect0.height - margin.y - margin.height - GUI.skin.window.border.vertical);
		GUILayout.BeginArea (bounds);
		if (!Application.isPlaying)
			GUI.enabled = false;
		if (GUILayout.Button ("Reload scene")) {
			SavePhoneDataHandlers ();
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
		if (!Application.isPlaying)
			GUI.enabled = true;

		Color tmpBGColor;
		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Phone");
		if (!Application.isPlaying)
			GUI.enabled = false;
		if (MariusMotion.GetSensor ().connected == LouisMotion.GetSensor ().connected) {
			if (GUILayout.Button ((MariusMotion.GetSensor ().connected ? "Stop" : "Start") + " Both")) {
				if (MariusMotion.GetSensor ().connected) {
					MariusMotion.GetSensor ().close ();
					LouisMotion.GetSensor ().close ();
				}
				else {
					MariusMotion.GetSensor ().connect ();
					LouisMotion.GetSensor ().connect ();
				}
			}
		}
		if (!Application.isPlaying)
			GUI.enabled = true;
		GUILayout.EndHorizontal ();
		{
			bool tmpEnabled = GUI.enabled;
			GUI.enabled = false;
			GUILayout.BeginVertical (GUI.skin.textArea);
			GUI.enabled = tmpEnabled;
			GUILayout.Label ("Marius");
			tmpBGColor = GUI.backgroundColor;
			GUI.backgroundColor = MariusMotion.GetSensor ().connected ? Color.green : Color.red;
			{
				tmpEnabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.BeginVertical (GUI.skin.textArea);
				GUI.enabled = tmpEnabled;
				MariusMotion.usePhoneDataHandler = GUILayout.Toggle (MariusMotion.usePhoneDataHandler, "Marius use phone");

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Com num : ");
				int tmp = IntField (MariusMotion.GetSensor ().comNum, GUILayout.Width (20));
				if (tmp != MariusMotion.GetSensor ().comNum) {
					MariusMotion.GetSensor ().comNum = tmp;
					if (MariusMotion.GetSensor ().connected) {
						MariusMotion.GetSensor ().close ();
						MariusMotion.GetSensor ().connect ();
					}
				}
				if (GUILayout.Button ((MariusMotion.GetSensor ().connected ? "Stop" : "Start") + " Connection")) {
					if (MariusMotion.GetSensor ().connected)
						MariusMotion.GetSensor ().close ();
					else
						MariusMotion.GetSensor ().connect ();
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
			}
			GUI.backgroundColor = LouisMotion.GetSensor ().connected ? Color.green : Color.red;
			GUILayout.Label ("Louis");
			{
				tmpEnabled = GUI.enabled;
				GUI.enabled = false;
				GUILayout.BeginVertical (GUI.skin.textArea);
				GUI.enabled = tmpEnabled;
				LouisMotion.usePhoneDataHandler = GUILayout.Toggle (LouisMotion.usePhoneDataHandler, "Louis use phone");

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Com num : ");
				int tmp = IntField (LouisMotion.GetSensor ().comNum, GUILayout.Width (20));
				if (tmp != LouisMotion.GetSensor ().comNum) {
					LouisMotion.GetSensor ().comNum = tmp;
					if (LouisMotion.GetSensor ().connected) {
						LouisMotion.GetSensor ().close ();
						LouisMotion.GetSensor ().connect ();
					}
				}
				if (GUILayout.Button ((LouisMotion.GetSensor ().connected ? "Stop" : "Start") + " Connection")) {
					if (LouisMotion.GetSensor ().connected)
						LouisMotion.GetSensor ().close ();
					else
						LouisMotion.GetSensor ().connect ();
				}
				GUILayout.EndHorizontal ();
				GUI.backgroundColor = tmpBGColor;

				GUILayout.EndVertical ();
			}
			GUILayout.EndVertical ();
		}
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
