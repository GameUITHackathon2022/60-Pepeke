using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PGPGround : MonoBehaviour
{
    //public List<PGPObject> OnSlotList;
    public PGPSlotList SlotOnGround;
    public PGPSpawner PGPSpawner;
    public PGPPlane Plane;

    public void SetUpGround()
    {
        PGPSpawner.SpawnObject(OnObjectSelected);
        /*foreach (var item in PGPSpawner.CurrentObjectList)
        {
            item.OnObjectSelectedCallBack = OnObjectSelected;
        }*/
    }

    public void CalculateScore()
    {

    }

    public void OnObjectSelected(Transform trans)
    {
        Transform emptySlot = SlotOnGround.GetEmptySlot();
        if (emptySlot == null) return;

        var tween = trans.DOMove(emptySlot.position, 0.5f, false);
        tween.onComplete += () =>
        {
            trans.GetComponent<Rigidbody>().isKinematic = true;
        };

        //Get the Object and Slot
        PGPObject curPGPObject = trans.gameObject.GetComponent<PGPObject>();
        PGPSlot curSlot = emptySlot.GetComponent<PGPSlot>();
        //Add this Object to CurrentList
        //OnSlotList.Add(curPGPObject);
        curSlot.ContainingObject = curPGPObject;
        curPGPObject.transform.parent = curSlot.transform;
        if(curPGPObject.Type == PGPSpawner.LevelType)
            PGPSpawner.CurrentObjectList.Remove(curPGPObject);
        
        //get 3 items (or less) by NameEnum
        var objectListByName = ListObjectByName(curPGPObject.Name);

        // if match 3 items -> effect -> destroy -> sort
        if (objectListByName != null)
        {
            if (objectListByName.Count > 2)
            {
                if(PGPSpawner.LevelType == curPGPObject.Type)
                {
                    SetNullObjectInSlotList(objectListByName);
                    StartCoroutine(DestroyFromList(objectListByName));
                    //Sort list
                    SortOnSlotList();
                }
            }
            else   //if not match -> set pos
            {
                //Block Object rigidbody 
                trans.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                //Assign this Object to this Slot
                curSlot.ContainingObject = curPGPObject;
            }
        }
    }

    public IEnumerator DestroyFromList(List<PGPObject> listThreeObject)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var item in listThreeObject)
        {
            Destroy(item.gameObject);
        }
    }

    public void SetNullObjectInSlotList(List<PGPObject> listObject)
    {
        foreach (var item in listObject)
        {
            var first = SlotOnGround.SlotList.First(i => i.ContainingObject == item);
            if (first)
            {
                first.ContainingObject = null;
            }
        }
    }

    private List<PGPObject> ListObjectByName(NameTypeEnum nameSearch)
    {
        List<PGPObject> tempList = new List<PGPObject>();
        foreach (var item in SlotOnGround.SlotList)
        {
            if (item.ContainingObject?.Name == nameSearch)
            {
                tempList.Add(item.ContainingObject);
            }
        }
        return tempList;
    }

    public void SortOnSlotList()
    {
        var tempSlotList = SlotOnGround.SlotList;
        for (int i = 0; i < tempSlotList.Count; i++)
        {
            if(tempSlotList[i].ContainingObject == null)
            {
                for(int j = i + 1; j < tempSlotList.Count; j++)
                {
                    if (tempSlotList[j].ContainingObject != null)
                    {
                        tempSlotList[j].ContainingObject.transform.DOMove(tempSlotList[i].transform.position, 0.1f, false);
                        tempSlotList[j].ContainingObject.transform.parent = tempSlotList[i].transform; ;
                        tempSlotList[i].ContainingObject = tempSlotList[j].ContainingObject;
                        tempSlotList[j].ContainingObject = null;
                        break;
                    }
                }
            }
        }
    }
}
