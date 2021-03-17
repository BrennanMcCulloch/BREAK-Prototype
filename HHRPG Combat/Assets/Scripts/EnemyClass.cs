using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    public string enemyName;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public MoveClass[] frontMoves;
    public MoveClass[] midMoves;
    public MoveClass[] backMoves;

    EnemyClass(string _name, AffinityDictionary _aff, StatDictionary _stats, MoveClass[] _front, MoveClass[] _mid, MoveClass[] _back)
    {
        enemyName = _name;
        affinities = _aff;
        stats = _stats;
        frontMoves = _front;
        midMoves = _mid;
        backMoves = _back;
    }
}
