using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : Singleton<Player> {

    public float lookAtTargetTime = 1;
    private float _lookTimeElapsed = 0;
    public float reactionTimeLimit = 5;

    [HideInInspector]
    public float reactionTimer = 0;

    private bool _inWave = false;
    private Transform _lookTarget;
    private Quaternion _initialLookRotation;
    private float _initialCameraTilt;
    private FirstPersonController _fpsController;

    private Camera _camera;
    private float _initialFOV;
    private float _targetFOV;

    // ------------------------------------------
    // Use this for initialization
    void Start () {
        _fpsController = GetComponent<FirstPersonController>();
        _camera = Camera.main;
        _initialFOV = _camera.fieldOfView;
	}


    // ------------------------------------------
    // Update is called once per frame
    void Update () {
		
        if (_inWave && _lookTarget != null)
        {
            reactionTimer -= Time.deltaTime;
            _lookTimeElapsed += Time.deltaTime;

            if (_lookTimeElapsed < lookAtTargetTime)
            {
                //print(_initialLookRotation.eulerAngles);
                _fpsController.SetCamera(Quaternion.Lerp(_initialLookRotation,
                    Quaternion.LookRotation(_lookTarget.position.SetY(0) - transform.position.SetY(0), Vector3.up),
                    _lookTimeElapsed / lookAtTargetTime),
                    Mathf.Lerp(_initialCameraTilt, 0, _lookTimeElapsed / lookAtTargetTime));

                _camera.fieldOfView = Mathf.Lerp(_initialFOV, _targetFOV, _lookTimeElapsed / lookAtTargetTime);
                    //Quaternion.Lerp(_initialLookRotation,
                    //Quaternion.LookRotation(_lookTarget.position.SetY(0) - transform.position.SetY(0), Vector3.up),
                    //_lookTimeElapsed / lookAtTargetTime).eulerAngles.x);
                //transform.rotation = Quaternion.Lerp(_initialLookRotation,
                //    Quaternion.LookRotation(_lookTarget.position.SetY(0) - transform.position.SetY(0), Vector3.up),
                //    _lookTimeElapsed / lookAtTargetTime);
            }
        }

        // Zoom back out after look
        else if (!_inWave && _camera.fieldOfView < _initialFOV && _lookTimeElapsed < lookAtTargetTime)
        {
            _lookTimeElapsed += Time.deltaTime;
            _camera.fieldOfView = Mathf.Lerp(_targetFOV, _initialFOV, _lookTimeElapsed / lookAtTargetTime);
        }
	}


    // ------------------------------------------
    // If we are waving, disable player movement
    // and look control for the duration
    public void SetInWave(bool inWave)
    {
        _inWave = inWave;
        if (_inWave)
        {
            reactionTimer = reactionTimeLimit;
            _fpsController.controlsEnabled = false;
        }
        else
        {
            _fpsController.controlsEnabled = true;
            _lookTimeElapsed = 0;
        }
    }


    // ------------------------------------------
    public void LookAt(GameObject target, float minDistance, float maxDistance)
    {
        _lookTarget = target.GetComponent<Person>().transform;
        _initialLookRotation = transform.rotation;
        _initialCameraTilt = _fpsController.GetCameraRot().eulerAngles.x;
        if (_initialCameraTilt > 180)
        {
            _initialCameraTilt -= 180;
        } else if (_initialCameraTilt < -180)
        {
            _initialCameraTilt += 180;
        }
        _lookTimeElapsed = 0;

        var distance = Vector3.Distance(transform.position, target.transform.position);
        _targetFOV = Mathf.Lerp(30, 15, Mathf.Clamp01(
            (distance - minDistance) / (maxDistance - minDistance)));
    }


    // ------------------------------------------
    void OnTriggerEnter(Collider col)
    {
        var goalZone = col.GetComponent<GoalZone>();
        if (goalZone == null || !goalZone.activeZone) return;

        // TODO
        // Show congratulations message
        goalZone.Visit();
    }
}
