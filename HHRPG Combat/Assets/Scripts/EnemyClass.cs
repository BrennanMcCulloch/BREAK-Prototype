using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    public string enemyName;
    public int currentHealth;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public ModifierDictionary buffDebuff;
    public GameObject[] frontMoves;
    public GameObject[] midMoves;
    public GameObject[] backMoves;
}
