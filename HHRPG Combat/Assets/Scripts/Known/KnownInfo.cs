using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class KnownInfo : MonoBehaviour
{
    public long totalEnemies;
    public KnownInfoDataType[] knownInfo;

    private void Start()
    {
        InitializeJSON();
    }

    public KnownInfoDataType getFromJSON(string name)
    {
        KnownInfoDataType temp = null;
        for(int x = 0; x < knownInfo.Length; x++)
        {
            if (knownInfo[x].name == "?")
            {
                temp = knownInfo[x];
                knownInfo[x].name = name;
                break;
            } //We looked through all known instances
            if (knownInfo[x].name == name)
            {
                temp = knownInfo[x];
                break;
            }
        }
        return temp;
    }

    public void writeToJSON(KnownInfoDataType thingToWrite)
    {
        int currentPos = 0;
        while(currentPos < knownInfo.Length) //find the right position in the JSON file
        {
            if (knownInfo[currentPos].name == "?") { break; } //We looked through all known instances
            if (knownInfo[currentPos].name == thingToWrite.name) { break; }
            currentPos++;
        }
        knownInfo[currentPos] = thingToWrite;
    }

    public void InitializeJSON()
    {
        AssetDatabase.ImportAsset("Assets/Resources/JSON/KnownAffinities.json");
        var known = (TextAsset)AssetDatabase.LoadMainAssetAtPath("Assets/Resources/JSON/KnownAffinities.json");
        knownInfo = JsonHelper.FromJson<KnownInfoDataType>(known.text);
    }

    public void UpdateJSON()
    {
        var writing = JsonHelper.ToJson<KnownInfoDataType>(knownInfo, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/JSON/KnownAffinities.json", writing);
    }

    public void InitializeJSONNew()
    {
        AssetDatabase.ImportAsset("Assets/Resources/JSON/KnownAffinities.json");
        var known = (TextAsset)AssetDatabase.LoadMainAssetAtPath("Assets/Resources/JSON/KnownAffinities.json");
        knownInfo = JsonHelper.FromJson<KnownInfoDataType>(known.text);

        for(int x = 0; x < knownInfo.Length; x++)
        {
            knownInfo[x].name = "?";
            knownInfo[x].affinities[0].affName = "Physical";
            knownInfo[x].affinities[1].affName = "Drum";
            knownInfo[x].affinities[2].affName = "Bass";
            knownInfo[x].affinities[3].affName = "Guitar";
            knownInfo[x].affinities[4].affName = "Piano";
            knownInfo[x].affinities[5].affName = "Violin";
            knownInfo[x].affinities[6].affName = "Woodwind";
            knownInfo[x].affinities[7].affName = "Synth";
            knownInfo[x].affinities[8].affName = "Noise";
            knownInfo[x].affinities[9].affName = "Lyrics";

            for(int y = 0; y < 10; y++)
            {
                knownInfo[x].affinities[y].affValue = "?";
            }
        }

        var writing = JsonHelper.ToJson<KnownInfoDataType>(knownInfo, true);
        System.IO.File.WriteAllText(Application.dataPath + "/Resources/JSON/KnownAffinities.json", writing);

    }
}

[System.Serializable]
public class KnownInfoDataType
{
    [System.Serializable]
    public struct Affinity
    {
        public string affName;
        public string affValue;
    }

    public string name;
    public Affinity[] affinities = new Affinity[10];
}
