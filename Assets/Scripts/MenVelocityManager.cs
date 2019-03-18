using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenVelocityManager : MonoBehaviour {

	Rigidbody rb;

	public bool isMoving;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (rb.velocity == Vector3.zero) {
			isMoving = false;
		} else {
			isMoving = true;
		}
	}
}
