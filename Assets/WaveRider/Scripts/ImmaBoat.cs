﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmaBoat : MonoBehaviour {
    GameObject Wave;
    int time;
    int frame;
    public float spawnDist = 0;
	// Use this for initialization
	void Start ()
    {
        frame = 0;
        time = 60;
        Wave = GameObject.Find("Wave");
    }
	
	// Update is called once per frame
	void Update () {
		if ( frame == time && Wave != null)
        {
            frame = 0;
            if (Vector3.Distance(Wave.transform.position, transform.position) >= spawnDist)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            frame++;
        }
	}
}
