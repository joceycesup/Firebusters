using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FBMotionAnalyzer))]
public class FBMotionAnalyzerInspector : Editor {
	private FBMotionAnalyzer motion;

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
		motion.useKbRight = EditorGUILayout.Toggle ("Use keybawrd right", motion.useKbRight);

		EditorGUILayout.Space ();
		motion.toolMotion.showInInspector = EditorGUILayout.Foldout (motion.toolMotion.showInInspector, motion.isAxePuppet ? "Strike parameters :" : "Aim parameters :");
		if (motion.toolMotion.showInInspector) {
			motion.toolMotion.name = motion.isAxePuppet ? "Strike" : "Sheathe/Draw";
			motion.toolMotion = motion.toolMotion.GUIField ();
		}
		EditorGUILayout.Space ();
		motion.grabMotion.showInInspector = EditorGUILayout.Foldout (motion.grabMotion.showInInspector, "Grab parameters :");
		if (motion.grabMotion.showInInspector) {
			motion.grabMotion.name = "Grab";
			motion.grabMotion = motion.grabMotion.GUIField ();
		}

		EditorGUILayout.Space ();


		motion.maxRoll = EditorGUILayout.FloatField ("Max Roll Walk", motion.maxRoll);
		EditorGUILayout.BeginHorizontal ();
		motion.rollFactor = EditorGUILayout.CurveField ("Roll factor", motion.rollFactor);
		if (GUILayout.Button ("O", GUILayout.Width (20))) {
			FBMotionAnalyzer[] motions = FindObjectsOfType<FBMotionAnalyzer> ();
			foreach(FBMotionAnalyzer m in motions) {
				if (m != motion)
					m.rollFactor = motion.rollFactor;
			}
		}
		EditorGUILayout.EndHorizontal ();
		motion.maxPitch = EditorGUILayout.FloatField ("Max Pitch Steer", motion.maxPitch);
		EditorGUILayout.BeginHorizontal ();
		motion.pitchFactor = EditorGUILayout.CurveField ("Pitch factor", motion.pitchFactor);
		if (GUILayout.Button ("O", GUILayout.Width (20))) {
			FBMotionAnalyzer[] motions = FindObjectsOfType<FBMotionAnalyzer> ();
			foreach (FBMotionAnalyzer m in motions) {
				if (m != motion)
					m.pitchFactor = motion.pitchFactor;
			}
		}
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		motion.showSuccessDebug = EditorGUILayout.Toggle ("Show success debug", motion.showSuccessDebug);
		motion.showFailureDebug = EditorGUILayout.Toggle ("Show failure debug", motion.showFailureDebug);

		if (GUI.changed) {
			EditorUtility.SetDirty (motion);
		}
	}
}