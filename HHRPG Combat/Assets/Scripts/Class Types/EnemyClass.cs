using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyClass : MonoBehaviour
{
    public string enemyName;
    public GameObject UIStuff;
    public Material highlightMaterial;
    private Material def;
    public int currentHealth;
    private int lastHealth;
    public AffinityDictionary affinities;
    public StatDictionary stats;
    public ModifierDictionary buffDebuff;
    public GameObject[] frontMoves;
    public GameObject[] midMoves;
    public GameObject[] backMoves;
    KnownInfoDataType knownThing;
    private Canvas HPEP;
    private int max;
    private Slider healthBar;
    private Slider epBar;

    private void Start()
    {
        //UIStuff.SetActive(true);
        def = this.gameObject.GetComponent<Renderer>().material;
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
        lastHealth = currentHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            this.transform.parent.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
        if(currentHealth < max)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.value = currentHealth;
        }
        if(lastHealth != currentHealth)
        {
            this.gameObject.GetComponent<Animator>().SetTrigger("Hit");
            lastHealth = currentHealth;
        }
        
    }

    private void UpdateUI()
    {
        foreach (Text theThing in UIStuff.GetComponentsInChildren(typeof(Text), true))
        {
            if(theThing.name == "Name")
            {
                Text child = theThing.GetComponent<Text>();
                child.text = enemyName;
            }
            else
            {
                if(theThing.name == "Description")
                {
                    Text child = theThing.GetComponent<Text>();
                    child.text = "";
                }
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
        knownThing = KnownInfo.getFromJSON(enemyName);
        this.transform.gameObject.GetComponent<Renderer>().material = highlightMaterial;
        UpdateUI();
        foreach (Text textbox in UIStuff.GetComponentsInChildren(typeof(Text), true))
        {
            textbox.gameObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        this.transform.gameObject.GetComponent<Renderer>().material = def;
        foreach (Text textbox in UIStuff.GetComponentsInChildren(typeof(Text), true))
        {
            textbox.text = "";
            textbox.gameObject.SetActive(false);
        }
    }
}
