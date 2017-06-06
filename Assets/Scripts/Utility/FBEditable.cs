#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

[Serializable]
public abstract class FBEditable : MonoBehaviour {
	[HideInInspector]
	public string Name = "";
	[HideInInspector]
	public bool showingInInspector = false;

	private static string[] names = { "Hello gorgeous ;)", "Pls name me :'(", "Monde de merde...", "My name is nobody", "Chuck Norris' favorite" };

	public virtual FBEditable GUIField (string label = "") {
		EditorGUI.indentLevel++;
		if (Name.Length <= 0)
			Name = names[UnityEngine.Random.Range (0, names.Length)];
		Name = EditorGUILayout.TextField ("Name", Name);
		EditorGUI.indentLevel--;
		return this;
	}

	public virtual void DrawOnScene (bool externalCall = false) { }
}
#endif