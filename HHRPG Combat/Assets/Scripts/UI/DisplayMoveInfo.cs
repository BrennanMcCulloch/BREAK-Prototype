using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMoveInfo : MonoBehaviour
{
    public Text title;
    public Text description;
    private MoveClassWrapper move;

    // Start is called before the first frame update
    void Start()
    {
        title.text = null;
        description.text = null;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowDesc(GameObject buttonThatCalled)
    {
        GameObject newThing = GameObject.Find("EnemyInfo");
        newThing.gameObject.SetActive(true);
        DisplayMoveInfo disp = newThing.GetComponent<DisplayMoveInfo>();
        disp.title.gameObject.SetActive(true);
        disp.description.gameObject.SetActive(true);
        disp.title.text = buttonThatCalled.gameObject.GetComponent<MoveClassWrapper>().MoveClass.moveName;
        disp.description.text = buttonThatCalled.gameObject.GetComponent<MoveClassWrapper>().MoveClass.description + " Cost: " + buttonThatCalled.gameObject.GetComponent<MoveClassWrapper>().MoveClass.cost.ToString();
    }

    public void HideDesc()
    {
        GameObject newThing = GameObject.Find("EnemyInfo");
        newThing.gameObject.SetActive(true);
        DisplayMoveInfo disp = newThing.GetComponent<DisplayMoveInfo>();
        disp.title.text = null;
        disp.description.text = null;
        disp.title.gameObject.SetActive(false);
        disp.description.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
