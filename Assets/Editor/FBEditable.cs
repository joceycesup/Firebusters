#if UNITY_EDITOR
using UnityEditor;
#endif

public class FBEditable {
#if UNITY_EDITOR
	public string Name = "";
	public bool showingInInspector = false;

	private static string[] names = { "Hello gorgeous ;)", "Pls name me :'(", "Monde de merde...", "My name is nobody", "Chuck Norris' favorite" };

	public virtual FBEditable GUIField () {
		EditorGUI.indentLevel++;
		if (Name.Length <= 0)
			Name = names[UnityEngine.Random.Range (0, names.Length)];
		Name = EditorGUILayout.TextField ("Name", Name);
		EditorGUI.indentLevel--;
		return this;
	}
#endif
}
