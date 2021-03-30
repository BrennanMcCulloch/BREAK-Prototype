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
            difString.text = "Congrats, you did it! Now, for some of the fun parts.\nEnemies from here on out will appear in a grid, with a front, mid, and back row. Front row enemies typically do stronger single attacks, Mid does weaker group attacks, and Back support.\n By EQ-ing, you can drag and drop enemies around the battlefield, changing the move they do.\nOnly your party leader can EQ. Have fun!";
        }
        if(dif == "Battle 3")
        {
            difString.text = "BOOBIES";
        }
    }

    public void StartPrototype()
    {
        //KnownInfo.InitializeJSON();
        SceneManager.LoadSceneAsync(dif);
    }
}
