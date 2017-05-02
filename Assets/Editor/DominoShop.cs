using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DominoShop : EditorWindow {
	public GameObject monPrefab;
	private Transform dominoParent;
	private Vector3 posCorrection;
	private Vector3 lastPos;
	public float maxDistance;

	[MenuItem ("Window/Equilibre/Mafenetre")]
	public static void CreateWindow () {
		DominoShop ds = EditorWindow.GetWindow<DominoShop> ();
		ds.wantsMouseMove = true;
		ds.monPrefab = null;
		ds.maxDistance = 1.0f;
		ds.dominoParent = new GameObject ("DominoParent").transform;
	}

	private void OnFocus () {
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		Debug.Log ("DominoShop gained focus");
	}

	private void OnLostFocus () {
		//SceneView.onSceneGUIDelegate -= OnSceneGUI;
		Debug.Log ("DominoShop lost focus");
	}

	void OnGUI () {
		monPrefab = (GameObject) EditorGUILayout.ObjectField ("Prefab", monPrefab, typeof (GameObject), false);
		if (monPrefab) {
			float y = monPrefab.GetComponent<MeshRenderer> ().bounds.extents.y;
			posCorrection = new Vector3 (0.0f, y, 0.0f);
		}
		maxDistance = EditorGUILayout.FloatField ("Max Distance", maxDistance);
		if (GUILayout.Button ("Clear")) {
			DestroyImmediate (dominoParent.gameObject);
			dominoParent = new GameObject ("DominoParent").transform;
		}
	}

	void OnSceneGUI (SceneView sceneView) {
		if (!monPrefab)
			return;
		Ray ray = HandleUtility.GUIPointToWorldRay (Event.current.mousePosition);
		RaycastHit hit;
		if (Event.current.type == EventType.layout || Event.current.modifiers == EventModifiers.Alt) {
			int controlID = GUIUtility.GetControlID (FocusType.Passive);
			HandleUtility.AddDefaultControl (controlID);
		}
		else if (Physics.Raycast (ray, out hit) && hit.transform.tag == "Floor") {
			if (Event.current.type == EventType.mouseDown) {
				lastPos = hit.point;
				Event.current.Use ();
				//GameObject.Instantiate (monPrefab, lastPos + posCorrection, Quaternion.LookRotation (lastPos - hit.point, Vector3.up), dominoParent);
			}
			else if (Event.current.type == EventType.mouseDrag) {
				Event.current.Use ();

				float distance = Vector3.Distance (lastPos, hit.point);
				if (distance >= maxDistance) {
					Vector3 deltaPos = Vector3.zero;
					Vector3 newPos = Vector3.zero;

					do {
						deltaPos = Vector3.ClampMagnitude (hit.point - lastPos, maxDistance);
						newPos = lastPos + deltaPos;
						GameObject.Instantiate (monPrefab, newPos + posCorrection, Quaternion.LookRotation (lastPos - hit.point, Vector3.up), dominoParent);
						lastPos = newPos;
					} while ((distance = Vector3.Distance (lastPos, hit.point)) >= maxDistance);
				}
			}
		}
	}
}