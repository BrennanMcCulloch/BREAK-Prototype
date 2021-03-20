using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    public string enemyName;
    public int currentHealth;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public GameObject[] frontMoves;
    public GameObject[] midMoves;
    public GameObject[] backMoves;

    EnemyClass(string _name, AffinityDictionary _aff, StatDictionary _stats, GameObject[] _front, GameObject[] _mid, GameObject[] _back)
    {
        enemyName = _name;
        affinities = _aff;
        stats = _stats;
        frontMoves = _front;
        midMoves = _mid;
        backMoves = _back;
    }
}
