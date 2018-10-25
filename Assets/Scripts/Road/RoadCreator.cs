using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCreator : MonoBehaviour {

	[HideInInspector]
	public Road road;

	public enum Orientation {Horizontal, Vertical};

	public Orientation orientation;

	public Color anchorCol = Color.red;
	public Color controlCol = Color.white;
	public Color segmentCol = Color.green;
	public Color selectedSegmentCol = Color.yellow;
	public float anchorDiameter = .1f;
	public float controlDiameter = 0.075f;
	[HideInInspector]
	public bool displayControlPoints = true;

	public void CreateRoad() {
		road = new Road(transform.position);
	}

	void Reset() {
		CreateRoad();
	}
}
