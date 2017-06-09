using System;
using System.Collections;
using UnityEngine;

public class FBMenuHandler : MonoBehaviour {
	private enum State {
		Menu,
		Play,
		Credits
	}

	public Canvas canvas;
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

	void Start () {
		creditsPanel.pivot = new Vector2 (0.5f, -1.0f);
		cam.transform.position = menuCamPosition.position;
		cam.transform.rotation = menuCamPosition.rotation;
		menuCButton.enabled = false;
		menuPButton.enabled = false;
		startButton.enabled = false;
	}

	public void ShowCredits () {
		creditsButton.enabled = false;
		playButton.enabled = false;
		quitButton.enabled = false;
		StartCoroutine (SlidePanel (creditsPanel, new Vector2 (0.5f, -1.0f), new Vector2 (0.5f, 0.0f), () => {
			menuCButton.enabled = true;
		}));
		if (menuPanel)
			StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.5f, 1.0f), new Vector2 (0.5f, 2.0f), () => { }));
		StartCoroutine (MoveCamera (menuCamPosition, creditsCamPosition));
		state = State.Credits;
	}

	public void ShowMenu () {
		menuCButton.enabled = false;
		menuPButton.enabled = false;
		startButton.enabled = false;
		StartCoroutine (SlidePanel (state == State.Credits ? creditsPanel : playPanel, new Vector2 (0.5f, 0.0f), new Vector2 (0.5f, -1.0f), () => {
			creditsButton.enabled = true;
			playButton.enabled = true;
			quitButton.enabled = true;
		}));
		if (menuPanel)
			StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.5f, 2.0f), new Vector2 (0.5f, 1.0f), () => { }));
		StartCoroutine (MoveCamera (state == State.Credits ? creditsCamPosition : playCamPosition, menuCamPosition));
		state = State.Menu;
	}

	public void ShowPlay () {
		creditsButton.enabled = false;
		playButton.enabled = false;
		quitButton.enabled = false;
		StartCoroutine (SlidePanel (playPanel, new Vector2 (0.5f, 0.0f), new Vector2 (0.5f, -1.0f), () => {
			menuPButton.enabled = true;
		}));
		StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.5f, 2.0f), new Vector2 (0.5f, 1.0f), () => { }));
		StartCoroutine (MoveCamera (menuCamPosition, playCamPosition));
		state = State.Play;
	}

	public void ExitGame () {
		Application.Quit ();
	}

	public IEnumerator SlidePanel (RectTransform target, Vector2 start, Vector2 end, Action callback) {
		float startTime = Time.time;
		while (Time.time < startTime + slideDelay) {
			target.pivot = Vector2.Lerp (start, end, (Time.time - startTime) / slideDelay);
			yield return null;
		}
		target.pivot = end;
		callback ();
	}

	public IEnumerator MoveCamera (Transform start, Transform end) {
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
