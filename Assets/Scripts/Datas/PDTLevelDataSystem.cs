using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PGPObjectSpawnVolume
{
    public PDTObjectInfo ObjectToSpawn;
    public int Quantity;
}

[System.Serializable]
public class PDTLevelInfo
{
    public int Level;
    public ObjectTypeEnum LevelType;
    public Sprite Background;
    public string Desciption;
    public float Time;
    public List<Sprite> StoryList;
    public List<PGPObjectSpawnVolume> ObjectList;
}

[System.Serializable]
[CreateAssetMenuAttribute(fileName = "LevelDataSystem", menuName = "Data/Scriptable/Level Data System")]
public class PDTLevelDataSystem : ScriptableObject
{
    public List<PDTLevelInfo> LevelInfoList;

    private Hashtable LevelInfoHashTable = new Hashtable();

    private void GenerateHashTable()
    {
        for (int i = 0; i < LevelInfoList.Count; i++)
        {
            LevelInfoHashTable.Add(LevelInfoList[i].Level, LevelInfoList[i]);
        }
    }

    public PDTLevelInfo GetObjectInfo(int level)
    {
        if (LevelInfoHashTable.Count <= 0)
            GenerateHashTable();

        return LevelInfoHashTable[level] as PDTLevelInfo;
    }
}
