using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMemberClass : MonoBehaviour
{
    public string memberName;
    public Dictionary<string, string> affinities;
    public Dictionary<string, int> stats;
    public MoveClass[] moves; //MAKE THIS SIZE 8

    PartyMemberClass(string _name, Dictionary<string, string> _aff, Dictionary<string, int> _stats, MoveClass[] _moves)
    {
        memberName = _name;
        affinities = _aff;
        stats = _stats;
        moves = _moves;
    }
}