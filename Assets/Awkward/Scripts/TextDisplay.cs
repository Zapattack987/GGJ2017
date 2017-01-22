using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour {

    private Vector3 _initialScale;
    private bool _keepOpen;

	// Use this for initialization
	void Start () {
        _initialScale = transform.localScale;
        transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    // ------------------------------------------
    public void Activate(bool keepOpen = false)
    {
        _keepOpen = keepOpen;
        StartCoroutine(DisplayCoroutine());
    }



    // ------------------------------------------
    private IEnumerator DisplayCoroutine()
    {
        float t = 0;
        while (t < .5)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, _initialScale, t / 0.5f);
            yield return null;
        }

        if (_keepOpen) yield break;

        yield return new WaitForSeconds(3);

        t = 0;
        while (t < .5)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t / 0.5f);
            yield return null;
        }
    }


    // ------------------------------------------
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
