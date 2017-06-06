using UnityEditor;

[CustomEditor (typeof (FBEditable), true), CanEditMultipleObjects]
public class FBEditableInspector : Editor {
	private FBEditable editable;

	private void OnEnable () {
		editable = (FBEditable) target;
	}

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		editable.GUIField ();
	}

	private void OnSceneGUI () {
		editable.DrawOnScene ();
	}
}
