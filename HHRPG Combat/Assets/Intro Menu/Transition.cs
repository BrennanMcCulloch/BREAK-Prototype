using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public static string dif = "";
    public Text difString;

    private void Start()
    {
        difString = this.transform.Find("Description").GetComponent<Text>();
        if(dif == "Base Battle Scene")
        {
            difString.text = "Some things you should know before we get started.\nThis is a prototype. Do not expect AAA polish.\nAll interaction with this prototype is done with the mouse.\nFor this first battle, you can only attack, guard, and do rhythm (AKA magic) attacks.\nI'll teach you more later. Have fun!";
        }
        if(dif == "Final Boss")
        {
            difString.text = "Ya did it, Jonny!! You became one with the cosssmossss";
        }
    }

    public void StartPrototype()
    {
        SceneManager.LoadSceneAsync(dif);
    }
}
