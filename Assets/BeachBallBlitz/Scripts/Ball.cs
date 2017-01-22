using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour {

	private Rigidbody rb;
	private float rippleFactor = 25f;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	void Update () {

		rb.AddForce (Random.Range (-rippleFactor, rippleFactor), Random.Range (0, 0), Random.Range (-rippleFactor, rippleFactor), ForceMode.Impulse);

	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.tag == "Wall"){
			Destroy (this.gameObject);

		}
	}
		
}
