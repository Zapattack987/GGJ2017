﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeachBallPlayer : MonoBehaviour {

	public float speed;
	public string keyLeftDown;
	public string keyRightUp;
	public string keyAction;
	public string axis;
	public GameObject wave;
	public int wavespeed;
	private bool enabled = true;
	public float waveRotation;

	void Start ()
	{
	}

	void FixedUpdate ()
	{

		if (axis.ToLower() == "x") {
			
			if (Input.GetKey ((KeyCode)Enum.Parse (typeof(KeyCode), keyLeftDown)) && transform.position.x > -375) {
				transform.position += Vector3.left * speed * Time.deltaTime;

			}
			if (Input.GetKey ((KeyCode)Enum.Parse (typeof(KeyCode), keyRightUp)) && transform.position.x < 375) {
				transform.position += Vector3.right * speed * Time.deltaTime;
			}
		}

		else if (axis.ToLower() == "z") {

			if (Input.GetKey ((KeyCode)Enum.Parse (typeof(KeyCode), keyLeftDown)) && transform.position.z > -375) {
				transform.position += Vector3.back * speed * Time.deltaTime;

			}
			if (Input.GetKey ((KeyCode)Enum.Parse (typeof(KeyCode), keyRightUp)) && transform.position.z < 375) {
				transform.position += Vector3.forward * speed * Time.deltaTime;
			}
		}

		if(enabled && Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), keyAction))){
			GameObject waveInstance = new GameObject();
			if(axis.ToLower() == "x"){
				if (transform.position.z > 0) {
					waveInstance = Instantiate (wave, new Vector3 (transform.position.x+75, 1, transform.position.z - 10),  Quaternion.Euler (0, waveRotation, 180));
					Rigidbody waveRB = waveInstance.GetComponent<Rigidbody>();
					waveRB.velocity = new Vector3(0, 0, -wavespeed);
				} 
				else {
					waveInstance = Instantiate (wave, new Vector3 (transform.position.x-75, 1, transform.position.z + 10),  Quaternion.Euler (0, waveRotation, 180));
					Rigidbody waveRB = waveInstance.GetComponent<Rigidbody>();
					waveRB.velocity = new Vector3(0, 0, wavespeed);
				}
			}

			else if(axis.ToLower() =="z"){
				if (transform.position.x > 0) {
					waveInstance = Instantiate (wave, new Vector3 (transform.position.x - 10, 1, transform.position.z-75),  Quaternion.Euler (0, waveRotation, 180));
					Rigidbody waveRB = waveInstance.GetComponent<Rigidbody>();
					waveRB.velocity = new Vector3(-wavespeed, 0, 0);
				} 
				else {
					waveInstance = Instantiate (wave, new Vector3 (transform.position.x + 10, 1, transform.position.z+75),  Quaternion.Euler (0, 0, 180));
					Rigidbody waveRB = waveInstance.GetComponent<Rigidbody>();
					waveRB.velocity = new Vector3(wavespeed, 0, 0);
				}
			}
			Destroy (waveInstance, 1.25f);
			StartCoroutine (disableButton ());
		

		}

	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

	private IEnumerator disableButton (){
		enabled = false;
		yield return new WaitForSeconds (1f);
		enabled = true;
	}

}
