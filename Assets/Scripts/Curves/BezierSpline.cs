using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BezierSpline : MonoBehaviour {
	[SerializeField]
	public Vector3[] points;

	[SerializeField]
	private BezierControlPointMode[] modes;

	public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (index > 0) {
				points[index - 1] += delta;
			}
			if (index + 1 < points.Length) {
				points[index + 1] += delta;
			}
		}
		points[index] = point;
		EnforceMode (index);
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		EnforceMode (index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Length - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Length) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Length - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance (middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01 (t) * CurveCount;
			i = (int) t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint (Bezier.GetPoint (points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Length - 4;
		}
		else {
			t = Mathf.Clamp01 (t) * CurveCount;
			i = (int) t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint (Bezier.GetFirstDerivative (points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}

	public Vector3 Get2DNormal (float t) {
		Vector3 tmp = GetVelocity (t);
		return transform.TransformPoint (new Vector3 (tmp.y, -tmp.x, 0f));
	}

	public Vector3 Get2DNormal2 (float t) {
		Vector3 tmp = GetVelocity (t);
		return transform.TransformPoint (new Vector3 (-tmp.y, tmp.x, 0f));
	}

	public Vector3 GetDirection (float t) {
		return GetVelocity (t).normalized;
	}

	public float GetT (float currentT, float distance, int precision, bool forward = true) {
		if (!forward)
			distance = -distance;
		for (int j = 0; j < precision; ++j) {
			Vector3 velocity = GetVelocity (currentT);
			currentT += (distance / velocity.magnitude) / precision;
		}
		return currentT;
	}

	public void AddCurve () {
		Vector3 point = points[points.Length - 1];
		Array.Resize (ref points, points.Length + 3);
		point.x += 1f;
		points[points.Length - 3] = point;
		point.x += 1f;
		points[points.Length - 2] = point;
		point.x += 1f;
		points[points.Length - 1] = point;

		Array.Resize (ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode (points.Length - 4);
	}

	public void RemoveCurve (int index) {
		if (index % 3 != 0)
			return;
		int pointsToRemove = (index == 0 || index == points.Length - 1) ? 2 : 3;
		int i = index == 0 ? 0 : index - 1;
		for (; i < points.Length - pointsToRemove; ++i) {
			points[i] = points[i + pointsToRemove];
		}
		Array.Resize (ref points, points.Length - pointsToRemove);
	}

	public void Reset () {
		points = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}
#if UNITY_EDITOR
	public void Draw (Color color, bool showLines = true) {
		Vector3 p0 = transform.TransformPoint (GetControlPoint (0));
		for (int i = 1; i < ControlPointCount; i += 3) {
			Vector3 p1 = transform.TransformPoint (GetControlPoint (i));
			Vector3 p2 = transform.TransformPoint (GetControlPoint (i + 1));
			Vector3 p3 = transform.TransformPoint (GetControlPoint (i + 2));

			if (showLines) {
				Handles.color = Color.gray;
				Handles.DrawLine (p0, p1);
				Handles.DrawLine (p2, p3);
				Handles.color = new Color (0.0f, 0.0f, 0.0f, 0.2f);
				Handles.DrawLine (p1, p2);
			}

			Handles.DrawBezier (p0, p3, p1, p2, color, null, 2f);
			p0 = p3;
		}
	}
#endif
}