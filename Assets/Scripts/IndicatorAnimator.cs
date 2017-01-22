using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorAnimator : MonoBehaviour {

    public float heightVariation = 1;
    public float heightVariationPeriod = 2;

    private Vector3 _initialPos;
    private Renderer _renderer;



    // ------------------------------------------
    // Turn off all goal zones initially
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.enabled = false;
    }



    // ------------------------------------------
    // Use this for initialization
    void Start () {
        _initialPos = transform.position;
	}


    // ------------------------------------------
    // Update is called once per frame
    void Update () {
		if (_renderer.enabled)
        {
            var heightOffset = Mathf.Cos(Time.time * 2 * Mathf.PI / heightVariationPeriod) * heightVariation;
            transform.position = new Vector3(_initialPos.x,
                _initialPos.y + heightOffset,
                _initialPos.z);
        }
	}



    // ------------------------------------------
    public void Activate()
    {
        print("Activating indicator");
        _renderer.enabled = true;
    }



    // ------------------------------------------
    public void Deactivate()
    {
        _renderer.enabled = false;
    }
}
