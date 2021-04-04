﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class MoveClass
{
    [SerializeField]
    public string moveName;
    public string type; //attack, rhythm, support
    public float effective; //for support skills, use this stat as a percentage 0-1
    public bool group; //true if group effect, false if not
    public int cost;
    public bool friendly; //true if only cast on party, false if cast on enemy
    public string description;
    public bool harmonic; //true if it's a move for a harmonic

    public MoveClass(string _move, string _type, float _eff, bool _group, int _cost, bool _friend, string _desc, bool _harm)
    {
        moveName = _move;
        type = _type;
        effective = _eff;
        group = _group;
        cost = _cost;
        friendly = _friend;
        description = _desc;
        harmonic = _harm;
    }

    public string GetName()
    {
        return moveName;
    }

    public void SetName(string _n)
    {
        moveName = _n;
    }

    public string GetMoveType()
    {
        return type;
    }

    public void SetMoveType(string _m)
    {
        type = _m;
    }

    public float GetEffective()
    {
        return effective;
    }

    public void SetEffective(float _f)
    {
        effective = _f;
    }

    public bool GetGroup()
    {
        return group;
    }

    public void SetGroup(bool _b)
    {
        group = _b;
    }

    public int GetCost()
    {
        return cost;
    }

    public void SetCost(int _c)
    {
        cost = _c;
    }

    public bool GetFriend()
    {
        return friendly;
    }

    public void SetFriend(bool _f)
    {
        friendly = _f;
    }

    public string GetDesc()
    {
        return description;
    }

    public void SetDesc(string _d)
    {
        description = _d;
    }

    public bool GetHarm()
    {
        return harmonic;
    }

    public void SetHarm(bool _h)
    {
        harmonic = _h;
    }
}

[System.Serializable]
public class MoveClassArray
{
    public List<MoveClass> MoveClass;
}