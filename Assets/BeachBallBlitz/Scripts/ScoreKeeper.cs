using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour {

	public int playerNumber;
	public int startingScore;
	private int playerScore;
	public GameObject scoreboard;
	private Text scoretext;


	// Use this for initialization
	void Start () {
		playerScore = startingScore;
		scoretext = scoreboard.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		scoretext.text = "Player " + playerNumber.ToString() + " : " + playerScore.ToString();
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "Ball") {
			playerScore--;
		}
	}

}
