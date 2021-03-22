using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleClassWrapper : MonoBehaviour
{
    //Booleans for the Update loop state machine
    private bool runItParty = false;
    private bool isItRunningParty = false;
    private bool runItEnemy = false;
    private bool isItRunningEnemy = false;

    public PartyMemberClass leader;
    public PartyMemberClass[] restOfParty;
    public int maxRowSize;
    public string difficulty;

    public EnemyClass[] front;
    public EnemyClass[] mid;
    public EnemyClass[] back;

    public Camera cam;
    private bool click; 

    BattleClass battle;

    // Start is called before the first frame update
    void Start()
    {
        click = false;
        runItParty = true;
        EnemyClass[,] enemies = new EnemyClass[3, maxRowSize];

        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < maxRowSize; y++)
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
                        throw new System.Exception("Fell into default in the battle class wrapper");
                }
            }
        }

        battle = new BattleClass(leader, maxRowSize, restOfParty, enemies, difficulty, cam);
    }

    private void Update()
    {
        var view = cam.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        if (isOutside == false)
        {
            if(click == false)
            {
                click = Input.GetMouseButton(0);
            }
            else
            {
                if (runItParty)
                {
                    isItRunningParty = true;
                    if (isItRunningParty) { battle.PartyPhase(ref isItRunningParty, ref runItParty, ref runItEnemy); }
                }
                else if (runItEnemy)
                {
                    isItRunningEnemy = true;
                    if (isItRunningEnemy) { battle.EnemyPhase(ref isItRunningEnemy, ref runItParty, ref runItEnemy); }
                }

            }
        }
    }
}
