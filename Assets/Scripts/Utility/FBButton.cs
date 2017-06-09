using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (MeshRenderer))]
public class FBButton : MonoBehaviour {
	public UnityEvent onClick;
	public UnityEvent onHover;
	public float emissiveOnHover;

	private bool isHover = false;
	private MeshRenderer mr;
	private Color emissiveColor;
	private int hoverFrame = 0;

	void OnEnable () {
		SetAlbedo (Color.white);
	}

	void OnDisable () {
		SetAlbedo (Color.gray);
	}

	void Start () {
		mr = GetComponent<MeshRenderer> ();
		emissiveColor = mr.material.GetColor ("_EmissionColor");
		SetEmissive (0.0f);
		SetAlbedo (Color.white);
	}

	public void Click () {
		if (!enabled)
			return;
		onClick.Invoke ();
	}

	public void Hover () {
		if (!enabled)
			return;
		if (!isHover) {
			StartCoroutine (ResetEmissive (hoverFrame));
		}
		onHover.Invoke ();
		isHover = true;
		hoverFrame++;
	}

	void SetEmissive (float value) {
		if (mr)
			mr.material.SetColor ("_EmissionColor", emissiveColor * Mathf.LinearToGammaSpace (value));
	}

	void SetAlbedo (Color c) {
		if (mr)
			mr.material.SetColor ("_Color", c);
	}

	private IEnumerator ResetEmissive (int startFrame) {
		SetEmissive (emissiveOnHover);
		while (startFrame <= hoverFrame) {
			startFrame++;
			yield return null;
		}
		isHover = false;
		SetEmissive (0.0f);
	}
}
