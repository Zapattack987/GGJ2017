using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveController : MonoBehaviour {
    public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var y = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            y += Time.deltaTime * 30.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            y -= Time.deltaTime * 30.0f;
        }
        transform.Rotate(0, y, 0);
        transform.Translate(-speed, 0, 0);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
	}
}
