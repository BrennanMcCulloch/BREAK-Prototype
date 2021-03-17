using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleClass : MonoBehaviour
{
    private static int maxPartySize = 3;
    private static int maxRowSize = 4;
    private static int maxNumberOfRows = 3;

    public PartyMemberClass leader;
    public PartyMemberClass[] party;
    public EnemyClass[,] enemies = new EnemyClass[maxNumberOfRows, maxRowSize]; //3 is the number of rows (front, mid, back), 4 is the number of potential enemies in each row
    public string difficulty;

    public PartyMemberClass[] restOfParty = new PartyMemberClass[maxPartySize];

    BattleClass(PartyMemberClass _lead, PartyMemberClass[] _party, EnemyClass[][] _enemy, string _bpm)
    {
        leader = _lead;

        //initializing rest of party array
        for(int x = 0; x < _party.Length; x++)
        {
            if (x >= maxPartySize) { throw new System.Exception("Tried to put too many people in the party!"); }
            restOfParty[x] = _party[x];
        }


        //initializing party array with random assortment besides leader
        party = new PartyMemberClass[_party.Length + 1];
        int currentSlot = 0;
        for (float x = restOfParty.Length; x > 0; x--)
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
        for(int x = 0; x < _enemy.Length; x++)
        {
            if (x >= maxNumberOfRows) { throw new System.Exception("Tried to make too many rows!"); }

            for(int y = 0; y < _enemy[x].Length; y++)
            {
                if(y >= maxRowSize) { throw new System.Exception("Tried to put too many enemies in a row!"); }

                enemies[x, y] = _enemy[x][y];
            }
        }

        difficulty = _bpm;
    }

    private void PartyPhase()
    {

    }

    private void PartyMemberTurn()
    {

    }

    private void EnemyPhase()
    {

    }

    private void EnemyTurn()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
