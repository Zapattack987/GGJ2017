using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Person : MonoBehaviour {

    public float minTimeBetweenWaves = 20;
    private float _timeUntilCanWaveAgain = 0;
    [HideInInspector]
    public bool canWave = true;

    public float lookAtTargetTime = 1;
    private float _lookTimeElapsed = 0;
    private Quaternion _initialLookRotation;

    public List<Identifier> hats;
    public Transform hatAttach;
    private GameObject _hat;

    public List<Identifier> faces;
    public Transform faceAttach;
    private GameObject _face;

    private NavMeshAgent _navMeshAgent;
    private List<GoalZone> _goals;

    private GoalZone _currentGoal;
    private Renderer _renderer;
    private Animator _animator;

    private bool _inWave = false;

    // ------------------------------------------
    // Use this for initialization
    void Start () {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _goals = GameHandler.Instance.goals;
        _currentGoal = Helper.GetItem(_goals);
        _navMeshAgent.destination = _currentGoal.transform.position;

        var hatPrefab = Helper.GetItem(hats).gameObject;
        _hat = (GameObject)Instantiate(hatPrefab);
        _hat.transform.parent = hatAttach.transform;
        _hat.transform.position = hatAttach.position;

        var facePrefab = Helper.GetItem(faces).gameObject;
        _face = (GameObject)Instantiate(facePrefab);
        _face.transform.parent = faceAttach.transform;
        _face.transform.position = faceAttach.position;

        GameHandler.Instance.RegisterPerson(this);

        _renderer = GetComponentInChildren<Renderer>();
        _animator = GetComponent<Animator>();
    }


    // ------------------------------------------
    // Update is called once per frame
    void Update () {


		if (_inWave)
        {
            if (_lookTimeElapsed < lookAtTargetTime)
            {
                _lookTimeElapsed += Time.deltaTime;

                if (_lookTimeElapsed < lookAtTargetTime)
                {
                    transform.rotation = Quaternion.Lerp(_initialLookRotation,
                        Quaternion.LookRotation(Player.Instance.transform.position.SetY(0) - transform.position.SetY(0), Vector3.up),
                        _lookTimeElapsed / lookAtTargetTime);
                }
            }
        }

        // Count down until we can wave again
        else
        {
            if (!canWave && _timeUntilCanWaveAgain > 0)
            {
                _timeUntilCanWaveAgain -= Time.deltaTime;
            } else if (!canWave && _timeUntilCanWaveAgain <= 0)
            {
                canWave = true;
            }
        }
	}


    // ------------------------------------------
    void OnTriggerEnter(Collider col)
    {
        var goal = col.GetComponent<GoalZone>();
        if (goal == null || goal != _currentGoal) return;

        //print("Agent reached goal");
        _currentGoal = Helper.GetItem(_goals.Where(g => g != _currentGoal).ToList());
        _navMeshAgent.destination = _currentGoal.transform.position;
    }



    // ------------------------------------------
    public void SetInWave(bool inWave)
    {
        _inWave = inWave;
        if (inWave)
        {
            _navMeshAgent.enabled = false;
            _lookTimeElapsed = 0;
            _initialLookRotation = transform.rotation;
            canWave = false;
            _animator.SetTrigger("Start Waving");
        } else
        {
            _navMeshAgent.enabled = true;
            _navMeshAgent.destination = _currentGoal.transform.position;
            _timeUntilCanWaveAgain = minTimeBetweenWaves;
            _animator.SetTrigger("Stop Waving");
        }
    }



    // ------------------------------------------
    public void SetDebugIndicator(bool debugStatus)
    {
        if (debugStatus)
        {
            _renderer.material.SetColor("_Color", Color.green);
        } else
        {
            _renderer.material.SetColor("_Color", Color.white);
        }
    }
}
