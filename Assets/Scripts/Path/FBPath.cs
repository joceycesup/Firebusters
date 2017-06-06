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
			if (_start != null && _end != value) {
				OnOpen -= _start.OpenPath;
				OnClose -= _start.ClosePath;
				_start.paths.Remove (this);
			}
			_start = value;
			if (value != null) {
				_start.paths.Add (this);
				OnOpen += _start.OpenPath;
				OnClose += _start.ClosePath;
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
			if (_end != null && _end != value) {
				OnOpen -= _end.OpenPath;
				OnClose -= _end.ClosePath;
				_end.paths.Remove (this);
			}
			_end = value;
			if (value != null) {
				_start.paths.Add (this);
				OnOpen += _end.OpenPath;
				OnClose += _end.ClosePath;
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
	public override FBEditable GUIField (string label = "") {
		base.GUIField ();

		start = _start;
		end = _end;

		EditorGUI.BeginChangeCheck ();
		spline = (BezierSpline) EditorGUILayout.ObjectField ("Spline", spline, typeof (BezierSpline), true);
		start = (FBWaypoint) EditorGUILayout.ObjectField ("Start", start, typeof (FBWaypoint), true);
		end = (FBWaypoint) EditorGUILayout.ObjectField ("End", end, typeof (FBWaypoint), true);
		if (EditorGUI.EndChangeCheck ()) {
			SceneView.RepaintAll ();
		}
		EditorGUI.BeginChangeCheck ();
		bool tmpOpen = EditorGUILayout.Toggle ("Open", open);
		if (EditorGUI.EndChangeCheck ()) {
			open = tmpOpen;
			SceneView.RepaintAll ();
		}
		return this;
	}

	public override void DrawOnScene (bool externalCall = false) {
		if (spline == null)
			return;
		spline.Draw (new Color (open ? 0.0f : 1.0f, open ? 1.0f : 0.0f, 0.0f, showingInInspector || !externalCall ? 1.0f : 0.2f), showingInInspector || !externalCall);
	}
#endif
}
