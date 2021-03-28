using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PartyMemberClass : MonoBehaviour
{
    public string memberName;
    public int currentHealth;
    public int currentEP;
    public GameObject UIStuff;
    public bool currentlyChained = false;
    public bool currentlyGuarding = false;
    public ModifierDictionary buffDebuff;
    [SerializeField] public AffinityDictionary affinities;
    [SerializeField] public StatDictionary stats;
    public GameObject[] moves; //MAKE THIS SIZE 8
    public GameObject harmonic;
    private string[] names = new string[10];
    private Canvas HPEP;
    private int max;
    private int maxE;
    private Slider healthBar;
    private Slider epBar;

    private void Start()
    {
        names[0] = "Physical";
        names[1] = "Drum";
        names[2] = "Bass";
        names[3] = "Guitar";
        names[4] = "Piano";
        names[5] = "Violin";
        names[6] = "Woodwind";
        names[7] = "Synth";
        names[8] = "Noise";
        names[9] = "Lyrics";

        HPEP = this.gameObject.GetComponentInChildren<Canvas>();
        stats.TryGetValue("HP", out max);
        stats.TryGetValue("EP", out maxE);

        foreach (Slider slide in HPEP.gameObject.GetComponentsInChildren<Slider>())
        {
            if (slide.name == "HP")
            {
                slide.maxValue = max;
                healthBar = slide;
                healthBar.gameObject.SetActive(true);
            }
            else if (slide.name == "EP")
            {
                slide.maxValue = maxE;
                epBar = slide;
                epBar.gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        healthBar.value = currentHealth;
        epBar.value = currentEP;

        if (currentHealth <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void UpdateUI()
    {
        foreach (Text theThing in UIStuff.GetComponentsInChildren(typeof(Text), true))
        {
            if (theThing.name == "Name")
            {
                Text child = theThing.GetComponent<Text>();
                child.text = memberName;
            }
            else
            {
                if (theThing.name == "HP")
                {
                    string display = "HP: " + currentHealth;
                    Text child = theThing.GetComponent<Text>();
                    child.text = display;
                }
                if (theThing.name == "EP")
                {
                    string display = "EP: " + currentEP;
                    Text child = theThing.GetComponent<Text>();
                    child.text = display;
                    theThing.gameObject.SetActive(true);
                }
                for (int x = 0; x < affinities.Count; x++)
                {
                    string result;
                    affinities.TryGetValue(names[x], out result);

                    if (theThing.name == names[x])
                    {
                        string display = names[x] + ": " + result;
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
        UpdateUI();
        foreach (Text textbox in UIStuff.GetComponentsInChildren(typeof(Text), true))
        {
            textbox.gameObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        foreach (Text textbox in UIStuff.GetComponentsInChildren(typeof(Text), true))
        {
            textbox.gameObject.SetActive(false);
        }
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

//ON BUILD, ENABLE
//#if UNITY_EDITOR
//#endif
