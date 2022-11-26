using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPMMainMenuTopBar : DTNView
{
    public Button SettingBtn;

    public override void Init()
    {
        SetButtonOnclick();
        Debug.Log("Init");
    }

    public void SetButtonOnclick()
    {
        SettingBtn.onClick.RemoveAllListeners();
        SettingBtn.onClick.AddListener(() => {
            SettingBtnOnclick();
        });
    }

    public void SettingBtnOnclick()
    {
        parentView.ShowSubView(typeof(TPMSettingScreen));
    }
}
