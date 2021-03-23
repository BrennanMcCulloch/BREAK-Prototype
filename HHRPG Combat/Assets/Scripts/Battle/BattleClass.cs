using System.Collections;
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

    //Booleans for the Update loop state machine
    private bool runItParty = false;
    private bool isItRunningParty = false;
    private bool runItEnemy = false;
    private bool isItRunningEnemy = false;
    private bool click = false;

    private void Start()
    {
        enemies = new EnemyClass[maxNumberOfRows, maxRowSize];
        setUpEnemies();
        SetUpParty();
        runItParty = true;
        isItRunningParty = true;
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

    public void setUpEnemies()
    {
        for (int x = 0; x < maxNumberOfRows; x++)
        {
            for (int y = 0; y < maxRowSize; y++)
            {
                switch (x)
                {
                    case 0:
                        enemies[x, y] = front[y];
                        break;
                    case 1:
                        enemies[x, y] = mid[y];
                        break;
                    case 2:
                        enemies[x, y] = back[y];
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
        toDo = null;
        var partyButtons = Instantiate(PartyButtons);
        var xPos = person.gameObject.transform.position.x;
        var zPos = person.gameObject.transform.position.z;
        //POSITIONING UI ELEMENTS
        Vector3 temp = new Vector3(xPos * (-Screen.width / (Screen.width / 50)), (zPos + 18) * (-0.2f * Mathf.Abs(21 + zPos)) * (Screen.height / (Screen.height / 50)), 1);
        partyButtons.gameObject.transform.position = temp;
        partyButtons.transform.SetParent(canvas.transform, false);

        foreach (Button but in partyButtons.GetComponentsInChildren<Button>())
        {
            but.onClick.AddListener(() => changeToDo(but.GetComponentInChildren<Text>().text));
            if(leader != 0 && but.GetComponentInChildren<Text>().text == "EQ") { but.gameObject.SetActive(false); }
        }

        while(toDo == null)
        {
            //PUT INTERACTIVE STUFF HERE

            yield return null;
            while(toDo == "EQ")
            {
                yield return null;
                Debug.Log("EQ");
                yield break;
            }
            
            while(toDo == "Attack")
            {
                yield return null;

                yield break;
            }

            while(toDo == "Guard")
            {
                yield return null;

                yield break;
            }

            while(toDo == "Rhythm")
            {
                yield return null;

                yield break;
            }

            while(toDo == "BREAK")
            {
                yield return null;

                yield break;
            }
        }

        DestroyImmediate(partyButtons);


        /*Raycast to get enemy information
        RaycastHit hit;
        Ray ray;
        bool hitEnemy = false;

        while (hitEnemy == false)
        {
            yield return _WaitForInputClick(); 

            ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.GetComponent<EnemyClass>() != null)
                {
                    Debug.Log(hit.transform.gameObject.name);
                    hitEnemy = true;
                }
            }
        }
        */
    }

    public void changeToDo(string thing)
    {
        Debug.Log(thing);
        toDo = thing;
    }

    IEnumerator _WaitForInputClick()
    {
        yield return null; //need this to not duplicate click choices
        bool temp = Input.GetMouseButtonDown(0);
        while(temp == false)
        {
            yield return null;
            temp = Input.GetMouseButtonDown(0);
        }
        yield break;
    }


    /*
    *
    * WORK NEEDS TO BE DONE HERE
    * 
    */
    IEnumerator PartyMove(PartyMemberClass doerP, GameObject victimP, MoveClass moveP)
    {
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
                int crit = 1;
                if (d20 >= 19 || affinityInQuestion == "Weak") { crit = 2; }

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
                    break;
                }

                //If not, calculate damage
                double damagePercent = (crit * percent) + 0.6;
                int potentialDamage;
                if (moveP.type == "Physical")
                {
                    doerP.stats.TryGetValue("Physical", out potentialDamage);
                    Modifier buffsPhysical;
                    victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.TryGetValue("Physical", out buffsPhysical);
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
                    victimP.gameObject.GetComponent<EnemyClass>().buffDebuff.TryGetValue("Rhythm", out buffsRhythm);
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

                double damageNotRounded = damageDealt * percentDefended;
                if (affinityInQuestion == "Strong") { damageNotRounded = damageNotRounded * 0.5; }
                int damage = (int)System.Math.Round(damageNotRounded);

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
                }
                else if (affinityInQuestion == "Reflect") { doerP.currentHealth -= damage; }//REFLECT (FIX LATER)
                else { victimP.gameObject.GetComponent<EnemyClass>().currentHealth -= damage; }

                //DISPLAY IT
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doerP.name + " did a " + moveP.moveName + " on " + victimP.gameObject.GetComponent<EnemyClass>().name + " for " + damage);

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
                }
                else
                {
                    throw new System.Exception("No difficulty selected in battle class");
                }

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
                if (d20 >= 19 || affinityInQuestion == "Weak") { crit = 2; }

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
                    break;
                }

                //If not, calculate damage
                double damagePercent = (crit * percent) + 0.6;
                int potentialDamage;
                if (move.type == "Physical")
                {
                    doer.stats.TryGetValue("Physical", out potentialDamage);
                    Modifier buffsPhysical;
                    victim.buffDebuff.TryGetValue("Physical", out buffsPhysical);
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
                    victim.buffDebuff.TryGetValue("Rhythm", out buffsRhythm);
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

                double damageNotRounded = damageDealt * percentDefended;
                if (affinityInQuestion == "Strong") { damageNotRounded = damageNotRounded * 0.5; }
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
                }
                else if (affinityInQuestion == "Reflect") { doer.currentHealth -= damage; }//REFLECT (FIX LATER)
                else { victim.currentHealth -= damage; }

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

                int maxHealth;
                enemies[xPos, yPos].stats.TryGetValue("HP", out maxHealth);
                float healies = move.effective * maxHealth;
                int heals = (int)Mathf.Round(healies);
                enemies[xPos, yPos].currentHealth += heals;
                if(enemies[xPos, yPos].currentHealth > maxHealth) { enemies[xPos, yPos].currentHealth = maxHealth;  }

                //DISPLAY IT
                yield return new WaitForSeconds(1.0f);
                Debug.Log(doer.name + " did a " + move.moveName + " on " + enemies[xPos, yPos].name + " for " + heals);

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
        }

        int lowest = 100;
        int pos = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (lowest > information[x].stat)
            {
                lowest = information[x].stat;
                pos = x;
            }
        }

        return party[pos];
    }

    private PartyMemberClass PickBestPartyToDebuff(string type)
    {
        data[] information = new data[party.Length];
        for (int x = 0; x < party.Length; x++)
        {
            int temp;
            party[x].stats.TryGetValue(type, out temp);
            information[x].stat = temp;
        }

        int highest = -100;
        int pos = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (highest < information[x].stat)
            {
                highest = information[x].stat;
                pos = x;
            }
        }

        return party[pos];
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
        }

        //pick WORST one
        int lowestPoint = 100;
        int lowestPerson = -1;
        for(int x = 0; x < party.Length; x++)
        {
            if(information[x].points <= lowestPoint)
            {
                lowestPoint = information[x].points;
                lowestPerson = x;
            }
        }

        victim = party[lowestPerson];
        return victim;
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
        }

        //pick BEST one
        int highestPoint = -100;
        int highestPerson = -1;
        for (int x = 0; x < party.Length; x++)
        {
            if (information[x].points > highestPoint)
            {
                highestPoint = information[x].points;
                highestPerson = x;
            }
        }

        victim = party[highestPerson];
        return victim;
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