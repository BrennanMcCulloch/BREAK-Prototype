using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuStuff : MonoBehaviour
{
    public string dif = "Medium";
    public Text difString;
    public Text LevelString;
    public int tutorial = 1;

    private void Start()
    {
        difString = this.transform.Find("Difficulty").GetComponent<Text>();
        difString.text = "Difficulty: " + dif;
    }

    public void ChangeDifficulty(string type)
    {
        dif = type;
        difString.text = "Difficulty: " + dif;
    }

    public void ChangeStartingLevel()
    {
        tutorial ++;
        
        if (tutorial > 4)
        {
            tutorial = 1;
        }

        LevelString.text = "Level " + tutorial.ToString();
    }

    public void StartPrototype(string nameOfScene)
    {
        KnownInfo.InitializeJSONNew();
        BattleClass.tutorial = tutorial;
        BattleClass.difficulty = dif;
        Transition.dif = "Battle " + tutorial.ToString();
        SceneManager.LoadSceneAsync(nameOfScene);
    }
}
