using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveRunner : MonoBehaviour {
    Camera cam;
    public float scaleRate;
    GameObject ocean;
    GameObject killer;
    public bool gameover;
	// Use this for initialization
	void Start () {
        cam = (Camera)gameObject.GetComponentInChildren(typeof(Camera));
        ocean = GameObject.Find("Ocean");
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = transform.localScale + (new Vector3(.003f, .003f, .003f) * scaleRate);
        gameObject.GetComponent<WaveController>().speed += (.001f * scaleRate);
        cam.farClipPlane += (scaleRate);
        ocean.GetComponent<ObstacleSpawner>().spawnDist += (.015f * scaleRate);
        ocean.GetComponent<ObstacleSpawner>().frequency += (.0001f * scaleRate);
        if (gameover)
        {
            cam.transform.LookAt(killer.transform);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "ocean")
        {
            if (collision.gameObject.tag == "raft")
            {
                if (collision.gameObject.transform.localScale.x < transform.localScale.x)
                {
                    cam.gameObject.GetComponent<ScoreKeeper>().rafts++;
                    Destroy(collision.gameObject, .5f);
                }
                else
                {
                    //game over
                    killer = collision.gameObject;
                    cam.gameObject.transform.parent = collision.transform;
                    gameover = true;
                    Destroy(gameObject, .1f);
                }
            }
            else if (collision.gameObject.tag == "pirate")
            {
                if (collision.gameObject.transform.localScale.x / 50 < transform.localScale.x)
                {
                    cam.gameObject.GetComponent<ScoreKeeper>().pirateShips++;
                    Destroy(collision.gameObject, .5f);
                }
                else
                {
                    //game over
                    killer = collision.gameObject;
                    cam.gameObject.transform.parent = collision.transform;
                    gameover = true;
                    Destroy(gameObject, .1f);
                }
            }
            else if (collision.gameObject.tag == "war")
            {
                if (collision.gameObject.transform.localScale.x / 40 < transform.localScale.x)
                {
                    cam.gameObject.GetComponent<ScoreKeeper>().warShips++;
                    Destroy(collision.gameObject, .5f);
                }
                else
                {
                    //game over
                    killer = collision.gameObject;
                    cam.gameObject.transform.parent = collision.transform;
                    gameover = true;
                    Destroy(gameObject, .1f);
                }
            }
            if (gameover)
            {
                GameObject.Find("GameOver").GetComponent<Text>().text = "GAME OVER, MAN!";
            }
        }
    }
}
