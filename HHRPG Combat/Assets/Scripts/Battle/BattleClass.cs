using System.Collections;
using System.Collections.Generic;
using MEC; //coroutine stuff
using UnityEngine;
using System.Text.RegularExpressions;

/*
 * Ok, so, this file is long as heck. So here's an explanation.
 * Basically, the battle class gets attached to an empty gameobject in the scene,
 * and handles all of the enemyAI and move handling,
 * along with player interactivity. 
 * The EnemyMove method is the longest, but it's not too complex if ya sit and think. And also breathe. A lot.
 * You can do this. :)
 */

public class BattleClass
{
    private Camera camera;

    public PartyMemberClass leader;
    private PartyMemberClass[] party;
    public string difficulty;

    private static int maxPartySize = 3;
    public int maxRowSize = 4;
    private static int maxNumberOfRows = 3;

    public EnemyClass[] front;
    public EnemyClass[] mid;
    public EnemyClass[] back;
    private EnemyClass[,] enemies;

    public PartyMemberClass[] restOfParty = new PartyMemberClass[maxPartySize];

    public BattleClass(PartyMemberClass _lead, int _rowSize, PartyMemberClass[] _party, EnemyClass[,] _enemy, string _bpm, Camera _cam)
    {
        leader = _lead;
        maxRowSize = _rowSize;
        //enemies = new EnemyClass[maxNumberOfRows, maxRowSize];

        //initializing rest of party array
        for (int x = 0; x < _party.Length; x++)
        {
            if (x >= maxPartySize) { throw new System.Exception("Tried to put too many people in the party!"); }
            restOfParty[x] = _party[x];
        }


        //initializing party array with random assortment besides leader
        party = new PartyMemberClass[_party.Length + 1];
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

        //initializing enemy 2D array
        enemies = _enemy;

        difficulty = _bpm;

        camera = _cam;
    }


    /*
     *
     * WORK NEEDS TO BE DONE HERE
     * 
     */
    //Interactivity code for party member turn
    private void PartyMemberTurn(PartyMemberClass person, int leader)
    {
        //PUT INTERACTIVE STUFF HERE
        Debug.Log("In Party Member Turn for " + person.memberName);

        //Raycast to get enemy information
        RaycastHit hit;
        Ray ray;
        ray = camera.ScreenPointToRay(Input.mousePosition);
        bool hitEnemy = false;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.GetComponent<EnemyClass>().enemyName != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                hitEnemy = true;
            }
       
        }
    }

    IEnumerator<float> _WaitForInputClick()
    {
        while(true)
        {
            //Debug.Log("in coroutine");
            bool temp = Input.GetMouseButtonDown(0);
            if(temp)
            {
                break;
            }
            yield return Timing.WaitForOneFrame;
        }
    } 


    /*
    *
    * WORK NEEDS TO BE DONE HERE
    * 
    */
    private void PartyMove()
    {
        //MORE INTERACTIVE STUFF HERE
    }

    //Things to do on enemy turn
    private void EnemyTurn(EnemyClass enemy, int row, int col)
    {
        switch(row)
        {
            case 0: //front row
                if(enemy.frontMoves.Length > 0)
                {
                    int whichFront = Random.Range(0, enemy.frontMoves.Length);
                    GameObject doItFront = enemy.frontMoves[whichFront];
                    MoveClassWrapper theThingFrontBasis = doItFront.GetComponent<MoveClassWrapper>();
                    MoveClass theThingFront = theThingFrontBasis.MoveClass;
                    EnemyMove(enemy, theThingFront);
                }
                break;
            case 1: //mid row
                if(enemy.midMoves.Length > 0)
                {
                    int whichMid = Random.Range(0, enemy.midMoves.Length);
                    GameObject doItMid = enemy.midMoves[whichMid];
                    MoveClassWrapper theThingMidBasis = doItMid.GetComponent<MoveClassWrapper>();
                    MoveClass theThingMid = theThingMidBasis.MoveClass;
                    EnemyMove(enemy, theThingMid);
                }
                break;
            case 2: //back row
                if(enemy.backMoves.Length > 0)
                {
                    int whichBack = Random.Range(0, enemy.backMoves.Length);
                    GameObject doItBack = enemy.backMoves[whichBack];
                    MoveClassWrapper theThingBackBasis = doItBack.GetComponent<MoveClassWrapper>();
                    MoveClass theThingBack = theThingBackBasis.MoveClass;
                    EnemyMove(enemy, theThingBack);
                }
                break;
            default:
                throw new System.Exception("You fell into default in the enemy turn function of battle class");
        }

    }


    //actual enemy attack
    private void EnemyMove(EnemyClass doer, MoveClass move)
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
                if (affinityInQuestion == "Absorb") { victim.currentHealth += damage; }
                else if (affinityInQuestion == "Reflect") { doer.currentHealth -= damage; }//REFLECT (FIX LATER)
                else { victim.currentHealth -= damage; }

                //DISPLAY IT
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
                Debug.Log(doer.name + " did a " + move.moveName + " on " + enemies[xPos, yPos].name + " for " + heals);

                break;
            default:
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
    public void PartyPhase(ref bool isItRunningParty, ref bool runItParty, ref bool runItEnemy)
    {
        isItRunningParty = false;
        Debug.Log("PARTY PHASE");
        int turn = 0;
        while (turn < party.Length)
        {
            PartyMemberTurn(party[turn], turn);
            turn++;
        }

        runItParty = false;
        runItEnemy = true;
    }

    //Enemy Phase of battle overview and turn movement
    public void EnemyPhase(ref bool isItRunningEnemy, ref bool runItParty, ref bool runItEnemy)
    {
        Debug.Log("ENEMY PHASE");
        isItRunningEnemy = false;
        for (int row = 0; row < maxNumberOfRows; row++)
        {
            for (int col = 0; col < maxRowSize; col++)
            {
                if (enemies[row, col] != null) { EnemyTurn(enemies[row, col], row, col); }
            }
        }

        runItEnemy = false;
        runItParty = true;
    }
}