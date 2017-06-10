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
		EditorGUI.BeginChangeCheck ();
		if (Name.Length <= 0)
			Name = names[UnityEngine.Random.Range (0, names.Length)];
		Name = EditorGUILayout.TextField ("Name", Name);
		if (EditorGUI.EndChangeCheck () && Name.Length > 0) {
			name = GetType () + "_" + Name;
		}
		return this;
	}

	public virtual void DrawOnScene (bool externalCall = false) { }
}
#endif