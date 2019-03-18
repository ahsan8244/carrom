using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//coded with <3 by ahsan

public class Carrom : MonoBehaviour {

	//striker object
	public GameObject striker;

	//original position vectors
	public Vector3 strikerOrigin, lClampOrigin, rClampOrigin, queenOrigin;
	//clamp positions
	public Transform lClamp, rClamp;
	//Men Parent Object
	public Transform menParent;

	//speed variable for setting striker pos
	public float speed = 10f;

	//number of pieces on the board
	public int menOnBoard;

	//pocketing booleans for logic
	public bool hasPocketed, hasPocketedQueen, hasToCoverQueen, hasCoveredQueen;

	//UI elements
	public Button player1Ready, player2Ready;
	public Text player1ScoreUI, player2ScoreUI, winningTextUI;
	public Image touchHandler;

	//list containing all the pieces
	public List<MenVelocityManager> men = new List<MenVelocityManager>();

	//player logic booleans
	private bool isPlayer1Ready, isPlayer2Ready, player1Shot, player2Shot, timeToCheck, areAllMenStill, hasTimeRunOut;

	//enum defining the 2 players
	public enum Player {
		player1,
		player2
	}

	//contains the player who is currently playing
	public Player currentPlayer;

	//rigidbody attached to striker
	private Rigidbody rb;

	//integer variables containing scores for the 2 players
	public int player1Score, player2Score;

	//number of pieces with 0 velocity
	private int stillMen;

	//variables for the calculation of the striker's speed
	Vector3 startPos, endPos;
	float startTime, endTime;

	// Use this for initialization
	void Start () {
		currentPlayer = Player.player1;
		lClampOrigin = new Vector3 (lClamp.position.x, lClamp.position.y * -1, lClamp.position.z);
		rClampOrigin = new Vector3 (rClamp.position.x, rClamp.position.y * -1, rClamp.position.z);
		player1Ready.interactable = true;
		rb = striker.GetComponent<Rigidbody> ();

		player1Score = 0;
		player2Score = 0;
		menOnBoard = 19;
		stillMen = 19;

		strikerOrigin = striker.transform.position;
		lClamp.position = lClampOrigin;
		rClamp.position = rClampOrigin;
		queenOrigin = men [0].transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0 && 
			Input.GetTouch(0).phase == TouchPhase.Moved && !isPlayer1Ready && !player1Shot && !isPlayer2Ready && !player2Shot) {
			 
			// Get movement of the finger since last frame
			Vector2 touchDeltaPosition= Input.GetTouch(0).deltaPosition;

			// Move object across X plane
			striker.transform.Translate (Mathf.Clamp(touchDeltaPosition.x * speed, lClamp.position.x, rClamp.position.x), 0, 0);
			Vector3 pos = striker.transform.position;
			pos.x = Mathf.Clamp (pos.x, lClamp.position.x, rClamp.position.x);
			striker.transform.position = pos;
		}

		if (isPlayer1Ready || isPlayer2Ready) {
			touchHandler.enabled = true;
		}

		if (!isPlayer1Ready && !isPlayer2Ready) {
			touchHandler.enabled = false;
		}

		if (player1Shot) {
			StartCoroutine (TurnTimer ());
			timeToCheck = true;
			if ((rb.velocity == Vector3.zero && areAllMenStill) || hasTimeRunOut) {
				StopCoroutine (TurnTimer ());
				if (!hasPocketed && !hasPocketedQueen) {
					currentPlayer = Player.player2;
					striker.transform.position = new Vector3 (strikerOrigin.x, strikerOrigin.y, strikerOrigin.z * -1);
					lClamp.position = new Vector3 (lClampOrigin.x, lClampOrigin.y * -1, lClampOrigin.z);
					rClamp.position = new Vector3 (rClampOrigin.x, rClampOrigin.y * -1, rClampOrigin.z);
					player2Ready.interactable = true;
				}
				if (hasPocketed) {
					striker.transform.position = strikerOrigin;
					lClamp.position = lClampOrigin;
					rClamp.position = rClampOrigin;
					player1Ready.interactable = true;
				}
				if (hasPocketedQueen) {
					if (hasToCoverQueen) {
						if (!hasCoveredQueen) {
							men [0].transform.position = queenOrigin;
							currentPlayer = Player.player2;
							striker.transform.position = new Vector3 (strikerOrigin.x, strikerOrigin.y, strikerOrigin.z * -1);
							lClamp.position = new Vector3 (lClampOrigin.x, lClampOrigin.y * -1, lClampOrigin.z);
							rClamp.position = new Vector3 (rClampOrigin.x, rClampOrigin.y * -1, rClampOrigin.z);
							player2Ready.interactable = true;
							hasToCoverQueen = false;
							hasPocketedQueen = false;
						}
					} else {
						hasToCoverQueen = true;
						striker.transform.position = strikerOrigin;
						lClamp.position = lClampOrigin;
						rClamp.position = rClampOrigin;
						player1Ready.interactable = true;
					}
				}
				hasPocketed = false;
				player1Shot = false;

				//check if game is supposed to end
				if(menParent.childCount == 0){
					Debug.Log ("game over!");
					player1Ready.interactable = false;
					player2Ready.interactable = false;
					if (player1Score == player2Score)
						winningTextUI.text = "Tied!";
					else if (player1Score > player2Score)
						winningTextUI.text = "player1 wins!";
					else
						winningTextUI.text = "player2 wins!";
				}
			}
		}

		if (player2Shot) {
			StartCoroutine (TurnTimer ());
			timeToCheck = true;
			if ((rb.velocity == Vector3.zero && areAllMenStill) || hasTimeRunOut) {
				StopCoroutine (TurnTimer ());
				if (!hasPocketed && !hasPocketedQueen) {
					currentPlayer = Player.player1;
					striker.transform.position = strikerOrigin;
					lClamp.position = lClampOrigin;
					rClamp.position = rClampOrigin;
					player1Ready.interactable = true;
				}
				if (hasPocketed) {
					striker.transform.position = new Vector3 (strikerOrigin.x, strikerOrigin.y, strikerOrigin.z * -1);
					lClamp.position = new Vector3 (lClampOrigin.x, lClampOrigin.y * -1, lClampOrigin.z);
					rClamp.position = new Vector3 (rClampOrigin.x, rClampOrigin.y * -1, rClampOrigin.z);
					player2Ready.interactable = true;
				}
				if (hasPocketedQueen) {
					if (hasToCoverQueen) {
						if (!hasCoveredQueen) {
							men [0].transform.position = queenOrigin;
							currentPlayer = Player.player1;
							striker.transform.position = strikerOrigin;
							lClamp.position = lClampOrigin;
							rClamp.position = rClampOrigin;
							player1Ready.interactable = true;
							hasToCoverQueen = false;
							hasPocketedQueen = false;
						}
					} else {
						hasToCoverQueen = true;
						striker.transform.position = new Vector3 (strikerOrigin.x, strikerOrigin.y, strikerOrigin.z * -1);
						lClamp.position = new Vector3 (lClampOrigin.x, lClampOrigin.y * -1, lClampOrigin.z);
						rClamp.position = new Vector3 (rClampOrigin.x, rClampOrigin.y * -1, rClampOrigin.z);
						player2Ready.interactable = true;
					}
				}
				hasPocketed = false;
				player2Shot = false;

				//check if game is supposed to end
				if(menParent.childCount == 0){
					Debug.Log ("game over!");
					player1Ready.interactable = false;
					player2Ready.interactable = false;
					if (player1Score == player2Score)
						winningTextUI.text = "Tied!";
					else if (player1Score > player2Score)
						winningTextUI.text = "player1 wins!";
					else
						winningTextUI.text = "player2 wins!";
				}
			}
		}

		if (timeToCheck) {
			stillMen = 0;
			foreach (MenVelocityManager man in men) {
				if (man != null) {
					if (!man.isMoving) {
						stillMen++;
						Debug.Log ("one stopped");
					}
				}
			}
			if (stillMen >= menOnBoard) {
				areAllMenStill = true;
				timeToCheck = false;
			} else {
				areAllMenStill = false;
			}
			stillMen = 0;
		}

		UpdateScores (player1Score, player2Score);
	}

	private void UpdateScores(int score1, int score2){
		player1ScoreUI.text = score1.ToString ();
		player2ScoreUI.text = score2.ToString ();
	}

	public void Player1Ready(){
		Debug.Log ("player 1 is ready to strike");
		isPlayer1Ready = true;
		player1Ready.interactable = false;
	}

	public void Player2Ready(){
		isPlayer2Ready = true;
		player2Ready.interactable = false;
	}

	public void StrikeStart(){
		Debug.Log ("strike start");
		startPos = Input.mousePosition;
		startTime = Time.time;
	}

	public void StrikeEnd(){
		Debug.Log ("strike end");
		endPos = Input.mousePosition;
		endTime = Time.time;
		float duration = endTime - startTime;
		float vx = (endPos.x - startPos.x) / duration;
		float vz = (endPos.y - startPos.y) / duration;

		rb.velocity = new Vector3 (vx /10f, 0, vz /10f);
		if (currentPlayer == Player.player1) {
			player1Shot = true;
			isPlayer1Ready = false;
		} else if (currentPlayer == Player.player2) {
			player2Shot = true;
			isPlayer2Ready = false;
		}
	}

	IEnumerator TurnTimer(){
		hasTimeRunOut = false;
		yield return new WaitForSeconds (10);
		hasTimeRunOut = true;
	}
}
