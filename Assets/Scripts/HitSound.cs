using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSound : MonoBehaviour {

	AudioSource sound;

	void Start(){
		sound = GetComponent<AudioSource> ();
	}

	void OnCollisionEnter(Collision col){
		if (col.collider.tag == "White" || col.collider.tag == "Black" || col.collider.tag == "Queen") {
			sound.Play ();
		}
	}
}
