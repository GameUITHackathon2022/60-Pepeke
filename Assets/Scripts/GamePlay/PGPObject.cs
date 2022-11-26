using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PGPObject : MonoBehaviour
{
    public bool IsSelected = false;
    public NameTypeEnum Name;
    public ObjectTypeEnum Type;

    public Action<Transform> OnObjectSelectedCallBack;

    public void OnObjectSelected()
    {
        PlaySound();
        OnObjectSelectedCallBack(this.transform);
    }
    public void PlaySound()
    {
        string tempSoundName;
        
        if(Name == NameTypeEnum.ToiletPaper || Name == NameTypeEnum.Magazine)
        {
            tempSoundName = "Paper";
        }
        else if(Name == NameTypeEnum.Can || Name == NameTypeEnum.Battery)
        {
            tempSoundName = "Metal";
        }
        else if(Name == NameTypeEnum.Vase || Name == NameTypeEnum.Bulb || Name == NameTypeEnum.Wine)
        {
            tempSoundName = "Glass";
        }
        else
        {
            tempSoundName = "Rest";
        }

        PGPSoundManagement.Instance.Play(tempSoundName);

    }
}

