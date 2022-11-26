using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGPSlot : MonoBehaviour
{
    public PGPObject ContainingObject;
    public bool IsEmpty => ContainingObject == null;
    //private void Awake()
    //{
    //    isEmpty = true;
    //}


}
