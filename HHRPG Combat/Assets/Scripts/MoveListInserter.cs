using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class MoveListInserter : MonoBehaviour
{
    //INITIALIZE TO ALL POSSIBLE MOVES yes
    public MoveClass[] allMoves;
    public TextAsset moveList;

    private void Update()
    {
#if UNITY_EDITOR
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        AssetDatabase.ImportAsset("Assets/Resources/JSON/AllMovesList.json");
        moveList = (TextAsset) AssetDatabase.LoadMainAssetAtPath("Assets/Resources/JSON/AllMovesList.json");
        allMoves = JsonHelper.FromJson<MoveClass>(moveList.text);
        MoveClass fixit = new MoveClass("Bufuuu", "Rhythm", false, 4, false, false);
        allMoves[0] = fixit;
        string fixitstring = JsonHelper.ToJson<MoveClass>(allMoves, true);
        Debug.Log(fixitstring);

        for (int x = 0; x < allMoves.Length; x++)
        {
            GameObject temp = new GameObject();
            temp.transform.parent = this.gameObject.transform;
            temp.name = allMoves[x].GetName();

            MoveClass current = allMoves[x];
            MoveClass thing = temp.AddComponent<MoveClass>();
            thing.SetName(current.GetName());
            thing.SetMoveType(current.GetMoveType());
            thing.SetGroup(current.GetGroup());
            thing.SetCost(current.GetCost());
            thing.SetFriend(current.GetFriend());
            thing.SetHarm(current.GetHarm());
        }
#endif
    }
    
}
