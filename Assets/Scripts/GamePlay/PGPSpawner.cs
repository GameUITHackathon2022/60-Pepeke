using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
public class PGPSpawner : MonoBehaviour
{
    public PDTLevelDataSystem LevelDataSystem;
    List<PGPObjectSpawnVolume> ObjectList;
    public List<PGPObject> CurrentObjectList;
    public Transform[] SpawnPositions;
    Vector3 SpawnDirection;
    public int LevelIndex;
    public ObjectTypeEnum LevelType;
    public Image Background; 

    private void Awake()
    {
        LevelIndex = PlayerPrefs.GetInt("Level", 1);

        var levelInfo = LevelDataSystem.GetObjectInfo(LevelIndex);
        if(levelInfo!=null)
        {
            ObjectList = levelInfo.ObjectList;
            LevelType = levelInfo.LevelType;
            Background.sprite = levelInfo.Background;
        }

    }

    public void SpawnObject(Action<Transform> onObjectSelectedCallBack)
    {
        for (int i = 0; i < ObjectList.Count; i++)
        {
            for(int j = 0; j < ObjectList[i].Quantity; j++)
            {

                SpawnDirection = (this.transform.position - SpawnPositions[UnityEngine.Random.Range(0,4)].position).normalized;
                PGPObject spawnedObject = Instantiate(ObjectList[i].ObjectToSpawn.Object, SpawnPositions[UnityEngine.Random.Range(0, 4)].position * UnityEngine.Random.Range(0.8f,1.2f),Quaternion.AngleAxis(0,SpawnDirection));
                spawnedObject.OnObjectSelectedCallBack = onObjectSelectedCallBack;
                spawnedObject.GetComponent<Rigidbody>().velocity = SpawnDirection;
                if(spawnedObject.Type == LevelType)
                {
                    CurrentObjectList.Add(spawnedObject);
                }
            }
        }
    }

    IEnumerator IEnumSpawObject()
    {
        yield return null;
    }

    public bool IsEmpty()
    {
        if (CurrentObjectList.Count <= 0)
            return true;
        return false;
    }
}
