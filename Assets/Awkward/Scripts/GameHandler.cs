using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

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
    public List<GameObject> InstructionsBackdrops;
    public Text DestinationText;
    public TimerBar TimerBar;

    public TextDisplay awkwardDisplay;
    public TextDisplay staredDisplay;
    public TextDisplay notYourFriendDisplay;
    public TextDisplay sadFriendDisplay;
    public TextDisplay goodJobDisplay;

    public TextDisplay awkward1;
    public TextDisplay awkward2;
    public TextDisplay awkward3;

    public Canvas GameCanvas;
    public TextDisplay winDisplay;
    public TextDisplay loseDisplay;
    public List<Text> endGameText;
    public List<GameObject> endGameBackdrops;

    private bool _inWave = false;
    private bool _lookForWave = false;
    private Person _wavingPerson;
    private Person _behindPerson;
    private Player _player;

    private List<Person> _people = new List<Person>();
    private Coroutine _wavingCoroutine;

    private int awkwardCount = 0;
    private bool gameOver = false;


    // ------------------------------------------
    // Use this for initialization
    void Start () {

        Cursor.visible = false;
        _waveTimer = waveTimeRefreshPeriod;
        _activeGoal = Helper.GetItem(goals);
        _activeGoal.Activate();
        DestinationText.text = _activeGoal.goalName;

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
        InstructionsBackdrops.ForEach(b =>
        {
            b.SetActive(false);
        });

        endGameText.ForEach(t =>
        {
            t.enabled = false;
        });
        endGameBackdrops.ForEach(b =>
        {
            b.SetActive(false);
        });
	}


    // ------------------------------------------
    // Update is called once per frame
    void Update () {
		

        // QUIT THE GAME
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }

        // GAME OVER SCREEN
        if (gameOver)
        {
            _player.Deactivate();

            if (Input.GetButtonDown("Jump"))
            {
                SceneManager.LoadScene("Awkward");
            } else if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("MainMenu");
            }
            return;
        }



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
        InstructionsBackdrops.ForEach(b =>
        {
            b.SetActive(true);
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
        bool needToShowOtherWave = false;
        bool awkwardnessUp = false;

        print("Got matched friend items:");
        foreach (var item in matchedFriendItems)
        {
            print(item);
        }
        // Good wave
        if (playerWaved && haveInput && matchedFriendItems.Count > 0)
        {
            goodJobDisplay.Activate();
        }
        // Bad wave
        else if (playerWaved && haveInput && matchedFriendItems.Count == 0)
        {
            notYourFriendDisplay.Activate();
            awkwardDisplay.Activate();
            awkwardCount++;
            needToShowOtherWave = true;
            awkwardnessUp = true;
        }
        // Should have waved
        else if (!playerWaved && haveInput && matchedFriendItems.Count > 0)
        {
            sadFriendDisplay.Activate();
            awkwardDisplay.Activate();
            awkwardCount++;
            awkwardnessUp = true;
        }
        // Didn't wave, correctly
        else if (!playerWaved && haveInput && matchedFriendItems.Count == 0)
        {
            goodJobDisplay.Activate();
        }
        // Didn't decide
        else if (!haveInput)
        {
            staredDisplay.Activate();
            awkwardDisplay.Activate();
            awkwardCount++;
            awkwardnessUp = true;
        }

        if (awkwardnessUp)
        {
            if (awkwardCount == 1)
            {
                awkward1.Activate(true);
            } else if (awkwardCount == 2)
            {
                awkward1.Deactivate();
                awkward2.Activate(true);
            } else if (awkwardCount == 3)
            {
                awkward2.Deactivate();
                awkward3.Activate(true);
            }
        }
        print(Time.time + ": Awkwardness: " + awkwardCount);

        // Display UI controls
        TimerBar.Deactivate();
        InstructionsDisplay.ForEach(i =>
        {
            i.enabled = false;
        });
        InstructionsBackdrops.ForEach(b =>
        {
            b.SetActive(false);
        });

        // Flip over to show other waving person here if necessary
        if (needToShowOtherWave)
        {
            _player.LookAt(_behindPerson.gameObject, wavingPeopleMinRadius, wavingPeopleRadius);
            yield return new WaitForSeconds(3);
        }

        print("Exiting wave");
        _wavingPerson.SetInWave(false);
        _behindPerson.SetInWave(false);
        _player.SetInWave(false);
        _waveTimer = waveTimeRefreshPeriod;
        _inWave = false;

        // YOU LOSE
        if (awkwardCount > 3)
        {
            GameCanvas.enabled = false;
            loseDisplay.Activate();
            gameOver = true;

            endGameText.ForEach(t =>
            {
                t.enabled = true;
            });
            endGameBackdrops.ForEach(b =>
            {
                b.SetActive(true);
            });
        }
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

        if (goalsRemaining > 0)
        {
            _activeGoal = Helper.GetItem(goals.Where(g => !g.visited).ToList());
            _activeGoal.Activate();
            DestinationText.text = _activeGoal.goalName;
            waveTimeRefreshPeriod *= 0.87f;
        }
        else
        {
            GameCanvas.enabled = false;
            winDisplay.Activate();
            gameOver = true;
            endGameText.ForEach(t =>
            {
                t.enabled = true;
            });
            endGameBackdrops.ForEach(b =>
            {
                b.SetActive(true);
            });
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
            if (matchItemNames.Contains(GetInitialName(item.gameObject.name))) {
                highlightItems.Add(item);
            }
        }
        foreach (var item in person.GetItemsWorn())
        {
            if (matchItemNames.Contains(GetInitialName(item.gameObject.name)))
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
