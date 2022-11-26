using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPMSettingScreen : TPMBaseScreen
{
    public Button ExitBtn;
    
    public override void Init()
    {

    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void AddAllOnlickButtons()
    {
        ExitBtn.onClick.AddListener(() => {
            Hide();
        });
    }

    public override void RemoveAllOnlickButtons()
    {
        ExitBtn.onClick.RemoveAllListeners();
    }
}
