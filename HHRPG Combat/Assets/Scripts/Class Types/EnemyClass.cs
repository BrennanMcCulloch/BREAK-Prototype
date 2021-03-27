using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyClass : MonoBehaviour
{
    public string enemyName;
    public GameObject UIStuff;
    public int currentHealth;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public ModifierDictionary buffDebuff;
    public GameObject[] frontMoves;
    public GameObject[] midMoves;
    public GameObject[] backMoves;
    public KnownInfo known; //keep current known affinities
    KnownInfoDataType knownThing;
    private Canvas HPEP;
    private int max;
    private Slider healthBar;
    private Slider epBar;

    private void Start()
    {
        UIStuff.SetActive(false);
        HPEP = this.gameObject.GetComponentInChildren<Canvas>();
        stats.TryGetValue("HP", out max);

        foreach (Slider slide in HPEP.gameObject.GetComponentsInChildren<Slider>())
        {
            if(slide.name == "HP")
            {
                slide.maxValue = max;
                healthBar = slide;
                healthBar.gameObject.SetActive(false);
            }
            else if(slide.name == "EP")
            {
                epBar = slide;
                epBar.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }
        if(currentHealth < max)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.value = currentHealth;
        }
        
    }

    private void UpdateUI()
    {
        foreach (Text theThing in UIStuff.GetComponentsInChildren<Text>())
        {
            if(theThing.name == "Name")
            {
                Text child = theThing.GetComponent<Text>();
                child.text = enemyName;
            }
            else
            {
                if(theThing.name == "HP")
                {
                    string display = "HP: " + currentHealth;
                    Text child = theThing.GetComponent<Text>();
                    child.text = display;
                }
                if(theThing.name == "EP")
                {
                    Text child = theThing.GetComponent<Text>();
                    child.text = "";
                }
                for(int x = 0; x < knownThing.affinities.Length; x++)
                {
                    if(theThing.name == knownThing.affinities[x].affName)
                    {
                        string display = knownThing.affinities[x].affName + ": " + knownThing.affinities[x].affValue;
                        Text child = theThing.GetComponent<Text>();
                        child.text = display;
                        break;
                    }
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        knownThing = known.getFromJSON(enemyName);
        UpdateUI();
        UIStuff.SetActive(true);
    }

    private void OnMouseExit()
    {
        UIStuff.SetActive(false);
    }
}
