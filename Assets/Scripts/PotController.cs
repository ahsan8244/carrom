using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotController : MonoBehaviour {

	public Carrom carrom;
	public Transform queenHolder;

	private GameObject queen;
	private AudioSource sound;

	// Use this for initialization
	void Start () {
		sound = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col){
		if (col.tag == "White" || col.tag == "Black") {
			carrom.hasPocketed = true;
			if (carrom.currentPlayer == Carrom.Player.player1) {
				carrom.player1Score++;
				if (carrom.hasPocketedQueen) {
					carrom.player1Score = carrom.player1Score + 3;
					Destroy (queen);
					carrom.hasCoveredQueen = true;
					carrom.hasToCoverQueen = false;
					carrom.hasPocketedQueen = false;
				}
			} else if (carrom.currentPlayer == Carrom.Player.player2) {
				carrom.player2Score++;
				if (carrom.hasPocketedQueen) {
					carrom.player2Score = carrom.player2Score + 3;
					Destroy (queen);
					carrom.hasCoveredQueen = true;
					carrom.hasToCoverQueen = false;
					carrom.hasPocketedQueen = false;
				}
			}
			carrom.menOnBoard--;
			Destroy (col.gameObject);
			sound.Play ();
		}
		if (col.tag == "Queen") {
			carrom.hasPocketedQueen = true;
			carrom.menOnBoard--;
			queen = col.gameObject;
			queen.transform.position = queenHolder.position;
			sound.Play ();
		}
	}
}
