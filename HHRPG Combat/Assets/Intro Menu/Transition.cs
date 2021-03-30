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
        if(dif == "Battle 1")
        {
            difString.text = "Some things you should know before we get started.\nThis is a prototype. Do not expect AAA polish.\nAll interaction with this prototype is done with the mouse.\nFor this first battle, you can only attack, guard, and do rhythm (AKA magic) attacks.\nI'll teach you more later. Have fun!";
        }
        if(dif == "Battle 2")
        {
            difString.text = "Congrats, you did it! Now, for some of the fun parts.\nEnemies from here on out will appear in a grid, with a front, mid, and back row. Front row enemies typically do stronger single attacks, Mid does weaker group attacks, and Back does support.\nBy EQ-ing, you can drag and drop enemies around the battlefield, changing the move they do.\nOnly your party leader can EQ.\nAlso, EQing costs EP, or Magic, and doubles every time you use it in a single turn.\nHave fun!";
        }
        if(dif == "Battle 3")
        {
            difString.text = "Wow, look at you go! Alright, time for the last mechanics.\nYou may have noticed a line appearing between your party member and an enemy in the front row when you get a crit.\nThis is called a sidechain. Breaking these sidechains does damage, but instantly ends your entire party phase.\nYou can, however, harmonic pass between party members, and potentially build more sidechains.\nThe more sidechains you break at the same time, the exponentially more damage you do.\nYou do not have to break the chains if you don't want to. But, where's the fun in that?\nSpeaking of which, have fun!";
        }
        if(dif == "Battle 4")
        {
            difString.text = "Congratulations, you've learned everything I have to teach you.\nHere's a nice, big, chunky fight for you to go after.\nHave fun, and thanks for playing!";
        }
        if(dif == "Battle 5")
        {
            difString.text = "Thank you!";
            dif = "Intro Menu";
        }
        if(dif == "Battle 6")
        {
            Debug.Log("You shouldn't be here.");
        }
    }

    public void StartPrototype()
    {
        SceneManager.LoadSceneAsync(dif);
    }
}
