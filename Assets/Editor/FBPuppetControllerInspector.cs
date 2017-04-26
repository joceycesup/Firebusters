using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FBPuppetController))]
public class FBPuppetControllerInspector : Editor {
	private FBPuppetController controller;

	private void OnEnable () {
		controller = (FBPuppetController) target;
	}

	public override void OnInspectorGUI () {
		GameObject g;
		g = (GameObject) EditorGUILayout.ObjectField ("Left foot", (controller.leftFoot != null) ? ((controller.leftFoot.transform != null) ? controller.leftFoot.transform.gameObject : null) : null, typeof (GameObject), true);
		if (g)
			controller.CreateFoot(g.transform, true);
		g = (GameObject) EditorGUILayout.ObjectField ("Right foot", (controller.rightFoot != null) ? ((controller.rightFoot.transform != null) ? controller.rightFoot.transform.gameObject : null) : null, typeof (GameObject), true);
		if (g)
			controller.CreateFoot (g.transform, false);

		EditorGUILayout.Space ();
		controller.steeringTurnRate = EditorGUILayout.FloatField ("Steering turn rate", controller.steeringTurnRate);
		controller.useMaxAngleSpan = EditorGUILayout.Toggle ("Use max angle span", controller.useMaxAngleSpan);
		controller.maxAngleSpan = EditorGUILayout.FloatField ("Max angle span", controller.maxAngleSpan);

		EditorGUILayout.Space ();
		controller.camera = (Camera) EditorGUILayout.ObjectField ("Camera", controller.camera, typeof (Camera), true);

		controller.cameraTarget = (Transform) EditorGUILayout.ObjectField ("Camera target", controller.cameraTarget, typeof (Transform), true);
		controller.cameraPosition = (Transform) EditorGUILayout.ObjectField ("Camera position", controller.cameraPosition, typeof (Transform), true);
		if (controller.cameraPosition) {
			controller.camera.transform.parent = controller.cameraPosition;
			controller.camera.transform.localPosition = Vector3.zero;
		}
		if (controller.cameraTarget) {
			controller.camera.transform.LookAt (controller.cameraTarget);
		}

		if (GUI.changed) {
			EditorUtility.SetDirty (controller);
		}
	}
}