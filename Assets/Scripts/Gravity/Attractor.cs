using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour {

	public static List<Attractor> attractors; 
	public Rigidbody rb;

	void FixedUpdate() {
		foreach (Attractor a in attractors) {
			if (a != this)
				Attract(a);
		}
	}

	void OnEnable() {

		if (attractors == null)
			attractors = new List<Attractor>();

		attractors.Add(this);
	}

	void OnDisable() {

		if (attractors == null)
			attractors = new List<Attractor>();

		attractors.Remove(this);
	}

	void Attract(Attractor objectToAttract) {
		Rigidbody rbToAttract = objectToAttract.rb;

		Vector3 direction = rb.position - rbToAttract.position;
		float distance = direction.magnitude;

		if (distance == 0)
			return;

		float forceMagnitute = (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
		Vector3 force = direction.normalized * forceMagnitute;

		rbToAttract.AddForce(force);
	}
}
