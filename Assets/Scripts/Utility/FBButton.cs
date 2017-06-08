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

	void Start () {
		mr = GetComponent<MeshRenderer> ();
		emissiveColor = mr.material.GetColor ("_EmissionColor");
	}

	public void Click () {
		onClick.Invoke ();
	}

	public void Hover () {
		if (!isHover) {
			StartCoroutine (ResetEmissive (hoverFrame));
		}
		onHover.Invoke ();
		isHover = true;
		hoverFrame++;
	}

	void SetEmissive (float value) {
		mr.material.SetColor ("_EmissionColor", emissiveColor * Mathf.LinearToGammaSpace (value));
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
