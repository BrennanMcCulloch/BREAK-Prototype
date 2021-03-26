using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainClass : MonoBehaviour
{
    public PartyMemberClass chainHolder;
    public EnemyClass chainVictim;
    private LineRenderer line;

    // Start is called before the first frame update
    void Awake()
    {
        line = this.gameObject.AddComponent<LineRenderer>();
    }

    public void Initialize()
    {
        Debug.Log(line);
        line.SetPosition(0, chainHolder.transform.position);
        line.SetPosition(1, chainVictim.transform.position);
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.startColor = Color.cyan;
        line.endColor = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        //Make the chain wiggly maybe?
    }
}
