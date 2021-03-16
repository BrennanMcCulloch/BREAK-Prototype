using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveClass : MonoBehaviour
{
    protected string moveName;
    protected string type; //attack, rhythm, support
    protected bool group; //true if group effect, false if not
    protected int cost;
    protected bool friendly; //true if only cast on party, false if cast on enemy

    MoveClass(string _move, string _type, bool _group, int _cost, bool _friend)
    {
        moveName = _move;
        type = _type;
        group = _group;
        cost = _cost;
        friendly = _friend;
    }
}
