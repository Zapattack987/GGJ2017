using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Identifier : MonoBehaviour {

    private Renderer _renderer;
    private Color _initialColor;

	// Use this for initialization
	void Start () {
        _renderer = GetComponent<Renderer>();
        _initialColor = _renderer.material.color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    // ------------------------------------------
    public void Highlight()
    {
        StartCoroutine(HighlightCoroutine());
    }

    private IEnumerator HighlightCoroutine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            var color = Color.Lerp(Color.Green, _initialColor, t);
            _renderer.material.SetColor("_Color", color);
            yield return null;
        }
    }
}
