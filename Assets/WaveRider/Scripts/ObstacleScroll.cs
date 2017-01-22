using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScroll : MonoBehaviour
{
    public float speed;
    void Start ()
    {
	}
	void Update ()
    {
        transform.Translate(new Vector3(0, -(speed), 0) * Time.deltaTime);
    }
}
