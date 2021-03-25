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

    public IEnumerator holdPosition()
    {
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.transform.position.z);
        this.transform.position = mousePos;
        yield return null;
        if(Input.GetMouseButton(0) == false) //if you've let go of the mouse
        {
            yield break;
        }
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
