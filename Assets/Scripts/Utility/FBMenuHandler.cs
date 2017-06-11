using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent (typeof (Canvas))]
public class FBMenuHandler : MonoBehaviour {
	private enum State {
		Menu,
		Play,
		Credits
	}

	private Canvas canvas;
	public RectTransform creditsPanel;
	public RectTransform playPanel;
	public RectTransform menuPanel;
	public Camera cam;
	public Transform menuCamPosition;
	public Transform playCamPosition;
	public Transform creditsCamPosition;
	public float slideDelay = 1.0f;
	public FBButton menuCButton;
	public FBButton menuPButton;
	public FBButton playButton;
	public FBButton creditsButton;
	public FBButton quitButton;
	public FBButton startButton;
	private State state = State.Menu;

	public GameObject LouisPaired;
	public GameObject LouisNotPaired;
	public GameObject MariusPaired;
	public GameObject MariusNotPaired;

	private AsyncOperation gameLoading;
	public Image loadImage;
	public float minLoadRotation = 0.0f;
	public float maxLoadRotation = 360.0f;

	void Start () {
		AkSoundEngine.PostEvent ("Play_MusicMenu", FBGlobalSoundManager.instance);
		canvas = GetComponent<Canvas> ();
		playPanel.position = creditsPanel.position = new Vector2 (0.0f, canvas.pixelRect.height);
		cam.transform.position = menuCamPosition.position;
		cam.transform.rotation = menuCamPosition.rotation;
		LouisPaired.SetActive (false);
		MariusPaired.SetActive (false);
	}

	private void Update () {
		if (FBPhonesContainer.sensors[0].connected) {
			LouisNotPaired.SetActive (false);
			LouisPaired.SetActive (true);
		}
		if (FBPhonesContainer.sensors[1].connected) {
			MariusNotPaired.SetActive (false);
			MariusPaired.SetActive (true);
		}
		if (state == State.Play && menuPButton.isActiveAndEnabled && FBPhonesContainer.sensors[0].connected && FBPhonesContainer.sensors[1].connected) {
			startButton.enabled = true;
		}
	}

	public void ShowMenu () {
		menuCButton.enabled = false;
		menuPButton.enabled = false;
		startButton.enabled = false;
		StartCoroutine (SlidePanel (state == State.Credits ? creditsPanel : playPanel, new Vector2 (0.0f, 0.0f), new Vector2 (0.0f, canvas.pixelRect.height), () => {
			creditsButton.enabled = true;
			playButton.enabled = true;
			quitButton.enabled = true;
		}));
		if (menuPanel)
			StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.0f, -canvas.pixelRect.height), new Vector2 (0.0f, 0.0f), () => { }));
		StartCoroutine (MoveCamera (state == State.Credits ? creditsCamPosition : playCamPosition, menuCamPosition));
		state = State.Menu;
	}

	public void ShowCredits () {
		creditsButton.enabled = false;
		playButton.enabled = false;
		quitButton.enabled = false;
		menuCButton.enabled = false;
		menuPButton.enabled = false;
		startButton.enabled = false;
		StartCoroutine (SlidePanel (creditsPanel, new Vector2 (0.0f, canvas.pixelRect.height), new Vector2 (0.0f, 0.0f), () => {
			menuCButton.enabled = true;
		}));
		if (menuPanel)
			StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.0f, 0), new Vector2 (0.0f, -canvas.pixelRect.height), () => { }));
		StartCoroutine (MoveCamera (menuCamPosition, creditsCamPosition));
		state = State.Credits;
	}

	public void ShowPlay () {
		creditsButton.enabled = false;
		playButton.enabled = false;
		quitButton.enabled = false;
		menuPButton.enabled = false;
		startButton.enabled = false;
		StartCoroutine (SlidePanel (playPanel, new Vector2 (0.0f, canvas.pixelRect.height), new Vector2 (0.0f, 0.0f), () => {
			menuPButton.enabled = true;
			FBPhonesContainer.instance.Connect ();
		}));
		if (menuPanel)
			StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.0f, 0), new Vector2 (0.0f, -canvas.pixelRect.height), () => { }));
		StartCoroutine (MoveCamera (menuCamPosition, playCamPosition));
		state = State.Play;
	}

	public void ExitGame () {
		Application.Quit ();
	}

	public void StartGame () {
		menuPButton.enabled = false;
		startButton.enabled = false;
		gameLoading = SceneManager.LoadSceneAsync (1);
		StartCoroutine (GameLoading ());
	}

	private IEnumerator GameLoading () {
		loadImage.transform.rotation = Quaternion.identity;
		loadImage.transform.parent.gameObject.SetActive (true);
		while (!gameLoading.isDone) {
			loadImage.transform.rotation = Quaternion.Euler (0.0f, 0.0f, Mathf.Lerp (minLoadRotation, maxLoadRotation, gameLoading.progress));
			yield return null;
		}
	}

	private IEnumerator SlidePanel (RectTransform target, Vector2 start, Vector2 end, Action callback) {
		float startTime = Time.time;
		while (Time.time < startTime + slideDelay) {
			//target.pivot = Vector2.Lerp (start, end, (Time.time - startTime) / slideDelay);
			target.position = Vector2.Lerp (start, end, (Time.time - startTime) / slideDelay);
			yield return null;
		}
		target.position = end;
		callback ();
	}

	private IEnumerator MoveCamera (Transform start, Transform end) {
		float startTime = Time.time;
		while (Time.time < startTime + slideDelay) {
			cam.transform.position = Vector3.Slerp (start.position, end.position, (Time.time - startTime) / slideDelay);
			cam.transform.rotation = Quaternion.Slerp (start.rotation, end.rotation, (Time.time - startTime) / slideDelay);
			yield return null;
		}
		cam.transform.position = end.position;
		cam.transform.rotation = end.rotation;
	}

	public void Bullshit () {
		Debug.LogWarning ("Bullshit!");
	}
}
