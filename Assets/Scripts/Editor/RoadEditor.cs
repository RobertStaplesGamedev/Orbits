using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadCreator))]
public class RoadEditor : Editor {
	RoadCreator creator;
	Road Road {
		get {
			return creator.road;
		}
	}

	bool advanced = false;

	const float segmentSelectedDistanceThreshold = .5f;
	int selectedSegmentIndex = -1;

	public override void OnInspectorGUI(){

		EditorGUI.BeginChangeCheck();

		bool displayControlPoints = GUILayout.Toggle(creator.displayControlPoints, "Display Control Points");
		if (displayControlPoints != creator.displayControlPoints) {
			Undo.RecordObject(creator, "Toggle Display Control Points");
			creator.displayControlPoints = displayControlPoints;
		}

		GUILayout.BeginHorizontal();
		bool isClosed = GUILayout.Toggle(Road.IsClosed, "Closed");
		if (isClosed != Road.IsClosed) {
			Undo.RecordObject(creator, "Toggle closed");
			Road.IsClosed = isClosed;
		}
		bool autoSetControlPoints = GUILayout.Toggle(Road.AutoSetControlPoints, "Auto Set Control Points");
		if (autoSetControlPoints != Road.AutoSetControlPoints) {
			Undo.RecordObject(creator, "Toggle auto set controls");
			Road.AutoSetControlPoints = autoSetControlPoints;
		}
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Create New")) {
			Undo.RecordObject(creator, "Create new");
			creator.CreateRoad();
		}

		if (EditorGUI.EndChangeCheck()) {
			SceneView.RepaintAll();
		}
		advanced = EditorGUILayout.Foldout(advanced, "Advanced");
		if (advanced) {
			base.OnInspectorGUI();
		}
	}

	void OnSceneGUI() {
		Input();
		Draw();
	}

	void Input() {
		Event guiEvent = Event.current;
		Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift) {
			if (selectedSegmentIndex != -1) {
				Undo.RecordObject(creator, "Split Segment");
				Road.SplitSegment(mousePos, selectedSegmentIndex);
			} else if (!Road.IsClosed) {
				Undo.RecordObject(creator, "Add Segment");
				Road.AddSegment(mousePos);
			}
		}
		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1) {
			float minDstToAnchor = creator.anchorDiameter * .5f;
			int closestAchorIndex = -1;

			for (int i = 0; i < Road.NumPoints; i+=3) {
				float dst = Vector2.Distance(mousePos, Road[i]);
				if (dst < minDstToAnchor) {
					minDstToAnchor = dst;
					closestAchorIndex = i;
				}
			}
			if (closestAchorIndex != -1) {
				Undo.RecordObject(creator, "Delete Segment");
				Road.DeleteSegment(closestAchorIndex);
			}
		}
		if (guiEvent.type == EventType.MouseMove) {
			float minDstToSegment = segmentSelectedDistanceThreshold;
			int newSelectedSegmentIndex = -1;

			for (int i = 0; i < Road.NumSegments; i++) {
				Vector2[] points = Road.GetPointsInSegment(i);
				float dst = HandleUtility.DistancePointBezier(mousePos, Vectorize(points[0]),Vectorize(points[3]),Vectorize(points[1]),Vectorize(points[2]));
				if (dst < minDstToSegment) {
					minDstToSegment = dst;
					newSelectedSegmentIndex = i;
				}
			}
			if (newSelectedSegmentIndex != selectedSegmentIndex) {
				selectedSegmentIndex = newSelectedSegmentIndex;
				HandleUtility.Repaint();
			}
		}
		HandleUtility.AddDefaultControl(0);
	}

	void Draw() {

		for (int i = 0; i < Road.NumSegments; i++) {
			Vector2[] points = Road.GetPointsInSegment(i);
			if (creator.displayControlPoints) {
				Handles.color = Color.black;
				Handles.DrawLine(Vectorize(points[1]),Vectorize(points[0]));
				Handles.DrawLine(Vectorize(points[2]),Vectorize(points[3]));
			}
			Color segmentCol = (i == selectedSegmentIndex && Event.current.shift) ? creator.selectedSegmentCol : creator.segmentCol;
			Handles.DrawBezier(Vectorize(points[0]),Vectorize(points[3]), Vectorize(points[1]),Vectorize(points[2]), segmentCol, null, 2);
		}

		Handles.color = Color.red;
		for (int i = 0; i < Road.NumPoints; i++) {
			if (i%3 == 0 || creator.displayControlPoints) {
				Handles.color = (i % 3 == 0) ? creator.anchorCol : creator.controlCol;
				float handleSize = (i % 3 == 0) ? creator.anchorDiameter : creator.controlDiameter;
				Vector2 newPos = Handles.FreeMoveHandle(Vectorize(Road[i]), Quaternion.identity, handleSize, Vector2.zero, Handles.CylinderHandleCap);
				if (Road[i] != newPos) {
					Undo.RecordObject(creator, "Move Point");
					Road.MovePoint(i, newPos);
				}
			}
		}
	}

	void OnEnable() {
		creator = (RoadCreator)target;
		if (creator.road == null) {
			creator.CreateRoad();
		}
	}

	Vector3 Vectorize (Vector2 point) {

		if (creator.orientation == RoadCreator.Orientation.Horizontal) {
			return new Vector3(point.x, 0, point.y);
		} else {
			return new Vector3(point.x, point.y, 0);
		}
		
	}

}
