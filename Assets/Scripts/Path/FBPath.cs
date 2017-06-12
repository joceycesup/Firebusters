using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public enum FBPathMode {
	Normal = 0x01,
	TwoWay = 0x02,
	Revert = 0x04
}

[Serializable]
public class FBPath : FBEditable {

	public delegate void PathEvent (FBPath path);
	public event PathEvent OnClose;
	public event PathEvent OnOpen;
	public event PathEvent OnDestroyed;

	[SerializeField, HideInInspector]
	private BezierSpline _spline;
	public BezierSpline spline {
		get { return _spline; }
		set {
			_spline = value;
			if (_spline != null) {
#if UNITY_EDITOR
				Name = "path_" + _spline.name;
#endif
				if (_start != null)
					start = _start;
				if (_end != null)
					end = _end;
			}
		}
	}
	[SerializeField, HideInInspector]
	private FBWaypoint _start;
	public FBWaypoint start {
		get { return _start; }
		set {
			_start = value;
			if (value != null) {
				if (_spline != null)
					_spline.transform.position = value.transform.position;
			}
		}
	}
	[SerializeField, HideInInspector]
	private FBWaypoint _end;
	public FBWaypoint end {
		get { return _end; }
		set {
			_end = value;
			if (value != null) {
				if (_spline != null) {
					if (_start == null)
						_spline.transform.position = value.transform.position - _spline.points[_spline.points.Length - 1];
					else
						_spline.points[_spline.points.Length - 1] = value.transform.position - spline.transform.position;
				}
			}
		}
	}
	//*
	[SerializeField, HideInInspector]
	private bool _open;
	public bool open {
		get { return _open; }
		set {
			_open = value;
			if (value) {
				if (OnOpen != null)
					OnOpen (this);
			}
			else if (OnClose != null)
				OnClose (this);
		}
	}/*
	[SerializeField]
	private bool _twoWays;
	public bool twoWays {
		get { return _twoWays; }
		set { _twoWays = value; }
	}//*/
	[SerializeField]
	private FBPathMode _mode;
	public FBPathMode mode {
		get { return _mode; }
		set { _mode = value; }
	}
	public bool highlight {
		get;
		private set;
	}

	void Awake () {
		start.AddPath (this);
		end.AddPath (this);
	}

	void OnDestroy () {
		if (OnDestroyed != null)
			OnDestroyed (this);
	}

	public bool CanTakePath (FBWaypoint wp) {
		return mode == FBPathMode.TwoWay || (mode == FBPathMode.Normal ? start == wp : end == wp);
		//return twoWays || start == wp;
	}

#if UNITY_EDITOR

	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		GUILayout.BeginHorizontal ();
		EditorGUI.BeginChangeCheck ();
		highlight = GUILayout.Button ("Show");
		if (EditorGUI.EndChangeCheck ()) {
			SceneView.RepaintAll ();
		}
		if (GUILayout.Button ("Rename")) {
			Name = start.Name + "_" + end.Name;
			spline.name = "Spline_" + Name;
			name = GetType () + "_" + Name;
		}
		GUILayout.EndHorizontal ();

		start = _start;
		end = _end;

		if (spline == null) {
			GUILayout.BeginHorizontal ();
			EditorGUI.BeginDisabledGroup (start == null || end == null);
			if (GUILayout.Button ("Create")) {
				GameObject newBezier = new GameObject ("spline_" + start.Name + "_" + end.Name);
				newBezier.transform.SetParent (transform.parent);
				spline = newBezier.AddComponent<BezierSpline> ();
				spline.transform.position = start.transform.position;
				for (int i = 0; i < spline.points.Length; ++i) {
					spline.points[i] = Vector3.Lerp (start.transform.position, end.transform.position, i / (spline.points.Length - 1.0f)) - spline.transform.position;
				}
			}
			EditorGUI.EndDisabledGroup ();
			spline = (BezierSpline) EditorGUILayout.ObjectField ("Spline", spline, typeof (BezierSpline), true);
			GUILayout.EndHorizontal ();
		}
		EditorGUI.BeginChangeCheck ();
		BezierSpline bs = (BezierSpline) EditorGUILayout.ObjectField ("Spline", spline, typeof (BezierSpline), true);
		if (EditorGUI.EndChangeCheck ())
			spline = bs;
		EditorGUI.BeginChangeCheck ();
		start = (FBWaypoint) EditorGUILayout.ObjectField ("Start", start, typeof (FBWaypoint), true);
		end = (FBWaypoint) EditorGUILayout.ObjectField ("End", end, typeof (FBWaypoint), true);
		if (EditorGUI.EndChangeCheck ()) {
			SceneView.RepaintAll ();
		}
		EditorGUI.BeginChangeCheck ();
		bool tmpOpen = EditorGUILayout.Toggle ("Open", open);
		if (EditorGUI.EndChangeCheck ()) {
			//Debug.Log (tmpOpen);
			open = tmpOpen;
			SceneView.RepaintAll ();
		}
		/*
		EditorGUI.BeginChangeCheck ();
		bool tmpTW = EditorGUILayout.Toggle ("Two ways", twoWays);
		if (EditorGUI.EndChangeCheck ()) {
			twoWays = tmpTW;
			SceneView.RepaintAll ();
		}//*/

		EditorGUI.BeginChangeCheck ();
		FBPathMode tmpMode = (FBPathMode) EditorGUILayout.EnumPopup ("Mode", mode);
		if (EditorGUI.EndChangeCheck ()) {
			mode = tmpMode;
			SceneView.RepaintAll ();
		}
		return this;
	}

	public override void DrawOnScene (bool externalCall = false) {
		if (spline == null)
			return;
		Color c = highlight ? Color.white : new Color (open ? 0.0f : 1.0f, open ? 1.0f : 0.0f, 0.0f, showingInInspector || !externalCall ? 1.0f : 0.2f);
		spline.Draw (c, showingInInspector || !externalCall);
		/*
		spline.DrawArrow (c);
		if (twoWays)
			spline.DrawArrow (c, false);
		/*/
		if (mode == FBPathMode.TwoWay || mode == FBPathMode.Normal)
			spline.DrawArrow (c, true);
		if (mode == FBPathMode.TwoWay || mode == FBPathMode.Revert)
			spline.DrawArrow (c, false);//*/
	}
#endif
}
