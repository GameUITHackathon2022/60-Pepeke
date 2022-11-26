using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGPSlotList : MonoBehaviour
{
    public List<PGPSlot> SlotList;
    public bool IsFull()
    {
        foreach (PGPSlot item in SlotList)
        {
            if (item.IsEmpty)
                return false;
        }
        return true;
    }

    public Transform GetEmptySlot()
    {
        if (IsFull())
        {
            return null;
        }
        else
        {
            foreach (var item in SlotList)
            {
                if (item.IsEmpty)
                {
                    return item.transform;
                }
            }
        }
        return null;
    }
}
