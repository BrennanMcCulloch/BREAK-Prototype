using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class MoveListInserter : MonoBehaviour
{
    //INITIALIZE TO ALL POSSIBLE MOVES
    public MoveClass[] allMoves;
    public TextAsset moveList;

    private void Update()
    {
#if UNITY_EDITOR
        AssetDatabase.ImportAsset("Assets/Resources/JSON/AllMovesList.json");
        moveList = (TextAsset) AssetDatabase.LoadMainAssetAtPath("Assets/Resources/JSON/AllMovesList.json");
        allMoves = JsonHelper.FromJson<MoveClass>(moveList.text);
        string fixitstring = JsonHelper.ToJson<MoveClass>(allMoves, true);

        Debug.Log(moveList);

        for (int x = 0; x < allMoves.Length; x++)
        {
            bool isItThereAlready = false;

            foreach (var child in this.transform.GetComponentsInChildren(typeof(MoveClassWrapper), true)) {
                if (child.name == allMoves[x].GetName()) { isItThereAlready = true; break; }
            }

            if(isItThereAlready == true) { continue; }

            GameObject temp = new GameObject();
            temp.transform.parent = this.gameObject.transform;
            temp.name = allMoves[x].GetName();

            MoveClass current = allMoves[x];

            System.Type myType = System.Type.GetType("MoveClass");

            
            MoveClassWrapper thing = temp.AddComponent<MoveClassWrapper>();
            thing.MoveClass.SetName(current.GetName());
            thing.MoveClass.SetMoveType(current.GetMoveType());
            thing.MoveClass.SetEffective(current.GetEffective());
            thing.MoveClass.SetGroup(current.GetGroup());
            thing.MoveClass.SetCost(current.GetCost());
            thing.MoveClass.SetFriend(current.GetFriend());
            thing.MoveClass.SetHarm(current.GetHarm());
            
        }
#endif
    }
    
}
