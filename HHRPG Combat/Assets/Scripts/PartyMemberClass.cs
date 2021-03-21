using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PartyMemberClass : MonoBehaviour
{
    public string memberName;
    public int currentHealth;
    public int currentEP;
    public bool currentlyGuarding = false;
    public ModifierDictionary buffDebuff;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public GameObject[] moves; //MAKE THIS SIZE 8
    public GameObject harmonic;

    PartyMemberClass(string _name, AffinityDictionary _aff, StatDictionary _stats, GameObject[] _moves, GameObject _harm)
    {
        memberName = _name;
        affinities = _aff;
        stats = _stats;
        moves = _moves;
        harmonic = _harm;
    }

}

public class Modifier
{
    public string statName { get; set; }
    public int amount { get; set; }
    public int turnTime { get; set; }
}

[System.Serializable]
public class AffinityDictionary : SerializableDictionary<string, string> { }

[System.Serializable]
public class ModifierDictionary : SerializableDictionary<string, Modifier> { }

[System.Serializable]
public class StatDictionary : SerializableDictionary<string, int> { }

[CustomPropertyDrawer (typeof(AffinityDictionary))]
public class MyDictionaryDrawerAffinity : DictionaryDrawer<string, string> { }

[CustomPropertyDrawer (typeof(StatDictionary))]
public class MyDictionaryDrawerStats : DictionaryDrawer<string, int> { }
