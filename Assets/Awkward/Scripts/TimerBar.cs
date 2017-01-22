using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBar : MonoBehaviour {

    private float _lifespan;
    private float _timeRemaining;

    private Renderer _renderer;
    private Vector3 _initialScale;

	// Use this for initialization
	void Start () {
        _renderer = GetComponent<Renderer>();
        _initialScale = transform.localScale;
        _renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
        if (_timeRemaining > 0)
        {
            _timeRemaining -= Time.deltaTime;

            var lerpVal = 1 - (_timeRemaining / _lifespan);
            var xScale = Mathf.Lerp(_initialScale.x, 0, lerpVal);
            transform.localScale = _initialScale.SetX(xScale);

            var newColor = Color.Lerp(Color.yellow, Color.red, lerpVal);
            _renderer.material.SetColor("_Color", newColor);
        }

        else if (_timeRemaining <= 0 && _renderer.enabled)
        {
            _renderer.enabled = false;
        }
	}



    // ------------------------------------------
    public void Activate(float lifespan)
    {
        _lifespan = lifespan;
        _timeRemaining = lifespan;
        _renderer.enabled = true;
    }



    // ------------------------------------------
    public void Deactivate()
    {
        _timeRemaining = 0;
        _renderer.enabled = false;
    }
}
