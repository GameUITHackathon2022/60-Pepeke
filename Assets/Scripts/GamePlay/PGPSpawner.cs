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
    public List<PGPObject> BlablaObjectList;
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
    bool used = false;
    public void FanBtnOclick()
    {
        if (used)
            return;
        foreach (var item in BlablaObjectList)
        {
            if(item!= null)
             item.GetComponent<Rigidbody>().AddForce(Vector3.up * UnityEngine.Random.Range(20, 50),ForceMode.VelocityChange);
        }
        StartCoroutine(IEnumWait());
    }
    IEnumerator IEnumWait()
    {
        used = true;
        yield return new WaitForSeconds(2.5f);
        used = false;
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
                BlablaObjectList.Add(spawnedObject);
                if (spawnedObject.Type == LevelType)
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
