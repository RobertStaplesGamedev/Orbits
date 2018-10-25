using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoadMeshCreator))]
public class RoadMeshEditor : Editor {

	RoadMeshCreator creator;

	void OnSceneGUI() {
		if (creator.autoUpdate && Event.current.type == EventType.Repaint) {
			creator.UpdateRoad();
		}
	}

	void OnEnable() {
		creator = (RoadMeshCreator)target;
	}
}
