using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

/*
 * Ok, so, this file is long as heck. So here's an explanation.
 * Basically, the battle class gets attached to an empty gameobject in the scene,
 * and handles all of the enemyAI and move handling,
 * along with player interactivity. 
 * The EnemyMove method is the longest, but it's not too complex if ya sit and think. And also breathe. A lot.
 * You can do this. :)
 */

/*
 * Why is update always stuff that doesnt need to be called every frame?? >.<
 * Only use I can think of off the top of my head for an update loop
 * in a turn-based game would be idle animation, aka wiggles and bounces.
 */

public class BattleClass : MonoBehaviour
{
    public GameObject canvas;
    private GameObject canvasDuplicate;
    public GameObject PartyButtons;
    public GameObject RhythmButtons;
    public GameObject EQButton;

    public float timing = 0.3f;
    public static int tutorial;

    public Camera camera;

    public string toDo;

    public PartyMemberClass leader;
    private PartyMemberClass[] party;
    public static string difficulty = "Medium";
    public static bool initialized = false;

    private static int maxPartySize = 3;
    public int maxRowSize = 1;
    private static int maxNumberOfRows = 3;

    public GameObject[] front;
    public GameObject[] mid;
    public GameObject[] back;
    public EnemyClass[,] enemies;

    public PartyMemberClass[] restOfParty;

    private MoveClass currentMove;

    private int harmonicAmount = 1;

    private bool recentlyChained = false;

    private bool currentlyBreaking = false;

    private bool groupMove = false;
    private bool lastCrit = false;

    private bool chainedAgain = false;

    //Booleans for the Update loop state machine
    private bool runItParty = false;
    private bool isItRunningParty = false;
    private bool runItEnemy = false;
    private bool isItRunningEnemy = false;
    private bool click = false;

    private List<ChainClass> chains = new List<ChainClass>();
    List<PartyMemberClass> partyMembersChained = new List<PartyMemberClass>();

    private void Awake()
    {
        if (!initialized)
        {
            tutorial = 1;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            SceneManager.LoadSceneAsync("Intro Menu");
        }
        enemies = new EnemyClass[maxNumberOfRows, maxRowSize];
        //KnownInfo.InitializeJSON();
        canvasDuplicate = Instantiate(canvas);
        SetUpEnemies();
        SetUpParty();
        runItParty = true;
        isItRunningParty = true;
        toDo = null; // false :P
    }

    private void Update()
    {
        //Check for win and loss
        bool haveLost = true;
        bool haveWon = true;

        foreach(GameObject child in front)
        {
            if (child.activeSelf == true)
            {
                haveWon = false;
            }
        }
        foreach (GameObject child in mid)
        {
            if (child.activeSelf == true)
            {
                haveWon = false;
            }
        }
        foreach (GameObject child in back)
        {
            if (child.activeSelf == true)
            {
                haveWon = false;
            }
        }
        foreach(PartyMemberClass child in party) 
        {
            if(child != null)
            {
                if(child.currentHealth > 0)
                {
                    haveLost = false;
                }
            }
        }

        if (haveLost)//Loss
        {
            tutorial = 1;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            SceneManager.LoadSceneAsync("Intro Menu");
        }
        if(haveWon)//Win
        {
            //KnownInfo.UpdateJSON();
            tutorial++;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Transition.dif = "Battle " + tutorial.ToString();
            SceneManager.LoadScene("Transition");
        }
        else
        {
            if (runItParty)
            {
                if (isItRunningParty) { StartCoroutine(PartyPhase()); }
            }
            else if (runItEnemy)
            {
                if (isItRunningEnemy) { StartCoroutine(EnemyPhase()); }
            }
        }
        
    }

    public void SetUpEnemies()
    {
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                switch (x)
                { //The cases change wether the end result goes to the front, mid, or back for various variables
                    case 0:
                        enemies[x, y] = front[y].GetComponentInChildren<EnemyClass>();
                        // from here
                        float xPosFront;
                        if(maxRowSize == 1)
                        {
                            xPosFront = 0;
                        }
                        else
                        {
                            xPosFront = (y - (Mathf.Abs(Mathf.Abs(x - 1) - ((maxRowSize - 1) / 2)) / 2)) * (Screen.width / 250) - (Screen.width / 250);
                        }
                        float yPosFront = x * 4.2f + (front[y].gameObject.transform.localScale.y / 2) + 1;
                        float zPosFront = -30 - (x * 10);
                        Vector3 positionFront = new Vector3(xPosFront, yPosFront, zPosFront);
                        // to here could be a function
                        positionFront.y += (front[y].gameObject.transform.localScale.y / 2);
                        front[y].transform.position = positionFront;

                        break;
                    case 1:
                        enemies[x, y] = mid[y].GetComponentInChildren<EnemyClass>();

                        float xPosMid;
                        if (maxRowSize == 1)
                        {
                            xPosMid = 0;
                        }
                        else
                        {
                            xPosMid = (y - (Mathf.Abs(Mathf.Abs(x - 1) - ((maxRowSize - 1) / 2)) / 2)) * (Screen.width / 250) - (Screen.width / 250);
                        }
                        float yPosMid = x * 4.2f + (mid[y].gameObject.transform.localScale.y / 2) + 1;
                        float zPosMid = -30 - (x * 10);
                        Vector3 positionMid = new Vector3(xPosMid, yPosMid, zPosMid);

                        positionMid.y += (mid[y].gameObject.transform.localScale.y / 2);
                        mid[y].gameObject.transform.position = positionMid;

                        break;
                    case 2:
                        enemies[x, y] = back[y].GetComponentInChildren<EnemyClass>();

                        float xPosBack;
                        if (maxRowSize == 1)
                        {
                            xPosBack = 0;
                        }
                        else
                        {
                            xPosBack = (y - (Mathf.Abs(Mathf.Abs(x - 1) - ((maxRowSize - 1) / 2)) / 2)) * (Screen.width / 250) - (Screen.width / 250);
                        }
                        float yPosBack = x * 4.2f + (back[y].gameObject.transform.localScale.y / 2) + 1;
                        float zPosBack = -30 - (x * 10);
                        Vector3 positionBack = new Vector3(xPosBack, yPosBack, zPosBack);

                        positionBack.y += (back[y].gameObject.transform.localScale.y / 2);
                        back[y].gameObject.transform.position = positionBack;

                        break;
                    default:
                        Debug.Log("Fell into default in setupenemies in battleclass");
                        break;
                }
            }
        }
    }

    public void SetUpParty()
    {
        //initializing party array with random assortment besides leader
        party = new PartyMemberClass[restOfParty.Length + 1];
        int currentSlot = 0;
        if(restOfParty.Length == 0)
        {
            party[0] = leader;
        }
        else
        {
            for (float x = restOfParty.Length; x >= 0; x--)
            {
                if (x == restOfParty.Length) { party[currentSlot] = leader; currentSlot++; } //put leader in first
                else if (x == 0) { party[currentSlot] = restOfParty[0]; currentSlot++; } //don't random search for the last entry
                else
                {
                    int select = Mathf.RoundToInt(Random.Range(0, x)); //selects a random party member in array
                    party[currentSlot] = restOfParty[select]; //slots it in position
                    while (select < restOfParty.Length - 1) //moves the rest of the future members up array for future random selection
                    {
                        restOfParty[select] = restOfParty[select + 1];
                        select++;
                    }
                    currentSlot++;
                }
            }
        }

    }

    /*
     *
     * WORK NEEDS TO BE DONE HERE
     * 
     */
    //Interactivity code for party member turn
    IEnumerator PartyMemberTurn(PartyMemberClass person, int leader)
    {
        Debug.Log("In " + person.memberName + "'s move");
        person.currentlyGuarding = false;
        toDo = null;
        currentlyBreaking = false;
        var partyButtons = Instantiate(PartyButtons);
        var rhythmButtons = Instantiate(RhythmButtons);
        var EQMenu = Instantiate(EQButton);
        var xPos = person.gameObject.transform.position.x;
        var zPos = person.gameObject.transform.position.z;
        float shift = -Mathf.Sign(xPos) * -(zPos + 20) * 20;

        //POSITIONING UI ELEMENTS
        Vector3 temp = new Vector3(xPos * (Screen.width / (Screen.width / 40)) + (shift), (zPos + 18) * (-0.2f * Mathf.Abs(21 + zPos)) * (Screen.height / (Screen.height / 40)) + (shift / 2), 1);
        partyButtons.gameObject.transform.position = temp;
        partyButtons.transform.SetParent(canvas.transform, false);
        foreach (Button but in partyButtons.GetComponentsInChildren<Button>())
        {
            but.onClick.AddListener(() => changeToDo(but.GetComponentInChildren<Text>().text));
            if (but.GetComponentInChildren<Text>().text == "EQ")
            {
                if(leader != 0 || tutorial < 2)
                {
                    but.gameObject.SetActive(false);
                }
            }
            else if ((but.GetComponentInChildren<Text>().text == "BREAK") || (but.GetComponentInChildren<Text>().text == "Harmonic"))
            {
                if(person.currentlyChained == false || tutorial < 3)
                {
                    but.gameObject.SetActive(false);
                }
            }
        }

        //readjust for stuff in ground
        rhythmButtons.gameObject.transform.position = temp;
        rhythmButtons.transform.SetParent(canvas.transform, false);
        for (int x = 0; x < person.moves.Length; x++)
        {
            MoveClass moveIt = person.moves[x].GetComponent<MoveClassWrapper>().MoveClass;
            string title = "Button " + x;
            Button thing = rhythmButtons.transform.Find(title).gameObject.GetComponent<Button>();
            thing.gameObject.GetComponent<MoveClassWrapper>().MoveClass = moveIt;
            thing.onClick.AddListener(() => changeToDo("Rhythm Targeting"));
            thing.onClick.AddListener(() => UpdateMove(moveIt, person));
            thing.gameObject.GetComponentInChildren<Text>().text = person.moves[x].GetComponent<MoveClassWrapper>().MoveClass.moveName;
        }
        for (int x = person.moves.Length; x < 8; x++)
        {
            string title = "Button " + x;
            Button thing = rhythmButtons.transform.Find(title).gameObject.GetComponent<Button>();
            thing.gameObject.SetActive(false);
        }
        string temporaryButton = "Button 8";
        Button tempButt = rhythmButtons.transform.Find(temporaryButton).gameObject.GetComponent<Button>();
        tempButt.onClick.AddListener(() => changeToDo(null));
        tempButt.gameObject.GetComponentInChildren<Text>().text = "Go Back";

        EQMenu.gameObject.transform.position = temp;
        EQMenu.transform.SetParent(canvas.transform, false);
        Button goBackBut = EQMenu.transform.GetComponent<Button>();
        goBackBut.onClick.AddListener(() => changeToDo(null));
        goBackBut.gameObject.GetComponentInChildren<Text>().text = "Go Back";

        EQMenu.SetActive(false);
        rhythmButtons.SetActive(false);
        partyButtons.SetActive(true);

        int EQPoints = 2;

        Texture2D cursor = Resources.Load("UI/target reticle") as Texture2D;
        Vector2 hotSpot = new Vector2(cursor.width / 2f, cursor.height / 2f);

        while (toDo == null && currentlyBreaking == false && groupMove == false && person.currentHealth > 0 && chainedAgain == false && person.chainedBefore == false)
        {
            //PUT INTERACTIVE STUFF HERE
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            EQMenu.SetActive(false);
            partyButtons.SetActive(true);
            yield return null;

            while(toDo == "EQ")
            {
                Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
                Debug.Log("EQ");

                partyButtons.SetActive(false);
                EQMenu.SetActive(true);
                RaycastHit hit;
                Ray ray;

                yield return _WaitForInputClick();
                GameObject enemyClicked = null;
                int xEnemy = -1;
                int yEnemy = -1;

                if(toDo == "EQ")
                {
                    ray = camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if(hit.transform.gameObject.GetComponent<EnemyClass>() != null)
                        {
                            toDo = "EQ Release";
                            enemyClicked = hit.transform.gameObject;
                            for(int x = 0; x < maxNumberOfRows; x++)
                            {
                                for(int y = 0; y < maxRowSize; y++)
                                {
                                    if(enemies[x, y] == enemyClicked.GetComponent<EnemyClass>())
                                    {
                                        xEnemy = x;
                                        yEnemy = y;
                                    }
                                }
                            }
                        }
                    }
                }

                if(toDo == "EQ Release")
                {
                    yield return _WaitForInputClickLift();
                    if(person.currentEP < EQPoints)
                    {
                        DamagePopup.Create(new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - 10), "NO EP", 1, 0, 1);
                        EQMenu.SetActive(false);
                        partyButtons.SetActive(true);
                        toDo = null;
                        continue;
                    }

                    ray = camera.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(ray, out hit))
                    {
                        bool didItHit = false;
                        for(int x = 0; x < maxNumberOfRows; x++)
                        {
                            for(int y = 0; y < maxRowSize; y++)
                            {
                                if (hit.transform.gameObject.GetComponent<EnemyClass>() == enemies[x, y]) //for each flat, if you've hit THAT flat when releasing the mouse
                                {
                                    //DO THE EXCHANGE
                                    didItHit = true;
                                    if(x == 0)
                                    {
                                        GameObject vic = front[y];
                                        front[y] = enemyClicked.transform.parent.gameObject;
                                        //from here
                                        if (xEnemy == 0) { front[yEnemy] = vic; }
                                        else if (xEnemy == 1) { mid[yEnemy] = vic; }
                                        else if (xEnemy == 2) { back[yEnemy] = vic; }

                                        SetUpEnemies();

                                        Debug.Log(x + " " + y + " switched with " + xEnemy + " " + yEnemy);
                                        //to here could be a function
                                    }
                                    else if(x == 1)
                                    {
                                        GameObject vicMid = mid[y];
                                        mid[y] = enemyClicked.transform.parent.gameObject;
                                        if (xEnemy == 0) { front[yEnemy] = vicMid; }
                                        else if (xEnemy == 1) { mid[yEnemy] = vicMid; }
                                        else if (xEnemy == 2) { back[yEnemy] = vicMid; }

                                        SetUpEnemies();

                                        Debug.Log(x + " " + y + " switched with " + xEnemy + " " + yEnemy);
                                    }
                                    else if(x == 2)
                                    {
                                        GameObject vicB = back[y];
                                        back[y] = enemyClicked.transform.parent.gameObject;
                                        if (xEnemy == 0) { front[yEnemy] = vicB; }
                                        else if (xEnemy == 1) { mid[yEnemy] = vicB; }
                                        else if (xEnemy == 2) { back[yEnemy] = vicB; }

                                        SetUpEnemies();

                                        Debug.Log(x + " " + y + " switched with " + xEnemy + " " + yEnemy);
                                    }
                                    else
                                    {
                                        throw new System.Exception("EQ method failed");
                                    }

                                }
                                else
                                {
                                    //PUT IT BACK
                                }
                            }
                        }

                        if(didItHit)
                        {
                            person.currentEP = person.currentEP - EQPoints;
                            EQPoints = EQPoints * 2;
                        }
                    }

                    toDo = "EQ";
                    enemyClicked = null;
                    xEnemy = -1;
                    yEnemy = -1;
                }

                partyButtons.SetActive(true);
                EQMenu.SetActive(false);
                enemyClicked = null;
                xEnemy = -1;
                yEnemy = -1;
            }
            
            while(toDo == "Attack")
            {
                Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
                partyButtons.SetActive(false);
                EQMenu.SetActive(true);
                RaycastHit hit;
                Ray ray;
                yield return _WaitForInputClick();

                if(toDo == "Attack")
                {
                    ray = camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.gameObject.GetComponent<EnemyClass>() != null)
                        {                                   // Name,    Type,    Effect, group,cost,friendly,description,harmonic
                            MoveClass hitThem = new MoveClass("Attack", "Physical", 1.0f, false, 0, false, "Attack. Duh.", false);

                            EQMenu.SetActive(false);
                            if(person.currentlyChained == true)
                            {
                                person.chainedBefore = true;
                            }
                            yield return PartyMove(person, hit.transform.gameObject, hitThem);
                            toDo = "Done";
                        }
                    }
                }

                partyButtons.SetActive(true);
                EQMenu.SetActive(false);
            }

            while(toDo == "Guard")
            {
                partyButtons.SetActive(false);
                yield return null;
                person.currentlyGuarding = true;
                toDo = "Done";
                partyButtons.SetActive(true);
            }

            while(toDo == "Rhythm")
            {
                partyButtons.SetActive(false);
                rhythmButtons.SetActive(true);
                yield return null;

                while (toDo == "Rhythm Targeting")
                {
                    Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
                    rhythmButtons.SetActive(false);

                    if(currentMove != null)
                    {

                        if (currentMove.group == true)
                        {
                            person.currentEP -= currentMove.cost; //UPDATE EP

                            if (currentMove.friendly == true)
                            {
                                foreach (PartyMemberClass homie in party)
                                {
                                   
                                    if(homie != null && homie.currentHealth > 0) { yield return PartyMove(person, homie.gameObject, currentMove); }
                                }
                            }
                            else
                            {
                                //DID THIS SO GROUP ATTACKS DON'T CAUSE MULTIPLE CALLBACKS
                                groupMove = true;
                                if(person.currentlyChained == true)
                                {
                                    person.chainedBefore = true;
                                }
                                for(int x = 0; x < maxNumberOfRows; x++)
                                {
                                    for(int y = 0; y < maxRowSize; y++)
                                    {
                                        if (enemies[x, y] != null && enemies[x, y].currentHealth > 0) { yield return PartyMove(person, enemies[x, y].gameObject, currentMove); }
                                    }
                                }
                                groupMove = false;
                                if(lastCrit)
                                {
                                    lastCrit = false;
                                    yield return PartyMemberTurn(person, 1);
                                }
                            }
                            toDo = "Done";
                        }
                        else
                        {
                            EQMenu.SetActive(true);
                            yield return _WaitForInputClick();
                            RaycastHit hit;
                            Ray ray;

                            ray = camera.ScreenPointToRay(Input.mousePosition);
                            if (Physics.Raycast(ray, out hit))
                            {
                                if (currentMove.friendly == true)
                                {
                                    if (hit.transform.gameObject.GetComponent<PartyMemberClass>() != null)
                                    {
                                        person.currentEP -= currentMove.cost; //UPDATE EP
                                        EQMenu.SetActive(false);
                                        yield return PartyMove(person, hit.transform.gameObject, currentMove);
                                        toDo = "Done";
                                    }
                                }
                                else
                                {
                                    if (hit.transform.gameObject.GetComponent<EnemyClass>() != null)
                                    {
                                        person.currentEP -= currentMove.cost; //UPDATE EP
                                        EQMenu.SetActive(false);
                                        yield return PartyMove(person, hit.transform.gameObject, currentMove);
                                        toDo = "Done";
                                    }
                                }
                            }
                        }
                    }
                }

                rhythmButtons.SetActive(false);
                partyButtons.SetActive(true);
                currentMove = null;
            }

            while(toDo == "BREAK")
            {
                currentlyBreaking = true;
                partyButtons.SetActive(false);
                yield return null;
                Debug.Log("BREAK");
                int totalChains = 0;
                List<EnemyClass> enemiesChained = new List<EnemyClass>();
                foreach (ChainClass chainThing in chains)
                {
                    if(enemiesChained.Contains(chainThing.chainVictim.GetComponent<EnemyClass>()) == false)
                    {
                        totalChains++;
                        enemiesChained.Add(chainThing.chainVictim.GetComponent<EnemyClass>());
                    }
                    Destroy(chainThing.gameObject);
                }

                chains = new List<ChainClass>();//reset it
                recentlyChained = false;

                int totalPotential = 0;
                foreach (PartyMemberClass dude in partyMembersChained)
                {
                    int temporary;
                    
                    dude.stats.TryGetValue("Potential", out temporary);
                    //Debug.Log(temporary);
                    totalPotential += temporary;
                    dude.currentlyChained = false;
                    dude.chainedBefore = false;
                }

                partyMembersChained = new List<PartyMemberClass>();
                int resultingDamage = totalPotential * (totalChains * totalChains);
                //Debug.Log(totalPotential);
                foreach (EnemyClass badGuy in enemiesChained)
                {
                    badGuy.currentHealth -= resultingDamage;
                    Debug.Log("Chain break! " + badGuy.name + " was broken for " + resultingDamage);
                    Vector3 chainposition = badGuy.transform.position;
                    chainposition.z += 2;
                    DamagePopup.Create(chainposition, resultingDamage.ToString(), 0, 1, 1);
                }

                //partyButtons.SetActive(true);

                runItParty = false;
                runItEnemy = true;
                isItRunningEnemy = true;

                DestroyImmediate(canvas);
                canvas = Instantiate(canvasDuplicate);

                recentlyChained = false;
                chainedAgain = false;
                harmonicAmount = 1;

                foreach(PartyMemberClass dood in party)
                {
                    dood.currentlyChained = false;
                    dood.chainedBefore = false;
                }

                StopAllCoroutines();//hypothetically, this should instantly trigger the enemy turn upon a break
                yield return new WaitForSeconds(timing * 4);

                toDo = "Done";
            }

            while (toDo == "Harmonic")
            {
                yield return null;
                Debug.Log("Harmonic");
                Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);

                partyButtons.SetActive(false);
                EQMenu.SetActive(true);
                RaycastHit hit;
                Ray ray;

                yield return _WaitForInputClick();

                if (toDo == "Harmonic")
                {
                    ray = camera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        PartyMemberClass friendlyperson = hit.transform.gameObject.GetComponent<PartyMemberClass>();
                        if (friendlyperson != null && friendlyperson.currentlyChained == false)
                        {
                            Vector3 damagePosition = friendlyperson.transform.position;
                            damagePosition.z += 2;
                            damagePosition.x -= 1;
                            Vector3 affinityPosition = friendlyperson.transform.position;
                            affinityPosition.z += 2;
                            affinityPosition.x -= 1;
                            affinityPosition.y += 3;

                            EQMenu.SetActive(false);
                            Modifier buff = new Modifier();
                            buff.statName = person.harmonic.GetComponent<MoveClassWrapper>().MoveClass.type;
                            buff.amount = harmonicAmount * (int) person.harmonic.GetComponent<MoveClassWrapper>().MoveClass.effective;
                            buff.turnTime = 1;
                            harmonicAmount += 2;
                            Modifier already;
                            friendlyperson.buffDebuff.TryGetValue(buff.statName, out already);

                            if(already == null)
                            {
                                friendlyperson.buffDebuff.Add(buff.statName, buff);
                            }
                            else
                            {
                                already.amount += buff.amount;
                            }

                            DamagePopup.Create(affinityPosition, buff.statName + "\nHARMONIC", 1, 0, 1);
                            DamagePopup.Create(damagePosition, buff.amount.ToString(), 1, 0, 1);

                            yield return PartyMemberTurn(friendlyperson, 1);
                            person.currentlyChained = false;
                            toDo = "Done";
                        }
                        else
                        {
                            Debug.Log("Cannot harmonic members currently chained");
                            toDo = "Harmonic";
                        }
                    }
                }

                yield return null;
                partyButtons.SetActive(true);
                EQMenu.SetActive(false);
            }

            EQMenu.SetActive(false);
            partyButtons.SetActive(true);
        }

        DestroyImmediate(partyButtons);
        DestroyImmediate(rhythmButtons);
        foreach (ChainClass chainThing in chains)
        {
            Destroy(chainThing.gameObject);
        }
        chains = new List<ChainClass>();

        Debug.Log(person.name + "'s turn just ended");

        recentlyChained = false;
        person.currentlyChained = false;
        person.chainedBefore = false;
        chainedAgain = false;
        harmonicAmount = 1;
    }

    public void changeToDo(string thing)
    {
        Debug.Log(thing);
        toDo = thing;
    }

    public void UpdateMove(MoveClass move, PartyMemberClass doingIt)
    {
        Debug.Log(move.moveName);
        if (move.cost <= doingIt.currentEP)
        {
            currentMove = move;
        }
        else
        {
            DamagePopup.Create(new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - 10), "NO EP", 1, 0, 1);
            toDo = null;
        }
    }

    IEnumerator _WaitForInputClick()
    {
        //yield return null; //need this to not duplicate click choices
        bool temp = Input.GetMouseButtonDown(0);
        while(temp == false)
        {
            yield return null;
            temp = Input.GetMouseButtonDown(0);
        }
    }

    IEnumerator _WaitForInputClickLift()
    {
        //yield return null; //need this to not duplicate click choices
        bool temp = Input.GetMouseButtonUp(0);
        while (temp == false)
        {
            yield return null;
            temp = Input.GetMouseButtonUp(0);
        }
    }

    IEnumerator PartyMove(PartyMemberClass doerP, GameObject victimP, MoveClass moveP)
    {
        Vector3 damagePosition = victimP.transform.position;
        damagePosition.z += 2;
        damagePosition.x -= 1;
        Vector3 affinityPosition = victimP.transform.position;
        affinityPosition.z += 2;
        affinityPosition.x -= 1;
        affinityPosition.y += 3;
        Vector3 doerDamagePosition = doerP.transform.position;
        doerDamagePosition.z += 2;
        doerDamagePosition.x -= 1;
        Vector3 doerAffinityPosition = doerP.transform.position;
        doerAffinityPosition.z += 2;
        doerAffinityPosition.x -= 1;
        doerAffinityPosition.y += 3;

        switch (moveP.type)
        {
            //ALL FORMS OF ATTACK. ONCE NEW TYPES ARE ADDED, THEY GO HERE.
            case "Physical":
            case "Drum":
            case "Bass":
            case "Guitar":
            case "Piano":
                //Determine who we're attacking
                //Determine attack value
                KnownInfoDataType keepIt = KnownInfo.getFromJSON(victimP.GetComponent<EnemyClass>().enemyName);
                int d20 = Random.Range(1, 21);
                double percent = d20 * 0.02;
                int statInQuestion;
                string affinityInQuestion;
                int agility;
                victimP.gameObject.GetComponent<EnemyClass>().affinities.TryGetValue(moveP.type, out affinityInQuestion);
                victimP.gameObject.GetComponent<EnemyClass>().stats.TryGetValue("Agility", out agility);

                if(doerP.currentlyChained == true && moveP.group == false)
                {
                    doerP.chainedBefore = true;
                }

                //Do they dodge?
                int d100 = Random.Range(1, 101);
                Modifier buffsAgility;
                victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.TryGetValue("Agility", out buffsAgility);
                if (buffsAgility != null && buffsAgility.turnTime != 0)
                {
                    agility += buffsAgility.amount;
                    buffsAgility.turnTime--;
                }
                else
                {
                    victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.Remove("Agility");
                }
                if (d100 <= agility)
                {
                    //DODGE STUFF HERE
                    Debug.Log("Dodge!");
                    DamagePopup.Create(affinityPosition, "Dodge!", 1, 0.92f, 0.016f);
                    yield return new WaitForSeconds(timing);
                    break;
                }

                if (moveP.type == "Physical")
                {
                    victimP.gameObject.GetComponent<EnemyClass>().stats.TryGetValue("Physical Defence", out statInQuestion);
                    Modifier buffs;
                    victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.TryGetValue("Physical Defence", out buffs);
                    if (buffs != null && buffs.turnTime != 0)
                    {
                        statInQuestion += buffs.amount;
                        buffs.turnTime--;
                    }
                    else
                    {
                        victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.Remove("Physical Defence");
                    }
                }
                else
                {
                    victimP.gameObject.GetComponent<EnemyClass>().stats.TryGetValue("Rhythm Defence", out statInQuestion);
                    Modifier buffs;
                    victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.TryGetValue("Rhythm Defence", out buffs);
                    if (buffs != null && buffs.turnTime != 0)
                    {
                        statInQuestion += buffs.amount;
                        buffs.turnTime--;
                    }
                    else
                    {
                        victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.Remove("Rhythm Defence");
                    }
                }

                for (int x = 0; x < keepIt.affinities.Length; x++)
                {
                    if (keepIt.affinities[x].affName == moveP.GetMoveType())
                    {
                        keepIt.affinities[x].affValue = affinityInQuestion;
                        break;
                    }
                }
                KnownInfo.writeToJSON(keepIt);


                int crit = 1;
                if (d20 >= 19 || affinityInQuestion == "Weak")
                {
                    if(doerP.currentlyChained == true)
                    {
                        if(moveP.group == false)
                        {
                            chainedAgain = true;
                        }
                    }
                    DamagePopup.Create(affinityPosition, "CRITICAL", 0, 1, 1);
                    yield return new WaitForSeconds(timing);
                    crit = 2;
                }

                //If not, calculate damage
                double damagePercent = (crit * percent) + 0.6;
                int potentialDamage;
                if (moveP.type == "Physical")
                {
                    doerP.stats.TryGetValue("Physical", out potentialDamage);
                    Modifier buffsPhysical;
                    doerP.buffDebuff.TryGetValue("Physical", out buffsPhysical);
                    if (buffsPhysical != null && buffsPhysical.turnTime != 0)
                    {
                        potentialDamage += buffsPhysical.amount;
                        buffsPhysical.turnTime--;
                    }
                    else
                    {
                        victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.Remove("Physical");
                    }
                }
                else
                {
                    doerP.stats.TryGetValue("Rhythm", out potentialDamage);
                    Modifier buffsRhythm;
                    doerP.buffDebuff.TryGetValue("Rhythm", out buffsRhythm);
                    if (buffsRhythm != null && buffsRhythm.turnTime != 0)
                    {
                        potentialDamage += buffsRhythm.amount;
                        buffsRhythm.turnTime--;
                    }
                    else
                    {
                        victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.Remove("Rhythm");
                    }
                }
                double damageDealt = damagePercent * potentialDamage * moveP.effective;

                int d20def = Random.Range(1, 21);
                double percentDefended = ((d20def * 0.01) + 0.8) * statInQuestion / 100;

                double damageNotRounded = damageDealt - (damageDealt * percentDefended);
                
                //Debug.Log(potentialDamage);
                if (affinityInQuestion == "Strong")
                {
                    damageNotRounded = damageNotRounded * 0.5;
                    DamagePopup.Create(affinityPosition, "RESIST", 1, 1, 1);
                    yield return new WaitForSeconds(timing);
                }
                int damage = (int)System.Math.Round(damageNotRounded);
                affinityPosition.y += 3;

                //Impact numbers
                if (affinityInQuestion == "Absorb")
                {
                    victimP.gameObject.GetComponent<EnemyClass>().currentHealth += damage;
                    int maxHealthParty;
                    doerP.gameObject.GetComponent<PartyMemberClass>().stats.TryGetValue("HP", out maxHealthParty);
                    if (victimP.gameObject.GetComponent<EnemyClass>().currentHealth > maxHealthParty)
                    {
                        victimP.gameObject.GetComponent<EnemyClass>().currentHealth = maxHealthParty;
                    }
                    DamagePopup.Create(damagePosition, damage.ToString(), 0, 1, 0);
                    yield return new WaitForSeconds(timing);
                }

            /*
             *
             * DO WORK HERE
             * 
             */

                else if (affinityInQuestion == "Reflect")
                {
                    int doerDefence;
                    if(moveP.type == "Physical")
                    {
                        doerP.stats.TryGetValue("Physical Defence", out doerDefence);
                    }
                    else
                    {
                        doerP.stats.TryGetValue("Rhythm Defence", out doerDefence);
                    }
                    doerP.currentHealth -= (damage * (1 - (doerDefence / 100)));
                }
                else
                {
                    victimP.gameObject.GetComponent<EnemyClass>().currentHealth -= damage;
                    DamagePopup.Create(damagePosition, damage.ToString(), 1, 0, 0);

                }

                //DISPLAY IT
                //yield return new WaitForSeconds(1.0f);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + victimP.gameObject.GetComponent<EnemyClass>().name + " for " + damage);

                if (crit > 1 && victimP.GetComponent<EnemyClass>().currentHealth > 0 && affinityInQuestion != "Reflect" && affinityInQuestion != "Absorb")
                {
                    partyMembersChained.Add(doerP);
                    GameObject temporaryThing = new GameObject();
                    temporaryThing.AddComponent<ChainClass>();
                    ChainClass surprise = temporaryThing.GetComponent<ChainClass>();
                    surprise.chainHolder = doerP;
                    surprise.chainVictim = victimP.GetComponent<EnemyClass>();
                    surprise.Initialize();

                    bool isItThere = false;
                    for (int x = 0; x < maxRowSize; x++)
                    {
                        if (victimP.GetComponent<EnemyClass>() == front[x].GetComponentInChildren<EnemyClass>())
                        {
                            isItThere = true;
                        }
                    }

                    //ADD CHAIN TO FRONT ROW ONLY (later change based on weapon)
                    if (doerP.currentlyChained == false && chains.Contains(surprise) == false && isItThere && victimP.GetComponent<EnemyClass>().currentHealth > 0)
                    {
                        chains.Add(surprise);
                        Debug.Log("CHAINED " + surprise.chainHolder.name + " " + surprise.chainVictim.name);
                        doerP.currentlyChained = true;
                    }
                    else
                    {
                        Destroy(surprise.gameObject);
                    }
                }

                //IF CHAINED, GO BACK TO TOP OF LOOP THING
                if (doerP.currentlyChained == true)
                {
                    if(moveP.group == true)
                    {
                        lastCrit = true;
                    }
                    else
                    {
                        if (doerP.chainedBefore == false)
                        {
                            yield return PartyMemberTurn(doerP, 1);
                        }
                    }
                }

                //retain learned information
                //yield return new WaitForSeconds(timing);

                break;
            //ALL BUFFS GO HERE.
            case "Strength Buff":
            case "Rhythm Buff":
            case "Physical Defence Buff":
            case "Rhythm Defence Buff":
            case "Agility Buff":
            case "Potential Buff":
                Regex regexBuff = new Regex("(\\s+( Buff)\\s*)$");
                string moveStringBuff = regexBuff.Replace(moveP.type, "");

                Modifier buff = new Modifier();
                buff.statName = moveStringBuff;
                buff.amount = (int)Mathf.Round(moveP.effective);
                buff.turnTime = 3;

                //ADD THE BUFF
                Modifier thingBuff;
                PartyMemberClass buddy = victimP.gameObject.GetComponent<PartyMemberClass>();
                buddy.buffDebuff.TryGetValue(buff.statName, out thingBuff);
                if (thingBuff == null)
                {
                    buddy.buffDebuff.Add(buff.statName, buff);
                }
                else //there's already a buff or debuff in that category
                {
                    if (thingBuff.amount < 0)
                    {
                        thingBuff.amount = buff.amount;
                        thingBuff.turnTime = 3;
                    }
                    else if (thingBuff.amount == buff.amount)
                    {
                        thingBuff.turnTime += buff.turnTime;
                    }
                    else if (thingBuff.amount > 0)
                    {
                        thingBuff.amount += buff.amount;
                        thingBuff.turnTime = 3;
                    }
                    else
                    {
                        Debug.Log("shouldn't break anything, but the buff isn't working");
                    }
                }

                //DISPLAY IT
                DamagePopup.Create(affinityPosition, thingBuff + "\nBUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, buff.amount.ToString(), 1, 0.92f, 0.016f);

                yield return new WaitForSeconds(timing * 4);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + buddy.name + " for " + buff.amount);

                break;
            //ALL DEBUFFS GO HERE.
            case "Strength Debuff":
            case "Rhythm Debuff":
            case "Physical Defence Debuff":
            case "Rhythm Defence Debuff":
            case "Agility Debuff":
            case "Potential Debuff":
                Regex regexDebuff = new Regex("(\\s+( Debuff)\\s*)$");
                string moveStringDebuff = regexDebuff.Replace(moveP.type, "");

                Modifier debuff = new Modifier();
                debuff.statName = moveStringDebuff;
                debuff.amount = (int)Mathf.Round(moveP.effective);
                debuff.turnTime = 3;

                EnemyClass notBuddy = victimP.gameObject.GetComponent<EnemyClass>();

                //ADD THE DEBUFF
                Modifier thingDebuff;
                notBuddy.buffDebuff.TryGetValue(debuff.statName, out thingDebuff);
                if (thingDebuff == null)
                {
                    notBuddy.buffDebuff.Add(debuff.statName, debuff);
                }
                else //there's already a buff or debuff in that category
                {
                    if (thingDebuff.amount > 0)
                    {
                        thingDebuff.amount = debuff.amount;
                        thingDebuff.turnTime = 3;
                    }
                    else if (thingDebuff.amount == debuff.amount)
                    {
                        thingDebuff.turnTime -= debuff.turnTime;
                    }
                    else if (thingDebuff.amount < 0)
                    {
                        thingDebuff.amount -= debuff.amount;
                        thingDebuff.turnTime = 3;
                    }
                    else
                    {
                        Debug.Log("shouldn't break anything, but the debuff isn't working");
                    }
                }


                //DISPLAY IT
                yield return new WaitForSeconds(timing * 4);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + notBuddy.name + " for " + debuff.amount);
                DamagePopup.Create(affinityPosition, thingDebuff + "\nDEBUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, debuff.amount.ToString(), 1, 0.92f, 0.016f);

                break;
            //HEALIES FOR YOUR FEELIES
            case "Heal":

                int maxHealth;
                victimP.gameObject.GetComponent<PartyMemberClass>().stats.TryGetValue("HP", out maxHealth);
                float healies = moveP.effective * maxHealth;
                int heals = (int)Mathf.Round(healies);
                victimP.gameObject.GetComponent<PartyMemberClass>().currentHealth += heals;
                if (victimP.gameObject.GetComponent<PartyMemberClass>().currentHealth > maxHealth) { victimP.gameObject.GetComponent<PartyMemberClass>().currentHealth = maxHealth; }

                //DISPLAY IT
                yield return new WaitForSeconds(timing * 4);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + victimP.gameObject.GetComponent<PartyMemberClass>().name + " for " + heals);
                DamagePopup.Create(damagePosition, heals.ToString(), 0, 1, 0);

                break;
            default:
                throw new System.Exception("Fell into default in playermove");
        }
    }

    //Things to do on enemy turn
    IEnumerator EnemyTurn(EnemyClass enemy, int row, int col)
    {
        switch(row)
        {
            case 0: //front row
                if(enemy.frontMoves.Length > 0 && enemy.currentHealth > 0)
                {
                    int whichFront = Random.Range(0, enemy.frontMoves.Length);
                    GameObject doItFront = enemy.frontMoves[whichFront];
                    MoveClassWrapper theThingFrontBasis = doItFront.GetComponent<MoveClassWrapper>();
                    MoveClass theThingFront = theThingFrontBasis.MoveClass;
                    yield return EnemyMove(enemy, theThingFront);
                }
                break;
            case 1: //mid row
                if(enemy.midMoves.Length > 0 && enemy.currentHealth > 0)
                {
                    int whichMid = Random.Range(0, enemy.midMoves.Length);
                    GameObject doItMid = enemy.midMoves[whichMid];
                    MoveClassWrapper theThingMidBasis = doItMid.GetComponent<MoveClassWrapper>();
                    MoveClass theThingMid = theThingMidBasis.MoveClass;
                    yield return EnemyMove(enemy, theThingMid);
                }
                break;
            case 2: //back row
                if(enemy.backMoves.Length > 0 && enemy.currentHealth > 0)
                {
                    int whichBack = Random.Range(0, enemy.backMoves.Length);
                    GameObject doItBack = enemy.backMoves[whichBack];
                    MoveClassWrapper theThingBackBasis = doItBack.GetComponent<MoveClassWrapper>();
                    MoveClass theThingBack = theThingBackBasis.MoveClass;
                    yield return EnemyMove(enemy, theThingBack);
                }
                break;
            default:
                throw new System.Exception("You fell into default in the enemy turn function of battle class");
        }

    }


    //actual enemy attack
    IEnumerator EnemyMove(EnemyClass doer, MoveClass move)
    {
        Vector3 damagePosition;
        Vector3 affinityPosition;
        Vector3 doerDamagePosition = doer.transform.position;
        doerDamagePosition.z += 2;
        doerDamagePosition.x -= 1;
        Vector3 doerAffinityPosition = doer.transform.position;
        doerAffinityPosition.z += 2;
        doerAffinityPosition.x -= 1;
        doerAffinityPosition.y += 3;

        doer.gameObject.GetComponent<Animator>().SetTrigger("Move");
        yield return new WaitForSeconds(timing);
        switch (move.type)
        {
            //ALL FORMS OF ATTACK. ONCE NEW TYPES ARE ADDED, THEY GO HERE.
            case "Physical":
            case "Drum":
            case "Bass":
            case "Guitar":
            case "Piano":
                //Determine who we're attacking
                PartyMemberClass[] vics = new PartyMemberClass[party.Length];
                if (move.group == true)
                {
                    for (int x = 0; x < party.Length; x++)
                    {
                        if(party[x] != null)
                        {
                            vics[x] = party[x];
                        }
                        else
                        {
                            vics[x] = null;
                        }
                    }
                }
                else
                {
                    if (difficulty == "Easy")
                    {
                        //Pick the worst enemy to attack
                        vics[0] = PickWorstPartyToHit(move.type);
                        if (vics[0] == null)
                        {
                            Debug.Log("No victim");
                            break;
                        }
                    }
                    else if (difficulty == "Medium")
                    {
                        //Randomly pick an enemy
                        PartyMemberClass vicA = PickWorstPartyToHit(move.type);
                        PartyMemberClass vicB = PickBestPartyToHit(move.type);
                        float coin = Random.value;
                        if (coin >= 0.5) { vics[0] = vicA; }
                        else { vics[0] = vicB; }
                    }
                    else if (difficulty == "Hard")
                    {
                        //Pick the best enemy to attack
                        vics[0] = PickBestPartyToHit(move.type);
                        if (vics[0] == null)
                        {
                            Debug.Log("No victim");
                            break;
                        }
                    }
                    else
                    {
                        throw new System.Exception("No difficulty selected in battle class");
                    }
                }

                foreach(PartyMemberClass victim in vics)
                {
                    if(victim != null && victim.currentHealth > 0)
                    {
                        damagePosition = victim.transform.position;
                        damagePosition.z += 2;
                        damagePosition.x -= 1;
                        affinityPosition = victim.transform.position;
                        affinityPosition.z += 2;
                        affinityPosition.x -= 1;
                        affinityPosition.y += 3;
                        //Do they dodge?
                        int agility;
                        victim.stats.TryGetValue("Agility", out agility);
                        int d100 = Random.Range(1, 101);
                        Modifier buffsAgility;
                        victim.buffDebuff.TryGetValue("Agility", out buffsAgility);
                        if (buffsAgility != null && buffsAgility.turnTime != 0)
                        {
                            agility += buffsAgility.amount;
                            buffsAgility.turnTime--;
                        }
                        else
                        {
                            victim.buffDebuff.Remove("Agility");
                        }

                        if (d100 <= agility)
                        {
                            //DODGE STUFF HERE
                            Debug.Log("Dodge!");
                            DamagePopup.Create(affinityPosition, "Dodge!", 1, 0.92f, 0.016f);
                            continue;
                        }

                        //Determine attack value
                        int d20 = Random.Range(1, 21);
                        double percent = d20 * 0.02;
                        int statInQuestion;
                        string affinityInQuestion;
                        victim.affinities.TryGetValue(move.type, out affinityInQuestion);
                        if (move.type == "Physical")
                        {
                            victim.stats.TryGetValue("Physical Defence", out statInQuestion);
                            Modifier buffs;
                            victim.buffDebuff.TryGetValue("Physical Defence", out buffs);
                            if (buffs != null && buffs.turnTime != 0)
                            {
                                statInQuestion += buffs.amount;
                                buffs.turnTime--;
                            }
                            else
                            {
                                victim.buffDebuff.Remove("Physical Defence");
                            }
                        }
                        else
                        {
                            victim.stats.TryGetValue("Rhythm Defence", out statInQuestion);
                            Modifier buffs;
                            victim.buffDebuff.TryGetValue("Rhythm Defence", out buffs);
                            if (buffs != null && buffs.turnTime != 0)
                            {
                                statInQuestion += buffs.amount;
                                buffs.turnTime--;
                            }
                            else
                            {
                                victim.buffDebuff.Remove("Rhythm Defence");
                            }
                        }


                        int crit = 1;
                        if (d20 >= 19 || affinityInQuestion == "Weak")
                        {
                            crit = 2;
                            DamagePopup.Create(affinityPosition, "CRITICAL", 0, 1, 1);
                            yield return new WaitForSeconds(timing * 4);
                        }

                        //If not, calculate damage
                        double damagePercent = (crit * percent) + 0.6;
                        int potentialDamage;
                        if (move.type == "Physical")
                        {
                            doer.stats.TryGetValue("Physical", out potentialDamage);
                            Modifier buffsPhysical;
                            doer.buffDebuff.TryGetValue("Physical", out buffsPhysical);
                            if (buffsPhysical != null && buffsPhysical.turnTime != 0)
                            {
                                potentialDamage += buffsPhysical.amount;
                                buffsPhysical.turnTime--;
                            }
                            else
                            {
                                victim.buffDebuff.Remove("Physical");
                            }
                        }
                        else
                        {
                            doer.stats.TryGetValue("Rhythm", out potentialDamage);
                            Modifier buffsRhythm;
                            doer.buffDebuff.TryGetValue("Rhythm", out buffsRhythm);
                            if (buffsRhythm != null && buffsRhythm.turnTime != 0)
                            {
                                potentialDamage += buffsRhythm.amount;
                                buffsRhythm.turnTime--;
                            }
                            else
                            {
                                victim.buffDebuff.Remove("Rhythm");
                            }
                        }
                        double damageDealt = damagePercent * potentialDamage * move.effective;

                        int d20def = Random.Range(1, 21);
                        double percentDefended = ((d20def * 0.01) + 0.8) * statInQuestion / 100;

                        double damageNotRounded = damageDealt - (damageDealt * percentDefended);
                        if (victim.currentlyGuarding == true) { damageNotRounded = damageNotRounded * 0.5; }
                        if (affinityInQuestion == "Strong") { damageNotRounded = damageNotRounded * 0.5; DamagePopup.Create(affinityPosition, "RESIST", 1, 1, 1); yield return new WaitForSeconds(timing * 4); }
                        int damage = (int)System.Math.Round(damageNotRounded);

                        //Impact numbers
                        if (affinityInQuestion == "Absorb")
                        {
                            victim.currentHealth += damage;
                            int maxHealthParty;
                            victim.stats.TryGetValue("HP", out maxHealthParty);
                            if (victim.currentHealth > maxHealthParty)
                            {
                                victim.currentHealth = maxHealthParty;
                            }
                            DamagePopup.Create(damagePosition, damage.ToString(), 0, 1, 0);
                        }

                        else if (affinityInQuestion == "Reflect")
                        {
                            int doerDefence;
                            if (move.type == "Physical")
                            {
                                doer.stats.TryGetValue("Physical Defence", out doerDefence);
                            }
                            else
                            {
                                doer.stats.TryGetValue("Rhythm Defence", out doerDefence);
                            }
                            doer.currentHealth -= (damage * (1 - (doerDefence / 100)));
                        }
                        else
                        {
                            victim.currentHealth -= damage;
                            DamagePopup.Create(damagePosition, damage.ToString(), 1, 0, 0);
                        }

                        //DISPLAY IT
                        //yield return new WaitForSeconds(1.0f);
                        Debug.Log(doer.name + " did a " + move.moveName + " on " + victim.name + " for " + damage);
                    }
                }


                break;
            //ALL BUFFS GO HERE.
            case "Strength Buff":
            case "Rhythm Buff":
            case "Physical Defence Buff":
            case "Rhythm Defence Buff":
            case "Agility Buff":
            case "Potential Buff":
                string moveStringBuff = move.type.Replace(" Buff", "");
                EnemyClass buddy;
                if (difficulty == "Easy")
                {

                    buddy = PickWorstEnemyToBuff(moveStringBuff);
                }
                else if (difficulty == "Medium")
                {

                    //Randomly pick an enemy
                    EnemyClass vicA = PickWorstEnemyToBuff(moveStringBuff);
                    EnemyClass vicB = PickBestEnemyToBuff(moveStringBuff);
                    float coin = Random.value;
                    if (coin >= 0.5) { buddy = vicA; }
                    else { buddy = vicB; }
                }
                else if (difficulty == "Hard")
                {

                    buddy = PickBestEnemyToBuff(moveStringBuff);
                }
                else
                {
                    throw new System.Exception("No difficulty selected in battle class");
                }

                //Debug.Log(doer + " " + buddy);
                if (buddy == null) { break; };
                damagePosition = buddy.transform.position;
                damagePosition.z += 2;
                damagePosition.x -= 1;
                affinityPosition = buddy.transform.position;
                affinityPosition.z += 2;
                affinityPosition.x -= 1;
                affinityPosition.y += 3;

                Modifier buff = new Modifier();
                buff.statName = moveStringBuff;
                buff.amount = (int)Mathf.Round(move.effective);
                buff.turnTime = 3;

                //ADD THE BUFF
                Modifier thingBuff;
                buddy.buffDebuff.TryGetValue(buff.statName, out thingBuff);
                if(thingBuff == null)
                {
                    buddy.buffDebuff.Add(buff.statName, buff);
                }
                else //there's already a buff or debuff in that category
                {
                    if(thingBuff.amount < 0)
                    {
                        thingBuff.amount = buff.amount;
                        thingBuff.turnTime = 3;
                    }
                    else if(thingBuff.amount == buff.amount)
                    {
                        thingBuff.turnTime += buff.turnTime;
                    }
                    else if(thingBuff.amount > 0)
                    {
                        thingBuff.amount += buff.amount;
                        thingBuff.turnTime = 3;
                    }
                    else
                    {
                        Debug.Log("shouldn't break anything, but the buff isn't working");
                    }
                }

                //DISPLAY IT
                yield return new WaitForSeconds(timing);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + buddy.name + " for " + buff.amount);
                DamagePopup.Create(affinityPosition, buff.statName + "\nBUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, buff.amount.ToString(), 1, 0.92f, 0.016f);

                break;
            //ALL DEBUFFS GO HERE.
            case "Strength Debuff":
            case "Rhythm Debuff":
            case "Physical Defence Debuff":
            case "Rhythm Defence Debuff":
            case "Agility Debuff":
            case "Potential Debuff":
                string moveStringDebuff = move.type.Replace(" Debuff", "");
                PartyMemberClass notBuddy;
                if (difficulty == "Easy")
                {
                    notBuddy = PickWorstPartyToDebuff(moveStringDebuff);
                }
                else if (difficulty == "Medium")
                {

                    //Randomly pick an enemy
                    PartyMemberClass vicA = PickWorstPartyToDebuff(moveStringDebuff);
                    PartyMemberClass vicB = PickBestPartyToDebuff(moveStringDebuff);
                    float coin = Random.value;
                    if (coin >= 0.5) { notBuddy = vicA; }
                    else { notBuddy = vicB; }
                }
                else if (difficulty == "Hard")
                {

                    notBuddy = PickBestPartyToDebuff(moveStringDebuff);
                }
                else
                {
                    throw new System.Exception("No difficulty selected in battle class");
                }

                damagePosition = notBuddy.transform.position;
                damagePosition.z += 2;
                damagePosition.x -= 1;
                affinityPosition = notBuddy.transform.position;
                affinityPosition.z += 2;
                affinityPosition.x -= 1;
                affinityPosition.y += 3;

                Modifier debuff = new Modifier();
                debuff.statName = moveStringDebuff;
                debuff.amount = (int)Mathf.Round(move.effective);
                debuff.turnTime = 3;

                //ADD THE DEBUFF
                Modifier thingDebuff;
                notBuddy.buffDebuff.TryGetValue(debuff.statName, out thingDebuff);
                if (thingDebuff == null)
                {
                    notBuddy.buffDebuff.Add(debuff.statName, debuff);
                }
                else //there's already a buff or debuff in that category
                {
                    if (thingDebuff.amount > 0)
                    {
                        thingDebuff.amount = debuff.amount;
                        thingDebuff.turnTime = 3;
                    }
                    else if (thingDebuff.amount == debuff.amount)
                    {
                        thingDebuff.turnTime -= debuff.turnTime;
                    }
                    else if (thingDebuff.amount < 0)
                    {
                        thingDebuff.amount -= debuff.amount;
                        thingDebuff.turnTime = 3;
                    }
                    else
                    {
                        Debug.Log("shouldn't break anything, but the debuff isn't working");
                    }
                }


                //DISPLAY IT
                yield return new WaitForSeconds(timing);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + notBuddy.name + " for " + debuff.amount);
                DamagePopup.Create(affinityPosition, debuff.statName + "\nDEBUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, debuff.amount.ToString(), 1, 0.92f, 0.016f);

                break;
            //HEALIES FOR YOUR FEELIES
            case "Heal":
                //Find the member of the enemies with the lowest health and HEAL THAT FOOL
                int lowestHealth = 500000;
                int xPos = -1;
                int yPos = -1;
                for (int x = 0; x < maxNumberOfRows; x++)
                {
                    for (int y = 0; y < maxRowSize; y++)
                    {
                        if(enemies[x, y] != null)
                        {
                            if (lowestHealth > enemies[x, y].currentHealth)
                            {
                                lowestHealth = enemies[x, y].currentHealth;
                                xPos = x;
                                yPos = y;
                            }
                        }
                    }
                }

                if (lowestHealth == 500000) { Debug.Log("couldn't heal"); break; }

                damagePosition = enemies[xPos, yPos].transform.position;
                damagePosition.z += 2;
                damagePosition.x -= 1;
                affinityPosition = enemies[xPos, yPos].transform.position;
                affinityPosition.z += 2;
                affinityPosition.x -= 1;
                affinityPosition.y += 3;

                int maxHealth;
                enemies[xPos, yPos].stats.TryGetValue("HP", out maxHealth);
                float healies = move.effective * maxHealth;
                int heals = (int)Mathf.Round(healies);
                enemies[xPos, yPos].currentHealth += heals;
                if(enemies[xPos, yPos].currentHealth > maxHealth) { enemies[xPos, yPos].currentHealth = maxHealth;  }

                //DISPLAY IT
                yield return new WaitForSeconds(timing);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + enemies[xPos, yPos].name + " for " + heals);
                DamagePopup.Create(damagePosition, heals.ToString(), 0, 1, 0);

                break;
            default:
                //yield return new WaitForSeconds(timing);
                Debug.Log("The enemy did nothing.");
                break;
        }

        yield return new WaitForSeconds(timing * 3);
    }

    public struct data
    {
        public string affinity { get; set; }
        public int stat { get; set; }
        public float healthPercentage { get; set; }
        public bool currentlyGuarding { get; set; }
        public int points { get; set; }
    }

    //AI STUFF
    private PartyMemberClass PickWorstPartyToDebuff(string type)
    {
        data[] information = new data[party.Length];
        for (int x = 0; x < party.Length; x++)
        {
            if (party[x] != null)
            {
                int temp;
                party[x].stats.TryGetValue(type, out temp);
                information[x].stat = temp;
                int maxHealth;
                party[x].stats.TryGetValue("HP", out maxHealth);
                information[x].healthPercentage = party[x].currentHealth / maxHealth;
                //Debug.Log(party[x].currentHealth);
            }
        }

        int lowest = 100;
        int pos = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (lowest > information[x].stat && information[x].healthPercentage != 0)//default is 0, this may become a problem later
            {
                lowest = information[x].stat;
                pos = x;
            }
        }

        if (pos == -1)
        {
            Debug.Log("couldn't find a party to debuff");
            return null;
        }
        else
        {
            return party[pos];
        }
    }

    private PartyMemberClass PickBestPartyToDebuff(string type)
    {
        data[] information = new data[party.Length];
        for (int x = 0; x < party.Length; x++)
        {
            if(party[x] != null)
            {
                int temp;
                party[x].stats.TryGetValue(type, out temp);
                information[x].stat = temp;
                int maxHealth;
                party[x].stats.TryGetValue("HP", out maxHealth);
                information[x].healthPercentage = party[x].currentHealth / maxHealth;
                //Debug.Log(party[x].currentHealth);
            }
        }

        int highest = -100;
        int pos = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (highest < information[x].stat && information[x].healthPercentage != 0)
            {
                highest = information[x].stat;
                pos = x;
            }
        }

        if (pos == -1)
        {
            Debug.Log("couldn't find a party to debuff");
            return null;
        }
        else
        {
            return party[pos];
        }
    }
    
    private EnemyClass PickWorstEnemyToBuff(string type)
    {
        data[,] information = new data[maxNumberOfRows, maxRowSize];
        for(int x = 0; x < maxNumberOfRows; x++)
        {
            for(int y = 0; y < maxRowSize; y++)
            {
                if(enemies[x, y] != null)
                {
                    int result;
                    //Debug.Log(type);
                    enemies[x, y].stats.TryGetValue(type, out result);
                    information[x, y].stat = result;
                    int maxHealth;
                    enemies[x, y].stats.TryGetValue("HP", out maxHealth);
                    information[x, y].healthPercentage = enemies[x, y].currentHealth / maxHealth;
                    //Debug.Log(x + " " + y + " " + information[x, y].stat);
                }
            }
        }

        int highest = -100;
        int xPos = -1;
        int yPos = -1;
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                if(highest < information[x, y].stat && information[x, y].stat != default)
                {
                    highest = information[x, y].stat;
                    xPos = x;
                    yPos = y;
                }
            }
        }

        if (xPos == -1)
        {
            Debug.Log("couldn't find an enemy to buff");
            return null;
        }
        else
        {
            return enemies[xPos, yPos];
        }
    }

    private EnemyClass PickBestEnemyToBuff(string type)
    {
        data[,] information = new data[maxNumberOfRows, maxRowSize];
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                if (enemies[x, y] != null)
                {
                    int result;
                    enemies[x, y].stats.TryGetValue(type, out result);
                    information[x, y].stat = result;
                    int maxHealth;
                    enemies[x, y].stats.TryGetValue("HP", out maxHealth);
                    information[x, y].healthPercentage = enemies[x, y].currentHealth / maxHealth;
                }
            }
        }

        int lowest = 100;
        int xPos = -1;
        int yPos = -1;
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                if (lowest >= information[x, y].stat && information[x, y].stat != default)
                {
                    lowest = information[x, y].stat;
                    xPos = x;
                    yPos = y;
                }
            }
        }

        if (xPos == -1)
        {
            Debug.Log("couldn't find an enemy to buff");
            return null;
        }
        else
        {
            return enemies[xPos, yPos];
        }
    }
    private PartyMemberClass PickWorstPartyToHit(string type)
    {
        data[] information = new data[party.Length];
        PartyMemberClass victim;

        //Establish AI data
        for(int x = 0; x < party.Length; x++)
        {
            if(party[x].gameObject.activeSelf != false)
            {
                string temp;
                int maxHealth;
                party[x].affinities.TryGetValue(type, out temp);
                party[x].stats.TryGetValue("HP", out maxHealth);
                information[x].affinity = temp;
                information[x].healthPercentage = party[x].currentHealth / maxHealth;
                information[x].currentlyGuarding = party[x].currentlyGuarding;
            }
        }

        //Impact AI point values 
        for(int x = 0; x < information.Length; x++)
        {
            //affecting points to pick the worst one
            if (information[x].affinity == "Weak") { information[x].points += 3; }
            else if (information[x].affinity == "Strong") { information[x].points -= 1; }
            else if (information[x].affinity == "Absorb" || information[x].affinity == "Reflect") { information[x].points -= 2; }

            if (information[x].healthPercentage < 0.5) { information[x].points += 2; }

            if (information[x].currentlyGuarding == true) { information[x].points -= 2; }
            else { information[x].points += 3; }

            if (information[x].healthPercentage == 0) { information[x].points = 100; }
        }

        //pick WORST one
        int lowestPoint = 100;
        int lowestPerson = -1;
        for(int x = 0; x < party.Length; x++)
        {
            if(information[x].points <= lowestPoint && information[x].affinity != default)
            {
                lowestPoint = information[x].points;
                lowestPerson = x;
            }
        }

        if(lowestPerson == -1)
        {
            Debug.Log("AI returned null");
            return null;
        }
        else
        {
            victim = party[lowestPerson];
            return victim;
        }
    }

    private PartyMemberClass PickBestPartyToHit(string type)
    {
        data[] information = new data[party.Length];
        PartyMemberClass victim;

        //Establish AI data
        for (int x = 0; x < party.Length; x++)
        {
            if (party[x].gameObject.activeSelf != false)
            {
                string temp;
                int maxHealth;
                party[x].affinities.TryGetValue(type, out temp);
                party[x].stats.TryGetValue("HP", out maxHealth);
                information[x].affinity = temp;
                information[x].healthPercentage = party[x].currentHealth / maxHealth;
                information[x].currentlyGuarding = party[x].currentlyGuarding;
            }
        }

        //Impact AI point values 
        for (int x = 0; x < information.Length; x++)
        {
            //affecting points to pick the worst one
            if (information[x].affinity == "Weak") { information[x].points += 3; }
            else if (information[x].affinity == "Strong") { information[x].points -= 1; }
            else if (information[x].affinity == "Absorb" || information[x].affinity == "Reflect") { information[x].points -= 2; }

            if (information[x].healthPercentage < 0.5) { information[x].points += 2; }

            if (information[x].currentlyGuarding == true) { information[x].points -= 2; }
            else { information[x].points += 3; }

            if (information[x].healthPercentage == 0) { information[x].points = 100; }
        }

        //pick BEST one
        int highestPoint = -100;
        int highestPerson = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (information[x].points > highestPoint && information[x].affinity != default)
            {
                highestPoint = information[x].points;
                highestPerson = x;
            }
        }

        if (highestPerson == -1)
        {
            return null;
        }
        else
        {
            victim = party[highestPerson];
            return victim;
        }
    }

    //Party Phase of battle overview and turn movement
    IEnumerator PartyPhase()
    {
        isItRunningParty = false;
        Debug.Log("PARTY PHASE");
        DamagePopup.Create(new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - 10), "PARTY PHASE", 0, 0, 1);
        int turn = 0;
        while (turn < party.Length && party[turn] != null)
        {
            yield return PartyMemberTurn(party[turn], turn);
            turn++;
        }
        runItParty = false;
        runItEnemy = true;
        isItRunningEnemy = true;
    }

    //Enemy Phase of battle overview and turn movement
    public struct Line
    {
        public GameObject enemy;
        public int xPos;
        public int yPos;
    }

    IEnumerator EnemyPhase()
    {
        Debug.Log("ENEMY PHASE");
        DamagePopup.Create(new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - 10), "ENEMY PHASE", 1, 0, 0);
        //yield return new WaitForSeconds(1);
        isItRunningEnemy = false;
        bool swapIt = false;
        int numberOfDead = 0;
        bool bopIt = false;
        for(int x = 0; x < maxRowSize; x++)
        {
            if(front[x].gameObject.activeSelf == false)
            {
                numberOfDead++;
            }
        }
        if(numberOfDead == maxRowSize)
        {
            bopIt = true;
        }
        if(bopIt)
        {
            swapIt = true;
        }
        if(swapIt == true)
        {
            Debug.Log("SWAPPING");
            GameObject[] timp = new GameObject[maxRowSize];
            for(int x = 0; x < maxRowSize; x++)
            {
                timp[x] = back[x];
            }
            for (int x = 0; x < maxRowSize; x++)
            {
                back[x] = mid[x];
            }
            for (int x = 0; x < maxRowSize; x++)
            {
                mid[x] = front[x];
            }
            for (int x = 0; x < maxRowSize; x++)
            {
                front[x] = timp[x];
            }
        }

        SetUpEnemies();

        //DO MOVES
        for (int row = 0; row < maxNumberOfRows; row++)
        {
            for (int col = 0; col < maxRowSize; col++)
            {
                //Debug.Log("in row " + row + " and col " + col);
                switch(row)
                {
                    case 0:
                        if(front[col].transform.GetChild(0).gameObject.activeSelf == true)
                        {
                            yield return EnemyTurn(front[col].GetComponentInChildren<EnemyClass>(), row, col);
                        }
                        break;
                    case 1:
                        if(mid[col].transform.GetChild(0).gameObject.activeSelf == true)
                        {
                            yield return EnemyTurn(mid[col].GetComponentInChildren<EnemyClass>(), row, col);
                        }
                        break;
                    case 2:
                        if(back[col].transform.GetChild(0).gameObject.activeSelf == true)
                        {
                            yield return EnemyTurn(back[col].GetComponentInChildren<EnemyClass>(), row, col);
                        }
                        break;
                    default:
                        throw new System.Exception("Default in enemy turn cycle");
                }

                
            }
        }

        runItEnemy = false;
        runItParty = true;
        isItRunningParty = true;
    }
}