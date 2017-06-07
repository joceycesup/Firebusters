using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class FBPathsGroup : MonoBehaviour {
	
	[SerializeField, HideInInspector]
	public List<FBPath> paths;// = new FBEditorList<FBPath> ();
}
