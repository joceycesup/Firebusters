using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FBMotionAnalyzer))]
public class FBMotionAnalyzerInspector : Editor {
	private FBMotionAnalyzer motion;
	private bool showToolParameters;

	private void OnEnable () {
		motion = (FBMotionAnalyzer) target;
	}

	private static FBAction MaskToAction (int mask) {
		FBAction res = FBAction.None;
		if (mask != 0) {
			FBAction[] actions = System.Enum.GetValues (typeof (FBAction)) as FBAction[];

			int bit = 1;
			for (int i = 0; i < actions.Length; ++i) {
				if ((mask & bit) == bit) {
					if (actions[i] == FBAction.None) {
						res = FBAction.None;
						break;
					}
					else {
						res |= actions[i];
					}
				}
				bit <<= 1;
			}
		}
		return res;
	}

	private static int ActionToMask (FBAction action) {
		int res = 0;
		if (action != FBAction.None) {
			FBAction[] actions = System.Enum.GetValues (typeof (FBAction)) as FBAction[];

			int bit = 1;
			for (int i = 0; i < actions.Length; ++i) {
				if ((action & actions[i]) != 0) {
					if (actions[i] != FBAction.None) {
						res |= bit;
					}
				}
				bit <<= 1;
			}
		}
		return res;
	}

	public override void OnInspectorGUI () {
		motion.abilities = MaskToAction ((int) (FBAction) EditorGUILayout.EnumMaskPopup ("Abilities", (FBAction) ActionToMask (motion.abilities)));
		//Debug.Log (System.Convert.ToString ((int) motion.abilities, 2));
		motion.usePhoneDataHandler = EditorGUILayout.Toggle ("Use phone data handler", motion.usePhoneDataHandler);

		EditorGUILayout.Space ();

		motion.isAxePuppet = EditorGUILayout.Toggle ("Is axe puppet", motion.isAxePuppet);

		EditorGUILayout.Space ();
		showToolParameters = EditorGUILayout.Foldout (showToolParameters, motion.isAxePuppet ? "Strike parameters :" : "Aim parameters :");
		if (showToolParameters) {
			EditorGUI.indentLevel = 1;
			if (motion.isAxePuppet) {
				motion.sheatheDrawMaxDuration = EditorGUILayout.FloatField ("Sheathe/Draw max duration", motion.sheatheDrawMaxDuration);
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

		motion.maxRoll = EditorGUILayout.FloatField ("Max Roll Walk", motion.maxRoll);
		motion.rollFactor = EditorGUILayout.CurveField ("Roll factor", motion.rollFactor);
		motion.maxPitch = EditorGUILayout.FloatField ("Max Pitch Steer", motion.maxPitch);
		motion.pitchFactor = EditorGUILayout.CurveField ("Pitch factor", motion.pitchFactor);

		if (GUI.changed) {
			EditorUtility.SetDirty (motion);
		}
	}
}