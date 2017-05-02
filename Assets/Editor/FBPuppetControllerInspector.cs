using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FBPuppetController))]
public class FBPuppetControllerInspector : Editor {
	private FBPuppetController controller;
	private bool showToolParameters;
	private bool axePuppet;

	private void OnEnable () {
		controller = (FBPuppetController) target;
		if (controller.motion == null) {
			controller.motion = controller.gameObject.GetComponent<FBMotionAnalyzer> ();
		}
	}

	public override void OnInspectorGUI () {
		GameObject g;
		g = (GameObject) EditorGUILayout.ObjectField ("Left foot", (controller.leftFoot != null) ? ((controller.leftFoot.transform != null) ? controller.leftFoot.transform.gameObject : null) : null, typeof (GameObject), true);
		if (g)
			controller.CreateFoot (g.transform, true);
		g = (GameObject) EditorGUILayout.ObjectField ("Right foot", (controller.rightFoot != null) ? ((controller.rightFoot.transform != null) ? controller.rightFoot.transform.gameObject : null) : null, typeof (GameObject), true);
		if (g)
			controller.CreateFoot (g.transform, false);

		EditorGUILayout.Space ();
		controller.steeringTurnRate = EditorGUILayout.FloatField ("Steering turn rate", controller.steeringTurnRate);
		controller.useMaxAngleSpan = EditorGUILayout.Toggle ("Use max angle span", controller.useMaxAngleSpan);
		controller.maxAngleSpan = EditorGUILayout.FloatField ("Max angle span", controller.maxAngleSpan);

		EditorGUILayout.Space ();
		showToolParameters = EditorGUILayout.Foldout (showToolParameters, controller.motion.isAxePuppet ? "Strike parameters :" : "Aim parameters :");
		if (showToolParameters) {
			EditorGUI.indentLevel = 1;
			controller.tool = (Rigidbody) EditorGUILayout.ObjectField ("Tool", controller.tool, typeof (Rigidbody), true);
			if (controller.motion.isAxePuppet) {
				controller.strikeDuration = EditorGUILayout.FloatField ("Duration", controller.strikeDuration);
				controller.strikeCooldown = EditorGUILayout.FloatField ("Cooldown", controller.strikeCooldown);

				controller.bladeForce = EditorGUILayout.FloatField ("Blade force", controller.bladeForce);
				controller.bottomForce = EditorGUILayout.FloatField ("Bottom force", controller.bottomForce);

				controller.anticipationBladeDirection = EditorGUILayout.Vector3Field ("Anticipation blade direction", controller.anticipationBladeDirection);
				controller.anticipationBottomDirection = EditorGUILayout.Vector3Field ("Anticipation bottom direction", controller.anticipationBottomDirection);
				controller.strikeBladeDirection = EditorGUILayout.Vector3Field ("Strike blade direction", controller.strikeBladeDirection);
				controller.strikeBottomDirection = EditorGUILayout.Vector3Field ("Strike bottom direction", controller.strikeBottomDirection);

				controller.anticipationDotProduct = EditorGUILayout.FloatField ("Anticipation dot product", controller.anticipationDotProduct);
			}
			else {
				controller.maxRollAim = EditorGUILayout.FloatField ("Max roll Aim", controller.maxRollAim);
			}
			EditorGUI.indentLevel = 0;
		}

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