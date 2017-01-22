using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    int frame;
    int nextSpawnFrame;
    public int FPS;
    public float frequency;
    private float FramesBetweenSpawns;
    public GameObject[] boats;
    public float spawnDist;
    // Use this for initialization
    private GameObject Wave;
    void Start ()
    {
        frame = 0;
        FramesBetweenSpawns = (float)FPS;
        nextSpawnFrame = 60;
        Wave = GameObject.Find("Wave");
	}
    void FixedUpdate()
    {
        if (frame == nextSpawnFrame && Wave != null)
        {
            Vector3 waveDirection = Wave.transform.localRotation.eulerAngles;
            float radians = waveDirection.y * (Mathf.PI / 180);
            Vector3 spawnPoint = Wave.transform.position + (new Vector3(Mathf.Cos(radians) * spawnDist, 0, Mathf.Sin(radians) * -spawnDist))  ;
            spawnPoint = new Vector3(spawnPoint.x + Random.Range(-150, 150), 2, spawnPoint.z + Random.Range(-150, 150));
            float spawning = 0f;
            if(Wave.transform.localScale.x < 10)
            {
                spawning = 1f;
            }
            else if (Wave.transform.localScale.x < 100)
            {
                spawning = 2f;
            }
            else if (Wave.transform.localScale.x < 300)
            {
                spawning = 3f;
            }
            else if (Wave.transform.localScale.x < 1000)
            {
                spawning = 4f;
            }
            else if (Wave.transform.localScale.x < 3000)
            {
                spawning = 5f;
            }
            GameObject spawned;
            float item = Random.Range(0.0f, 1.0f);
            if (item > .99)
            {
                Vector3 upper = new Vector3(0, 70.2f, 0) * spawning;
                spawned = Instantiate(boats[2], spawnPoint + upper, Quaternion.Euler(0, 0, 0));
                spawned.transform.localScale = spawned.transform.localScale * spawning;
                spawned.GetComponent<ImmaBoat>().spawnDist = spawnDist;
            }
            else if (item > .90)
            {
                Vector3 upper = new Vector3(0, 3, 0) * spawning;
                spawned = Instantiate(boats[1], spawnPoint + upper, Quaternion.Euler(0, 0, 0));
                spawned.transform.localScale = spawned.transform.localScale * spawning;
                spawned.GetComponent<ImmaBoat>().spawnDist = spawnDist;
            }
            else
            {
                Vector3 upper = new Vector3(0, 1, 0) * spawning;
                spawned = Instantiate(boats[0], spawnPoint, Quaternion.Euler(-90, 90, 180));
                spawned.transform.localScale = new Vector3(1f, 1f, 1f) * spawning;
                spawned.GetComponent<ImmaBoat>().spawnDist = spawnDist;
            }
            nextSpawnFrame = 60 - (int)(Random.Range(10.0f, 45.0f) * frequency);
            if (nextSpawnFrame <= 0)
            {
                nextSpawnFrame = 10;
            }
            frame = 0;
        }
        else { frame++; }
    }
    void Update () {
		
	}
}
