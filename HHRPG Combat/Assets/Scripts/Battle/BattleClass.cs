﻿using System.Collections;
using System.Collections.Generic;
using MEC; //coroutine stuff
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

/*
 * Ok, so, this file is long as heck. So here's an explanation.
 * Basically, the battle class gets attached to an empty gameobject in the scene,
 * and handles all of the enemyAI and move handling,
 * along with player interactivity. 
 * The EnemyMove method is the longest, but it's not too complex if ya sit and think. And also breathe. A lot.
 * You can do this. :)
 */

public class BattleClass : MonoBehaviour
{
    public GameObject canvas;
    public GameObject PartyButtons;
    public GameObject RhythmButtons;
    public GameObject EQButton;

    public Camera camera;

    public string toDo;

    public PartyMemberClass leader;
    private PartyMemberClass[] party;
    public string difficulty;

    private static int maxPartySize = 3;
    public int maxRowSize = 1;
    private static int maxNumberOfRows = 3;

    public EnemyClass[] front;
    public EnemyClass[] mid;
    public EnemyClass[] back;
    public EnemyClass[,] enemies;

    public PartyMemberClass[] restOfParty;

    private MoveClass currentMove;
    private bool recentlyChained = false;

    private bool currentlyBreaking = false;

    private bool groupMove = false;
    private bool lastCrit = false;

    //Booleans for the Update loop state machine
    private bool runItParty = false;
    private bool isItRunningParty = false;
    private bool runItEnemy = false;
    private bool isItRunningEnemy = false;
    private bool click = false;

    private List<ChainClass> chains = new List<ChainClass>();
    List<PartyMemberClass> partyMembersChained = new List<PartyMemberClass>();

    private void Start()
    {
        enemies = new EnemyClass[maxNumberOfRows, maxRowSize];
        SetUpEnemies();
        SetUpParty();
        runItParty = true;
        isItRunningParty = true;
        toDo = null;
    }

    private void Update()
    {
        var view = camera.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        if (isOutside == false)
        {
            if (click == false)
            {
                click = Input.GetMouseButtonDown(0);
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
    }


    public void SetUpEnemies()
    {
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                switch (x)
                {
                    case 0:
                        enemies[x, y] = front[y];

                        float xPosFront = (y - (Mathf.Abs(maxRowSize - 1) / 2) * (Screen.width / 3)); //3 is MAGIC NUMBER. bad.
                        float yPosFront = x * 4.2f + (enemies[x, y].gameObject.transform.localScale.y / 2) - 1;
                        float zPosFront = -30 - (x * 10);
                        Vector3 positionFront = new Vector3(xPosFront, yPosFront, zPosFront);

                        positionFront.y += (enemies[x, y].gameObject.transform.localScale.y / 2);
                        enemies[x, y].gameObject.transform.position = positionFront;

                        break;
                    case 1:
                        enemies[x, y] = mid[y];

                        float xPosMid = (y - (Mathf.Abs(maxRowSize - 1) / 2) * (Screen.width / 3)); //3 is MAGIC NUMBER. bad.
                        float yPosMid = x * 4.2f + (enemies[x, y].gameObject.transform.localScale.y / 2);
                        float zPosMid = -30 - (x * 10); ;
                        Vector3 positionMid = new Vector3(xPosMid, yPosMid, zPosMid);

                        positionMid.y += (enemies[x, y].gameObject.transform.localScale.y / 2);
                        enemies[x, y].gameObject.transform.position = positionMid;

                        break;
                    case 2:
                        enemies[x, y] = back[y];

                        float xPosBack = (y - (Mathf.Abs(maxRowSize - 1) / 2) * (Screen.width / 3)); //3 is MAGIC NUMBER. bad.
                        float yPosBack = x * 4.2f + (enemies[x, y].gameObject.transform.localScale.y / 2);
                        float zPosBack = -30 - (x * 10); ;
                        Vector3 positionBack = new Vector3(xPosBack, yPosBack, zPosBack);

                        positionBack.y += (enemies[x, y].gameObject.transform.localScale.y / 2);
                        enemies[x, y].gameObject.transform.position = positionBack;

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

    /*
     *
     * WORK NEEDS TO BE DONE HERE
     * 
     */
    //Interactivity code for party member turn
    IEnumerator PartyMemberTurn(PartyMemberClass person, int leader)
    {
        person.currentlyGuarding = false;
        toDo = null;
        currentlyBreaking = false;
        var partyButtons = Instantiate(PartyButtons);
        var rhythmButtons = Instantiate(RhythmButtons);
        var EQMenu = Instantiate(EQButton);
        var xPos = person.gameObject.transform.position.x;
        var zPos = person.gameObject.transform.position.z;

        //POSITIONING UI ELEMENTS
        Vector3 temp = new Vector3(xPos * (-Screen.width / (Screen.width / 30)), (zPos + 18) * (-0.2f * Mathf.Abs(21 + zPos)) * (Screen.height / (Screen.height / 25)), 1);
        partyButtons.gameObject.transform.position = temp;
        partyButtons.transform.SetParent(canvas.transform, false);
        foreach (Button but in partyButtons.GetComponentsInChildren<Button>())
        {
            but.onClick.AddListener(() => changeToDo(but.GetComponentInChildren<Text>().text));
            if (leader != 0 && but.GetComponentInChildren<Text>().text == "EQ") { but.gameObject.SetActive(false); }
            if ((but.GetComponentInChildren<Text>().text == "BREAK") || (but.GetComponentInChildren<Text>().text == "Harmonic"))
            {
                if(person.currentlyChained == false)
                {
                    but.gameObject.SetActive(false);
                }
            }
        }

        rhythmButtons.gameObject.transform.position = temp;
        rhythmButtons.transform.SetParent(canvas.transform, false);
        for (int x = 0; x < person.moves.Length; x++)
        {
            MoveClass moveIt = person.moves[x].GetComponent<MoveClassWrapper>().MoveClass;
            string title = "Button " + x;
            Button thing = rhythmButtons.transform.Find(title).gameObject.GetComponent<Button>();
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

        while (toDo == null && currentlyBreaking == false && groupMove == false && person.currentHealth > 0)
        {
            //PUT INTERACTIVE STUFF HERE
            yield return null;
            while(toDo == "EQ")
            {
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

                    ray = camera.ScreenPointToRay(Input.mousePosition);
                    if(Physics.Raycast(ray, out hit))
                    {
                        for(int x = 0; x < maxNumberOfRows; x++)
                        {
                            for(int y = 0; y < maxRowSize; y++)
                            {
                                if (hit.transform.gameObject.GetComponent<EnemyClass>() == enemies[x, y]) //for each flat, if you've hit THAT flat when releasing the mouse
                                {
                                    //DO THE EXCHANGE
                                    if(x == 0)
                                    {
                                        EnemyClass vic = front[y];
                                        front[y] = enemyClicked.GetComponent<EnemyClass>();
                                        if (xEnemy == 0) { front[yEnemy] = vic; }
                                        else if (xEnemy == 1) { mid[yEnemy] = vic; }
                                        else if (xEnemy == 2) { back[yEnemy] = vic; }

                                        SetUpEnemies();

                                        Debug.Log(x + " " + y + " switched with " + xEnemy + " " + yEnemy);
                                    }
                                    else if(x == 1)
                                    {
                                        EnemyClass vicMid = mid[y];
                                        mid[y] = enemyClicked.GetComponent<EnemyClass>();
                                        if (xEnemy == 0) { front[yEnemy] = vicMid; }
                                        else if (xEnemy == 1) { mid[yEnemy] = vicMid; }
                                        else if (xEnemy == 2) { back[yEnemy] = vicMid; }

                                        SetUpEnemies();

                                        Debug.Log(x + " " + y + " switched with " + xEnemy + " " + yEnemy);
                                    }
                                    else if(x == 2)
                                    {
                                        EnemyClass vicB = back[y];
                                        back[y] = enemyClicked.GetComponent<EnemyClass>();
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
                        toDo = "EQ";
                        enemyClicked = null;
                        xEnemy = -1;
                        yEnemy = -1;
                    }
                }

                partyButtons.SetActive(true);
                EQMenu.SetActive(false);
                enemyClicked = null;
                xEnemy = -1;
                yEnemy = -1;
            }
            
            while(toDo == "Attack")
            {
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
                        {
                            MoveClass hitThem = new MoveClass("Attack", "Physical", 1.0f, false, 0, false, false);

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
                yield return _WaitForInputClick();

                while (toDo == "Rhythm Targeting")
                {
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
                                    yield return PartyMove(person, homie.gameObject, currentMove);
                                }
                            }
                            else
                            {
                                //DID THIS SO GROUP ATTACKS DON'T CAUSE MULTIPLE CALLBACKS
                                groupMove = true;
                                for(int x = 0; x < maxNumberOfRows; x++)
                                {
                                    for(int y = 0; y < maxRowSize; y++)
                                    {
                                        yield return PartyMove(person, enemies[x, y].gameObject, currentMove);
                                    }
                                }
                                groupMove = false;
                                if(lastCrit)
                                {
                                    yield return PartyMemberTurn(person, 1);
                                    lastCrit = false;
                                }
                            }
                            toDo = "Done";
                        }
                        else
                        {
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
                                        yield return PartyMove(person, hit.transform.gameObject, currentMove);
                                        toDo = "Done";
                                    }
                                }
                                else
                                {
                                    if (hit.transform.gameObject.GetComponent<EnemyClass>() != null)
                                    {
                                        person.currentEP -= currentMove.cost; //UPDATE EP
                                        yield return PartyMove(person, hit.transform.gameObject, currentMove);
                                        toDo = "Done";
                                    }
                                }
                            }
                        }
                    }

                    yield return null;
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
                int totalEnemies = 0;
                int totalChains = 0;
                List<EnemyClass> enemiesChained = new List<EnemyClass>();
                foreach (ChainClass chainThing in chains)
                {
                    totalChains++;
                    if(enemiesChained.Contains(chainThing.chainVictim.GetComponent<EnemyClass>()) == false)
                    {
                        totalEnemies++;
                        enemiesChained.Add(chainThing.chainVictim.GetComponent<EnemyClass>());
                    }
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
                }

                int resultingDamage = totalPotential * (totalChains * totalChains);
                //Debug.Log(totalPotential);
                Debug.Log("Chained enemies: " + totalEnemies);
                foreach (EnemyClass badGuy in enemiesChained)
                {
                    badGuy.currentHealth -= resultingDamage;
                    Debug.Log("Chain break! " + badGuy.name + " was broken for " + resultingDamage);
                    Vector3 chainposition = badGuy.transform.position;
                    chainposition.z += 2;
                    DamagePopup.Create(chainposition, resultingDamage.ToString(), 0, 1, 1);
                }

                //partyButtons.SetActive(true);
                partyMembersChained = new List<PartyMemberClass>();
                toDo = "Done";
            }

            while (toDo == "Harmonic")
            {
                yield return null;
                Debug.Log("Harmonic");

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
                            EQMenu.SetActive(false);
                            yield return PartyMemberTurn(friendlyperson, 1);
                            person.currentlyChained = false;
                            toDo = "Done";
                        }
                        else
                        {
                            Debug.Log("Cannot harmonic members currently chained");
                        }
                    }
                }

                partyButtons.SetActive(true);
                EQMenu.SetActive(false);
                toDo = "Done";
            }
        }

        toDo = "Turn Ended";
        DestroyImmediate(partyButtons);
        DestroyImmediate(rhythmButtons);
        chains = new List<ChainClass>();

        //Debug.Log(person.name + "'s turn just ended");

        recentlyChained = false;
        person.currentlyChained = false;
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
                int d20 = Random.Range(1, 21);
                double percent = d20 * 0.02;
                int statInQuestion;
                string affinityInQuestion;
                int agility;
                victimP.gameObject.GetComponent<EnemyClass>().affinities.TryGetValue(moveP.type, out affinityInQuestion);
                victimP.gameObject.GetComponent<EnemyClass>().stats.TryGetValue("Agility", out agility);
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
                    yield return new WaitForSeconds(0.5f);
                    break;
                }

                int crit = 1;
                if (d20 >= 19 || affinityInQuestion == "Weak")//ADD CHAIN
                {
                    DamagePopup.Create(affinityPosition, "CRITICAL", 0, 1, 1);
                    partyMembersChained.Add(doerP);
                    yield return new WaitForSeconds(0.5f);
                    crit = 2;
                    ChainClass surprise = new ChainClass();
                    surprise.chainHolder = doerP;
                    surprise.chainVictim = victimP.GetComponent<EnemyClass>();
                    Debug.Log("CHAINED " + surprise.chainHolder.name + " " + surprise.chainVictim.name);
                    if (doerP.currentlyChained == false && chains.Contains(surprise) == false)
                    {
                        chains.Add(surprise);
                        doerP.currentlyChained = true;
                    }
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
                    DamagePopup.Create(affinityPosition, "STRONG", 1, 1, 1);
                    yield return new WaitForSeconds(0.5f);
                }
                int damage = (int)System.Math.Round(damageNotRounded);
                affinityPosition.y += 3;

                //Impact numbers
                if (affinityInQuestion == "Absorb")
                {
                    victimP.gameObject.GetComponent<EnemyClass>().currentHealth += damage;
                    int maxHealthParty;
                    victimP.gameObject.GetComponent<PartyMemberClass>().stats.TryGetValue("HP", out maxHealthParty);
                    if (victimP.gameObject.GetComponent<EnemyClass>().currentHealth > maxHealthParty)
                    {
                        victimP.gameObject.GetComponent<EnemyClass>().currentHealth = maxHealthParty;
                    }
                    DamagePopup.Create(damagePosition, damage.ToString(), 0, 1, 0);
                }

            /*
             *
             * DO WORK HERE
             * 
             */

                else if (affinityInQuestion == "Reflect")
                {
                    doerP.currentHealth -= damage;
                }//REFLECT (FIX LATER)
                else
                {
                    victimP.gameObject.GetComponent<EnemyClass>().currentHealth -= damage;
                    DamagePopup.Create(damagePosition, damage.ToString(), 1, 0, 0);

                }

                //DISPLAY IT
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + victimP.gameObject.GetComponent<EnemyClass>().name + " for " + damage);

                //IF CHAINED, GO BACK TO TOP OF LOOP THING
                if(crit > 1)
                {
                    if(moveP.group == true)
                    {
                        lastCrit = true;
                    }
                    else
                    {
                        yield return PartyMemberTurn(doerP, 1);
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
                DamagePopup.Create(affinityPosition, "BUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, buff.amount.ToString(), 1, 0.92f, 0.016f);

                yield return new WaitForSeconds(1.0f);
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
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + notBuddy.name + " for " + debuff.amount);
                DamagePopup.Create(affinityPosition, "DEBUFF", 1, 0.92f, 0.016f);
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
                yield return new WaitForSeconds(1.0f);
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
        //Debug.Log("In EnemyTurn");
        switch(row)
        {
            case 0: //front row
                if(enemy.frontMoves.Length > 0)
                {
                    int whichFront = Random.Range(0, enemy.frontMoves.Length);
                    GameObject doItFront = enemy.frontMoves[whichFront];
                    MoveClassWrapper theThingFrontBasis = doItFront.GetComponent<MoveClassWrapper>();
                    MoveClass theThingFront = theThingFrontBasis.MoveClass;
                    yield return EnemyMove(enemy, theThingFront);
                }
                break;
            case 1: //mid row
                if(enemy.midMoves.Length > 0)
                {
                    int whichMid = Random.Range(0, enemy.midMoves.Length);
                    GameObject doItMid = enemy.midMoves[whichMid];
                    MoveClassWrapper theThingMidBasis = doItMid.GetComponent<MoveClassWrapper>();
                    MoveClass theThingMid = theThingMidBasis.MoveClass;
                    yield return EnemyMove(enemy, theThingMid);
                }
                break;
            case 2: //back row
                if(enemy.backMoves.Length > 0)
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
        switch (move.type)
        {
            //ALL FORMS OF ATTACK. ONCE NEW TYPES ARE ADDED, THEY GO HERE.
            case "Physical":
            case "Drum":
            case "Bass":
            case "Guitar":
            case "Piano":
                //Determine who we're attacking
                PartyMemberClass victim;
                if (difficulty == "Easy")
                {
                    //Pick the worst enemy to attack
                    victim = PickWorstPartyToHit(move.type);
                    if(victim == null)
                    {
                        break;
                    }
                }
                else if (difficulty == "Medium")
                {
                    //Randomly pick an enemy
                    victim = party[Random.Range(0, maxPartySize + 1)].GetComponent<PartyMemberClass>();
                }
                else if (difficulty == "Hard")
                {
                    //Pick the best enemy to attack
                    victim = PickBestPartyToHit(move.type);
                    if (victim == null)
                    {
                        break;
                    }
                }
                else
                {
                    throw new System.Exception("No difficulty selected in battle class");
                }

                damagePosition = victim.transform.position;
                damagePosition.z += 2;
                damagePosition.x -= 1;
                affinityPosition = victim.transform.position;
                affinityPosition.z += 2;
                affinityPosition.x -= 1;
                affinityPosition.y += 3;

                //Determine attack value
                int d20 = Random.Range(1, 21);
                double percent = d20 * 0.02;
                int statInQuestion;
                string affinityInQuestion;
                int agility;
                victim.affinities.TryGetValue(move.type, out affinityInQuestion);
                victim.stats.TryGetValue("Agility", out agility);
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
                    yield return new WaitForSeconds(0.5f);
                }

                //Do they dodge?
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
                    break;
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
                if (victim.currentlyGuarding == true) { damageNotRounded = damageNotRounded * 0.5;  }
                if (affinityInQuestion == "Strong") { damageNotRounded = damageNotRounded * 0.5; DamagePopup.Create(affinityPosition, "STRONG", 1, 1, 1); yield return new WaitForSeconds(0.5f); }
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

            /*
             *
             * DO WORK HERE
             * 
             */

                else if (affinityInQuestion == "Reflect")
                {
                    doer.currentHealth -= damage;
                }//REFLECT (FIX LATER)
                else
                {
                    victim.currentHealth -= damage;
                    DamagePopup.Create(damagePosition, damage.ToString(), 1, 0, 0);
                }

                //DISPLAY IT
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + victim.name + " for " + damage);

                break;
            //ALL BUFFS GO HERE.
            case "Strength Buff":
            case "Rhythm Buff":
            case "Physical Defence Buff":
            case "Rhythm Defence Buff":
            case "Agility Buff":
            case "Potential Buff":
                Regex regexBuff = new Regex("(\\s+( Buff)\\s*)$");
                string moveStringBuff = regexBuff.Replace(move.type, "");
                EnemyClass buddy;
                if (difficulty == "Easy")
                {

                    buddy = PickWorstEnemyToBuff(moveStringBuff);
                }
                else if (difficulty == "Medium")
                {

                    buddy = enemies[Random.Range(0, maxNumberOfRows + 1), Random.Range(0, maxRowSize + 1)].GetComponent<EnemyClass>();
                }
                else if (difficulty == "Hard")
                {

                    buddy = PickBestEnemyToBuff(moveStringBuff);
                }
                else
                {
                    throw new System.Exception("No difficulty selected in battle class");
                }

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
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + buddy.name + " for " + buff.amount);
                DamagePopup.Create(affinityPosition, "BUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, buff.amount.ToString(), 1, 0.92f, 0.016f);

                break;
            //ALL DEBUFFS GO HERE.
            case "Strength Debuff":
            case "Rhythm Debuff":
            case "Physical Defence Debuff":
            case "Rhythm Defence Debuff":
            case "Agility Debuff":
            case "Potential Debuff":
                Regex regexDebuff = new Regex("(\\s+( Debuff)\\s*)$");
                string moveStringDebuff = regexDebuff.Replace(move.type, "");
                PartyMemberClass notBuddy;
                if (difficulty == "Easy")
                {

                    notBuddy = PickWorstPartyToDebuff(moveStringDebuff);
                }
                else if (difficulty == "Medium")
                {

                    notBuddy = party[Random.Range(0, maxPartySize + 1)].GetComponent<PartyMemberClass>();
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
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + notBuddy.name + " for " + debuff.amount);
                DamagePopup.Create(affinityPosition, "DEBUFF", 1, 0.92f, 0.016f);
                DamagePopup.Create(damagePosition, debuff.amount.ToString(), 1, 0.92f, 0.016f);

                break;
            //HEALIES FOR YOUR FEELIES
            case "Heal":
                //Find the member of the enemies with the lowest health and HEAL THAT FOOL
                int lowestHealth = 500000;
                int xPos = 0;
                int yPos = 0;
                for (int x = 0; x < maxNumberOfRows; x++)
                {
                    for (int y = 0; y < maxRowSize; y++)
                    {
                        if (lowestHealth > enemies[x, y].currentHealth)
                        {
                            lowestHealth = enemies[x, y].currentHealth;
                            xPos = x;
                            yPos = y;
                        }
                    }
                }

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
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + enemies[xPos, yPos].name + " for " + heals);
                DamagePopup.Create(damagePosition, heals.ToString(), 0, 1, 0);

                break;
            default:
                yield return new WaitForSeconds(1.0f);
                Debug.Log("The enemy did nothing.");
                break;
        }
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
            int temp;
            party[x].stats.TryGetValue(type, out temp);
            information[x].stat = temp;
            int maxHealth;
            party[x].stats.TryGetValue("HP", out maxHealth);
            information[x].healthPercentage = party[x].currentHealth / maxHealth;
        }

        int lowest = 100;
        int pos = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (lowest > information[x].stat && information[x].healthPercentage != 0)
            {
                lowest = information[x].stat;
                pos = x;
            }
        }

        if (pos == -1)
        {
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
            int temp;
            party[x].stats.TryGetValue(type, out temp);
            information[x].stat = temp;
            int maxHealth;
            party[x].stats.TryGetValue("HP", out maxHealth);
            information[x].healthPercentage = party[x].currentHealth / maxHealth;
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
                int result;
                enemies[x, y].stats.TryGetValue(type, out result);
                information[x, y].stat = result;
            }
        }

        int highest = -100;
        int xPos = -1;
        int yPos = -1;
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                if(highest < information[x, y].stat)
                {
                    highest = information[x, y].stat;
                    xPos = x;
                    yPos = y;
                }
            }
        }

        return enemies[xPos, yPos];
    }

    private EnemyClass PickBestEnemyToBuff(string type)
    {
        data[,] information = new data[maxNumberOfRows, maxRowSize];
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                int result;
                enemies[x, y].stats.TryGetValue(type, out result);
                information[x, y].stat = result;
            }
        }

        int lowest = 100;
        int xPos = -1;
        int yPos = -1;
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                if (lowest >= information[x, y].stat)
                {
                    lowest = information[x, y].stat;
                    xPos = x;
                    yPos = y;
                }
            }
        }

        return enemies[xPos, yPos];
    }
    private PartyMemberClass PickWorstPartyToHit(string type)
    {
        data[] information = new data[party.Length];
        PartyMemberClass victim;

        //Establish AI data
        for(int x = 0; x < party.Length; x++)
        {
            string temp;
            int maxHealth;
            party[x].affinities.TryGetValue(type, out temp);
            party[x].stats.TryGetValue("HP", out maxHealth);
            information[x].affinity = temp;
            information[x].healthPercentage = party[x].currentHealth / maxHealth;
            information[x].currentlyGuarding = party[x].currentlyGuarding;
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

            if (information[x].healthPercentage == 0) { information[x].points = -100; }
        }

        //pick WORST one
        int lowestPoint = 100;
        int lowestPerson = -1;
        for(int x = 0; x < party.Length; x++)
        {
            if(information[x].points <= lowestPoint && information[x].points != -100)
            {
                lowestPoint = information[x].points;
                lowestPerson = x;
            }
        }

        if(lowestPerson == -1)
        {
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
            string temp;
            int maxHealth;
            party[x].affinities.TryGetValue(type, out temp);
            party[x].stats.TryGetValue("HP", out maxHealth);
            information[x].affinity = temp;
            information[x].healthPercentage = party[x].currentHealth / maxHealth;
            information[x].currentlyGuarding = party[x].currentlyGuarding;
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

            if (information[x].healthPercentage == 0) { information[x].points = -100; }
        }

        //pick BEST one
        int highestPoint = -100;
        int highestPerson = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (information[x].points > highestPoint && information[x].points != -100)
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
        int turn = 0;
        while (turn < party.Length)
        {
            yield return PartyMemberTurn(party[turn], turn);
            turn++;
        }
        runItParty = false;
        runItEnemy = true;
        isItRunningEnemy = true;
    }

    //Enemy Phase of battle overview and turn movement
    IEnumerator EnemyPhase()
    {
        Debug.Log("ENEMY PHASE");
        isItRunningEnemy = false;
        for (int row = 0; row < maxNumberOfRows; row++)
        {
            for (int col = 0; col < maxRowSize; col++)
            {
                //Debug.Log("in row " + row + " and col " + col);
                if (enemies[row, col] != null) { yield return EnemyTurn(enemies[row, col], row, col); }
            }
        }

        runItEnemy = false;
        runItParty = true;
        isItRunningParty = true;
    }
}