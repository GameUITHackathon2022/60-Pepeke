using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectTypeEnum
{
    organic,
    recyableInorganic,
    nonRecyableInorganic,

    leftOver    


}

public enum NameTypeEnum
{
    Apple,
    Banana,
    Battery,
    Bulb,
    Can,
    Candy1,
    Candy2,
    Carrot,
    ChickenLeg,
    Cigarette,
    FishBone,
    Magazine,
    Med1,
    Med2,
    Milk,
    Pen,
    ToiletPaper,
    Vase,
    WaterBottle,
    Wine
}

[System.Serializable]
public class PDTObjectInfo
{
    public NameTypeEnum Name;
    public ObjectTypeEnum ObjectType;
    public string Description;
    public Sprite Icon;
    public PGPObject Object;
    public string Address;
}

[System.Serializable]
[CreateAssetMenuAttribute(fileName = "ObjectDataSystem", menuName = "Data/Scriptable/Object Data System")]
public class PDTObjectDataSystem : ScriptableObject
{
    public List<PDTObjectInfo> ObjectInfoList;
    private Hashtable ObjectInfoHashTable = new Hashtable();

    private void GenerateHashTable()
    {
        for (int i = 0; i < ObjectInfoList.Count; i++)
        {
            ObjectInfoHashTable.Add(ObjectInfoList[i].Name, ObjectInfoList[i]);
        }
    }

    public PDTObjectInfo GetObjectInfo(NameTypeEnum nameObject)
    {
        if (ObjectInfoHashTable.Count <= 0)
            GenerateHashTable();

        return ObjectInfoHashTable[nameObject] as PDTObjectInfo;
    }
}
