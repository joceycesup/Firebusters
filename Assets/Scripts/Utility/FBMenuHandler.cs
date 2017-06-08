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
	private State state = State.Menu;

	void Start () {
		creditsPanel.pivot = new Vector2 (0.5f, -1.0f);
		cam.transform.position = menuCamPosition.position;
		cam.transform.rotation = menuCamPosition.rotation;
	}

	public void ShowCredits () {
		StartCoroutine (SlidePanel (creditsPanel, new Vector2 (0.5f, -1.0f), new Vector2 (0.5f, 0.0f)));
		StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.5f, 1.0f), new Vector2 (0.5f, 2.0f)));
		StartCoroutine (MoveCamera (menuCamPosition, creditsCamPosition));
		state = State.Credits;
	}

	public void ShowMenu () {
		StartCoroutine (SlidePanel (state == State.Credits ? creditsPanel : playPanel, new Vector2 (0.5f, 0.0f), new Vector2 (0.5f, -1.0f)));
		StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.5f, 2.0f), new Vector2 (0.5f, 1.0f)));
		StartCoroutine (MoveCamera (creditsCamPosition, menuCamPosition));
		state = State.Menu;
	}

	public void ShowPlay () {
		StartCoroutine (SlidePanel (creditsPanel, new Vector2 (0.5f, 0.0f), new Vector2 (0.5f, -1.0f)));
		StartCoroutine (SlidePanel (menuPanel, new Vector2 (0.5f, 2.0f), new Vector2 (0.5f, 1.0f)));
		StartCoroutine (MoveCamera (creditsCamPosition, menuCamPosition));
		state = State.Play;
	}

	public void ExitGame () {
		Application.Quit ();
	}

	public IEnumerator SlidePanel (RectTransform target, Vector2 start, Vector2 end) {
		float startTime = Time.time;
		while (Time.time < startTime + slideDelay) {
			target.pivot = Vector2.Lerp (start, end, (Time.time - startTime) / slideDelay);
			yield return null;
		}
		target.pivot = end;
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
