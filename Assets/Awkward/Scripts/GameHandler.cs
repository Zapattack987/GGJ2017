using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameHandler : Singleton<GameHandler> {

    public List<GoalZone> goals;
    public List<UI_ItemDisplay> itemDisplays;

    [Header("Waving attributes")]
    public float waveTimeRefreshPeriod = 5.0f;
    private float _waveTimer;

    public float wavingPeopleMinRadius = 3;
    public float wavingPeopleRadius = 10;

    public List<Identifier> matchableItems;
    public int matchItemCount = 3;
    private List<Identifier> _matchItems;
    private GoalZone _activeGoal;

    [Header("UI")]
    public List<Text> InstructionsDisplay;
    public TimerBar TimerBar;

    private bool _inWave = false;
    private bool _lookForWave = false;
    private Person _wavingPerson;
    private Person _behindPerson;
    private Player _player;

    private List<Person> _people = new List<Person>();
    private Coroutine _wavingCoroutine;

    private int awkwardCount = 0;


    // ------------------------------------------
    // Use this for initialization
    void Start () {
        _waveTimer = waveTimeRefreshPeriod;
        _activeGoal = Helper.GetItem(goals);
        _activeGoal.Activate();

        _player = Player.Instance;

        // Randomly get some match items
        _matchItems = new List<Identifier>();
        for (var i = 0; i < matchItemCount; i++)
        {
            var newMatchItem = Helper.GetItem(matchableItems
                .Where(m => !_matchItems.Contains(m)).ToList());
            _matchItems.Add(newMatchItem);
        }

        // Assign them to UI displays
        for (var i = 0; i < itemDisplays.Count; i++)
        {
            var itemDisplay = itemDisplays[i];
            if (i < _matchItems.Count)
            {
                itemDisplay.SetItem(_matchItems[i]);
            } else
            {
                itemDisplay.SetItem(null);
            }
        }

        // Hide waving text popups at start
        InstructionsDisplay.ForEach(i =>
        {
            i.enabled = false;
        });
	}


    // ------------------------------------------
    // Update is called once per frame
    void Update () {
		
        if (!_inWave)
        {
            _waveTimer -= Time.deltaTime;
        }

        if (!_inWave && _wavingCoroutine != null)
        {
            StopCoroutine(_wavingCoroutine);
            _wavingCoroutine = null;
        }

        if (_waveTimer < 0 && !_inWave && !_lookForWave)
        {
            _lookForWave = true;
        }

        
        // LOOKING FOR A WAVE
        if (_lookForWave)
        {
            var closeEnoughPeople = _people
                .Where(p => p.canWave)
                .Where(p => Vector3.Distance(_player.transform.position, p.transform.position) <= wavingPeopleRadius)
                .Where(p => Vector3.Distance(_player.transform.position, p.transform.position) >= wavingPeopleMinRadius)
                .ToList();
            closeEnoughPeople = closeEnoughPeople.OrderBy(p => Random.Range(0, 1)).ToList();

            //foreach (var person in _people.Where(p => Vector3.Distance(_player.transform.position, p.transform.position) > wavingPeopleRadius))
            //{
            //    person.SetDebugIndicator(false);
            //}

            //print("People count is " + _people.Count);
            //print("Close people count is " + closeEnoughPeople.Count);

            //print("Forward is " + _player.transform.forward);
            for (var i = 0; i < closeEnoughPeople.Count; i++)
            {
                var firstPerson = closeEnoughPeople[i];

                // First person should be within 90 degree FOV of player forward
                var firstPersonAngleBetween = Vector3.Angle(_player.transform.forward, firstPerson.transform.position - _player.transform.position);

                if (Mathf.Abs(firstPersonAngleBetween) > 45)
                {
                    //print("Rejected first player for angle");
                    //firstPerson.SetDebugIndicator(false);
                    continue;
                }
                //else
                //{
                //    firstPerson.SetDebugIndicator(true);
                //}

                for (var j = 0; j < closeEnoughPeople.Count; j++)
                {
                    if (i == j) continue;

                    var secondPerson = closeEnoughPeople[j];

                    // Second person should be within 90 degree FOV of player backward
                    var secondPersonAngleBetween = Vector3.Angle(-_player.transform.forward, secondPerson.transform.position - _player.transform.position);
                    if (Mathf.Abs(secondPersonAngleBetween) > 45)
                    {
                        //print("Rejected second player for angle");
                        continue;
                    }

                    // Check if player falls roughly in straight line between people
                    var playerAngleOffOfLine = Vector3.Angle(_player.transform.forward, secondPerson.transform.position - firstPerson.transform.position);
                    if (Mathf.Abs(playerAngleOffOfLine) < 5 || Mathf.Abs(playerAngleOffOfLine) > 175)
                    {
                        _lookForWave = false;
                        _inWave = true;
                        _wavingPerson = firstPerson;
                        _behindPerson = secondPerson;

                        _wavingCoroutine = StartCoroutine(WavingCoroutine());
                        return;
                    } else
                    {
                        //print("Rejected line match");
                    }
                }
            }
        }
	}



    // ------------------------------------------
    private IEnumerator WavingCoroutine()
    {
        print("Starting waving coroutine");
        // Initialize wave here
        // Freeze player controls
        _player.SetInWave(true);

        // Make target (or targets) wave
        _wavingPerson.SetInWave(true);
        _behindPerson.SetInWave(true);

        // Zoom camera in manually on target
        _player.LookAt(_wavingPerson.gameObject, wavingPeopleMinRadius, wavingPeopleRadius);

        // Display UI controls
        TimerBar.Activate(Player.Instance.reactionTimeLimit);
        InstructionsDisplay.ForEach(i =>
        {
            i.enabled = true;
        });

        // Go into waiting for input loop
        var haveInput = false;
        var playerWaved = false;
        while (_player.reactionTimer > 0 && !haveInput)
        {
            // INPUT
            if (Input.GetButtonDown("Jump"))
            {
                print(Time.time + ": Got jump");
                playerWaved = true;
                haveInput = true;
            } else if (Input.GetButtonDown("Fire1"))
            {
                print(Time.time + ": Got fire");
                playerWaved = false;
                haveInput = true;
            }
            yield return null;
        }

        // EVALUATE WAVE
        var matchedFriendItems = GetMatchedItems(_wavingPerson);

        // Good wave
        if (playerWaved && haveInput && matchedFriendItems.Count > 0)
        {

        }
        // Bad wave
        else if (playerWaved && haveInput && matchedFriendItems.Count == 0)
        {
            awkwardCount++;
        }
        // Should have waved
        else if (!playerWaved && haveInput && matchedFriendItems.Count > 0)
        {
            awkwardCount++;
        }
        // Didn't wave
        else if (!haveInput)
        {
            awkwardCount++;
        }

        print(Time.time + ": " + awkwardCount);


        // TODO
        // Flip over to show other waving person here if necessary

        // Display UI controls
        TimerBar.Deactivate();
        InstructionsDisplay.ForEach(i =>
        {
            i.enabled = false;
        });

        print("Exiting wave");
        _wavingPerson.SetInWave(false);
        _behindPerson.SetInWave(false);
        _player.SetInWave(false);
        _waveTimer = waveTimeRefreshPeriod;
        _inWave = false;
    }



    // ------------------------------------------
    public void RegisterPerson(Person person)
    {
        _people.Add(person);
    }



    // ------------------------------------------
    public void GoalVisited()
    {
        var goalsRemaining = goals.Where(g => !g.visited).Count();
        print("GOAL VISITED, " + goalsRemaining + " REMAINING");

        if (goalsRemaining > 0)
        {
            _activeGoal = Helper.GetItem(goals.Where(g => !g.visited).ToList());
            _activeGoal.Activate();
        }
        else
        {
            print("YOU WIN");
        }
    }



    // ------------------------------------------
    private List<string> GetMatchedItems(Person person)
    {
        var matchItemNames = person.GetItemsWorn()
            .Select(i => GetInitialName(i.gameObject.name))
            .Where(i => _matchItems.Select(m => GetInitialName(m.gameObject.name)).ToList().Contains(i))
            .ToList();

        //print("Matched items:");
        //foreach (var itemName in matchItemNames)
        //{
        //    print(itemName);
        //}

        // Highlight matched items
        var highlightItems = new List<Identifier>();
        foreach (var item in _matchItems)
        {
            if (person.GetItemsWorn().Select(m => GetInitialName(m.gameObject.name)).ToList().Contains(GetInitialName(item.gameObject.name))) {
                highlightItems.Add(item);
            }
        }
        foreach (var item in person.GetItemsWorn())
        {
            if (_matchItems.Select(m => GetInitialName(m.gameObject.name)).ToList().Contains(GetInitialName(item.gameObject.name)))
            {
                highlightItems.Add(item);
            }
        }
        foreach (var item in highlightItems)
        {
            item.Highlight();
        }


        return matchItemNames;
    }


    private string GetInitialName(string name)
    {
        if (!name.Contains('('))
        {
            return name;
        } else
        {
            var trimIndex = name.IndexOf('(');
            return name.Substring(0, trimIndex);
        }
    }
}
