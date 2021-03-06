﻿using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (BezierSpline))]
public class BezierSplineInspector : Editor {

	private const int stepsPerCurve = 10;
	private const float directionScale = 0.1f;
	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	private static Color[] modeColors = {
		Color.white,
		Color.yellow,
		Color.cyan
	};

	private BezierSpline spline;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private static int selectedIndex = -1;
	private static int lastInstanceID = -1;

	public override void OnInspectorGUI () {
		if (lastInstanceID != (lastInstanceID = GetInstanceID ())) {
			selectedIndex = -1;
			SceneView.RepaintAll ();
		}
		selectedIndex = EditorGUILayout.IntField ("Selected index", selectedIndex);
		spline = target as BezierSpline;
		if (spline.points.Length != spline.modes.Length)
			System.Array.Resize (ref spline.modes, spline.points.Length);
		if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount) {
			DrawSelectedPointInspector ();
		}

		if (GUILayout.Button ("Add Curve")) {
			Undo.RecordObject (spline, "Add Curve");
			spline.AddCurve ();
			EditorUtility.SetDirty (spline);
		}
	}

	private void DrawSelectedPointInspector () {
		GUILayout.Label ("Selected Point");
		EditorGUI.BeginChangeCheck ();
		Vector3 point = EditorGUILayout.Vector3Field ("Position", spline.GetControlPoint (selectedIndex));
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (spline, "Move Point");
			EditorUtility.SetDirty (spline);
			spline.SetControlPoint (selectedIndex, point);
		}
		EditorGUI.BeginChangeCheck ();
		BezierControlPointMode mode = (BezierControlPointMode) EditorGUILayout.EnumPopup ("Mode", spline.GetControlPointMode (selectedIndex));
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (spline, "Change Point Mode");
			spline.SetControlPointMode (selectedIndex, mode);
			EditorUtility.SetDirty (spline);
		}
		GUILayout.BeginHorizontal ();
		EditorGUI.BeginDisabledGroup (selectedIndex <= 0);
		if (GUILayout.Button ("Add before")) {
			System.Array.Resize (ref spline.points, spline.points.Length + 3);
			System.Array.Resize (ref spline.modes, spline.points.Length + 3);
			for (int i = spline.points.Length - 1; i >= selectedIndex + 3; --i) {
				spline.points[i] = spline.points[i - 3];
				spline.modes[i] = spline.modes[i - 3];
			}
			for (int i = 1; i < 4; ++i) {
				spline.points[selectedIndex - 1 + i] = Vector3.Lerp (spline.points[selectedIndex - 1], spline.points[selectedIndex + 3], i / 4.0f);
				spline.modes[selectedIndex - 1 + i] = BezierControlPointMode.Free;
			}
			SceneView.RepaintAll ();
		}
		EditorGUI.EndDisabledGroup ();
		EditorGUI.BeginDisabledGroup (selectedIndex >= spline.points.Length - 1);
		if (GUILayout.Button ("Add after")) {
			System.Array.Resize (ref spline.points, spline.points.Length + 3);
			System.Array.Resize (ref spline.modes, spline.points.Length + 3);
			for (int i = spline.points.Length - 1; i > selectedIndex + 3; --i) {
				spline.points[i] = spline.points[i - 3];
				spline.modes[i] = spline.modes[i - 3];
			}
			for (int i = 1; i < 4; ++i) {
				spline.points[selectedIndex + i] = Vector3.Lerp (spline.points[selectedIndex], spline.points[selectedIndex + 4], i / 4.0f);
				spline.modes[selectedIndex + i] = BezierControlPointMode.Free;
			}
			SceneView.RepaintAll ();
		}
		EditorGUI.EndDisabledGroup ();
		GUILayout.EndHorizontal ();
	}

	private void OnSceneGUI () {
		spline = target as BezierSpline;
		handleTransform = spline.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;
		spline.Draw (Color.white);
		for (int i = 0; i < spline.points.Length; ++i) {
			ShowPoint (i);
		}
		//ShowDirections ();
		//ShowNormals ();
	}
	//*
	private void ShowDirections () {
		Handles.color = Color.blue;
		Vector3 point = spline.GetPoint (0f);
		Handles.DrawLine (point, point + spline.GetDirection (0f) * directionScale);
		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint (i / (float) steps);
			Handles.DrawLine (point, point + spline.GetDirection (i / (float) steps) * directionScale);
		}
	}

	private void ShowNormals () {
		Vector3 point = spline.GetPoint (0f);
		Handles.color = Color.green;
		Handles.DrawLine (point, point + spline.Get2DNormal (0f) * directionScale);
		Handles.color = Color.red;
		Handles.DrawLine (point, point + spline.Get2DNormal2 (0f) * directionScale);
		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 1; i <= steps; i++) {
			point = spline.GetPoint (i / (float) steps);
			Handles.color = Color.green;
			Handles.DrawLine (point, point + spline.Get2DNormal (i / (float) steps) * directionScale);
			Handles.color = Color.red;
			Handles.DrawLine (point, point + spline.Get2DNormal2 (i / (float) steps) * directionScale);
		}
	}
	/*/
	private void ShowDirections () {
		Handles.color = Color.green;
		Vector3 point;
		Vector3 acc;
		Vector3 vel;
		int steps = stepsPerCurve * spline.CurveCount;
		for (int i = 0; i <= steps; i++) {
			point = spline.GetPoint (i / (float)steps);
			acc = spline.GetAcceleration (i / (float)steps);
			vel = spline.GetDirection (i / (float)steps);
			vel.x *= Mathf.Sign (acc.x);vel.y *= Mathf.Sign (acc.y);vel.z *= Mathf.Sign (acc.z);
			Handles.DrawLine (point, point + vel * directionScale * acc.magnitude);
		}
	}//*/

	private Vector3 ShowPoint (int index) {
		Vector3 point = handleTransform.TransformPoint (spline.GetControlPoint (index));
		float size = HandleUtility.GetHandleSize (point);
		if (index == 0)
			size *= 2f;
		else if (index % 3 == 0)
			size *= 1.5f;
		Handles.color = modeColors[(int) spline.GetControlPointMode (index)];
		if (Handles.Button (point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap)) {
			selectedIndex = index;
			Repaint ();
			if (Event.current.button == 2) {
				if (index % 3 == 0) {
					Undo.RecordObject (spline, "Remove Point");
					EditorUtility.SetDirty (spline);
					spline.RemoveCurve (index);
				}
			}
		}
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck ();
			point = Handles.DoPositionHandle (point, handleRotation);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject (spline, "Move Point");
				EditorUtility.SetDirty (spline);
				spline.SetControlPoint (index, handleTransform.InverseTransformPoint (point));
				//if (index == spline.ControlPointCount - 1)	Debug.Log (Time.time);
			}
		}
		return point;
	}
}