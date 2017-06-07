using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FBPath :
#if UNITY_EDITOR
	FBEditable
#else
	MonoBehaviour
#endif
	{

	public delegate void PathEvent (FBPath path);
	public event PathEvent OnClose;
	public event PathEvent OnOpen;

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
	}

#if UNITY_EDITOR
	public bool highlight {
		get;
		private set;
	}

	void Awake () {
		start.AddPath (this);
		end.AddPath (this);
	}

	public override FBEditable GUIField (string label = "") {
		base.GUIField ();
		EditorGUI.BeginChangeCheck ();
		highlight = GUILayout.Button ("Show");
		if (EditorGUI.EndChangeCheck ()) {
			SceneView.RepaintAll ();
		}

		start = _start;
		end = _end;

		if (spline == null) {
			GUILayout.BeginHorizontal ();
			if (start == null || end == null)
				EditorGUI.BeginDisabledGroup (false);
			if (GUILayout.Button ("Create")) {
				GameObject newBezier = new GameObject ("spline_" + start.Name + "_" + end.Name);
				newBezier.transform.SetParent (transform.parent);
				spline = newBezier.AddComponent<BezierSpline> ();
				spline.transform.position = start.transform.position;
				for (int i = 0; i < spline.points.Length; ++i) {
					spline.points[i] = Vector3.Lerp (start.transform.position, end.transform.position, i / (spline.points.Length - 1.0f)) - spline.transform.position;
				}
			}
			if (start == null || end == null)
				EditorGUI.EndDisabledGroup ();
			spline = (BezierSpline) EditorGUILayout.ObjectField ("Spline", spline, typeof (BezierSpline), true);
			GUILayout.EndHorizontal ();
			EditorGUI.BeginChangeCheck ();
		}
		else {
			EditorGUI.BeginChangeCheck ();
			spline = (BezierSpline) EditorGUILayout.ObjectField ("Spline", spline, typeof (BezierSpline), true);
		}
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
		return this;
	}

	public override void DrawOnScene (bool externalCall = false) {
		if (spline == null)
			return;
		spline.Draw (highlight ? Color.white : new Color (open ? 0.0f : 1.0f, open ? 1.0f : 0.0f, 0.0f, showingInInspector || !externalCall ? 1.0f : 0.2f), showingInInspector || !externalCall);
	}
#endif
}
