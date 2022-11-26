using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TPMGamePlayLoseScreen : TPMBaseScreen
{
    public Button HomeBtn;
    public Button RetryBtn;
    public Action OnRetryLevel;
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
        HomeBtn.onClick.AddListener(()=> {
            SceneManager.LoadScene("MainMenu");
        });
        RetryBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("GamePlay");
        });
    }

    public override void RemoveAllOnlickButtons()
    {
        HomeBtn.onClick.RemoveAllListeners();
        RetryBtn.onClick.RemoveAllListeners();
    }

}
