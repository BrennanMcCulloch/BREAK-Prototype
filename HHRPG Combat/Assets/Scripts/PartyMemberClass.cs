using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PartyMemberClass : MonoBehaviour
{
    public string memberName;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public MoveClass[] moves; //MAKE THIS SIZE 8
    public MoveClass harmonic;

    PartyMemberClass(string _name, AffinityDictionary _aff, StatDictionary _stats, MoveClass[] _moves, MoveClass _harm)
    {
        memberName = _name;
        affinities = _aff;
        stats = _stats;
        moves = _moves;
        harmonic = _harm;
    }
}

[System.Serializable]
public class AffinityDictionary : SerializableDictionary<string, string> { }

[System.Serializable]
public class StatDictionary : SerializableDictionary<string, int> { }

[CustomPropertyDrawer (typeof(AffinityDictionary))]
public class MyDictionaryDrawerAffinity : DictionaryDrawer<string, string> { }

[CustomPropertyDrawer (typeof(StatDictionary))]
public class MyDictionaryDrawerStats : DictionaryDrawer<string, int> { }
