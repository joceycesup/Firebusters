using UnityEngine;

public class FBDisplayHandler : MonoBehaviour {
	public Canvas leftCanvas;
	public Canvas rightCanvas;

	private void Start () {
#if !PLAYTEST
		Cursor.visible = false;
#endif
		Camera fstCamera = null;
		Camera sndCamera = null;
		foreach (Camera cam in Camera.allCameras) {
			fstCamera = cam.targetDisplay == 0 ? cam : fstCamera;
			sndCamera = cam.targetDisplay == 1 ? cam : sndCamera;
		}

		if (Display.displays.Length <= 1 || !Screen.fullScreen) {
			sndCamera.targetDisplay = 0;
			fstCamera.rect = new Rect (0.0f, 0.0f, 0.5f, 1.0f);
			sndCamera.rect = new Rect (0.5f, 0.0f, 0.5f, 1.0f);

			if (leftCanvas) {
				leftCanvas.targetDisplay = 0;
				RectTransform rt = leftCanvas.transform.GetChild (0).GetComponent<RectTransform> ();
				rt.sizeDelta = new Vector2 (leftCanvas.GetComponent<RectTransform> ().sizeDelta.x / 2.0f, rt.sizeDelta.y);
				//Debug.Log (leftCanvas.GetComponent<RectTransform> ().sizeDelta.y + " : " + rt.sizeDelta.y);
				rt.anchorMin = new Vector2 (0.0f, rt.anchorMin.y);
				rt.anchorMax = new Vector2 (0.0f, rt.anchorMax.y);
			}
			if (rightCanvas) {
				rightCanvas.targetDisplay = 0;
				RectTransform rt = rightCanvas.transform.GetChild (0).GetComponent<RectTransform> ();
				rt.sizeDelta = new Vector2 (rightCanvas.GetComponent<RectTransform> ().sizeDelta.x / 2.0f, rt.sizeDelta.y);
				//Debug.Log (rightCanvas.GetComponent<RectTransform> ().sizeDelta.y + " : " + rt.sizeDelta.y);
				rt.anchorMin = new Vector2 (0.5f, rt.anchorMin.y);
				rt.anchorMax = new Vector2 (0.5f, rt.anchorMax.y);
			}
		}
		else {
			Display.displays[1].Activate ();
			fstCamera.rect = sndCamera.rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f);

			if (leftCanvas) {
				leftCanvas.targetDisplay = 0;
				RectTransform rt = leftCanvas.transform.GetChild (0).GetComponent<RectTransform> ();
				rt.sizeDelta = new Vector2 (leftCanvas.GetComponent<RectTransform> ().sizeDelta.x, rt.sizeDelta.y);
				//Debug.Log (leftCanvas.GetComponent<RectTransform> ().sizeDelta.y + " : " + rt.sizeDelta.y);
				rt.anchorMin = new Vector2 (0.0f, rt.anchorMin.y);
				rt.anchorMax = new Vector2 (0.0f, rt.anchorMax.y);
			}
			if (rightCanvas) {
				rightCanvas.targetDisplay = 1;
				RectTransform rt = rightCanvas.transform.GetChild (0).GetComponent<RectTransform> ();
				rt.sizeDelta = new Vector2 (rightCanvas.GetComponent<RectTransform> ().sizeDelta.x, rt.sizeDelta.y);
				//Debug.Log (rightCanvas.GetComponent<RectTransform> ().sizeDelta.y + " : " + rt.sizeDelta.y);
				rt.anchorMin = new Vector2 (0.0f, rt.anchorMin.y);
				rt.anchorMax = new Vector2 (0.0f, rt.anchorMax.y);
			}
#if TRIPEL
			if (Display.displays.Length > 2) {
				Display.displays[2].Activate ();
				GameObject fstCameraBisGO = new GameObject ();
				Camera fstCameraBis = fstCameraBisGO.AddComponent<Camera> ();
				fstCameraBis.CopyFrom (fstCamera);
				fstCameraBisGO.transform.SetParent (fstCamera.transform);

				GameObject sndCameraBisGO = new GameObject ();
				Camera sndCameraBis = sndCameraBisGO.AddComponent<Camera> ();
				sndCameraBis.CopyFrom (sndCamera);
				sndCameraBisGO.transform.SetParent (sndCamera.transform);

				fstCameraBisGO.transform.localEulerAngles = fstCameraBisGO.transform.localPosition = sndCameraBisGO.transform.localEulerAngles = sndCameraBisGO.transform.localPosition = Vector3.zero;

				fstCameraBis.targetDisplay = sndCameraBis.targetDisplay = 2;
				fstCameraBis.rect = new Rect (0.0f, 0.0f, 0.5f, 1.0f);
				sndCameraBis.rect = new Rect (0.5f, 0.0f, 0.5f, 1.0f);
			}
#endif
		}
		Destroy (this);
	}
}
