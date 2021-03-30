using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuStuff : MonoBehaviour
{
    public string dif = "Medium";
    public Text difString;

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

    public void StartPrototype(string nameOfScene)
    {
        KnownInfo.InitializeJSONNew();
        BattleClass.tutorial = 1;
        BattleClass.difficulty = dif;
        Transition.dif = "Battle 1";
        SceneManager.LoadSceneAsync(nameOfScene);
    }
}
