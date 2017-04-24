using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FBPuppetController))]
public class FBPuppetControllerInspector : Editor {

	public override void OnInspectorGUI () {
		FBPuppetController controller = (FBPuppetController) target;

		GameObject g;
		g = (GameObject) EditorGUILayout.ObjectField ("Left foot", (controller.leftFoot != null) ? ((controller.leftFoot.transform != null) ? controller.leftFoot.transform.gameObject : null) : null, typeof (GameObject), true);
		if (g)
			controller.leftFoot = new FBFootState (g.transform);
		g = (GameObject) EditorGUILayout.ObjectField ("Right foot", (controller.rightFoot != null) ? ((controller.rightFoot.transform != null) ? controller.rightFoot.transform.gameObject : null) : null, typeof (GameObject), true);
		if (g)
			controller.rightFoot = new FBFootState (g.transform);
	}
}