using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour {

    [HideInInspector]
    public bool activeZone = false;

    [HideInInspector]
    public bool visited = false;

    public IndicatorAnimator indicator;

    public string goalName = "";


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    // ------------------------------------------
    public void Activate()
    {
        // Show text
        print("Activating zone " + gameObject.name);
        activeZone = true;
        indicator.Activate();
    }




    // ------------------------------------------
    public void Visit()
    {
        visited = true;
        activeZone = false;
        indicator.Deactivate();
        GameHandler.Instance.GoalVisited();
    }
}
