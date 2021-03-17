using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class MoveClass : MonoBehaviour
{
    protected string moveName;
    protected string type; //attack, rhythm, support
    protected bool group; //true if group effect, false if not
    protected int cost;
    protected bool friendly; //true if only cast on party, false if cast on enemy
    protected bool harmonic; //true if it's a move for a harmonic

    MoveClass(string _move, string _type, bool _group, int _cost, bool _friend, bool _harm)
    {
        moveName = _move;
        type = _type;
        group = _group;
        cost = _cost;
        friendly = _friend;
        harmonic = _harm;
    }

    public string GetName()
    {
        return moveName;
    }

    public string GetMoveType()
    {
        return type;
    }

    public bool GetGroup()
    {
        return group;
    }

    public int GetCost()
    {
        return cost;
    }

    public bool GetFriend()
    {
        return friendly;
    }

    public bool GetHarm()
    {
        return harmonic;
    }
}
