using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TPMGamePlayWinScreen : TPMBaseScreen
{
    public Button HomeBtn;
    public Button NextBtn;
    public Action OnNextLevel;

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
        HomeBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });
        NextBtn.onClick.AddListener(() => {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);
            OnNextLevel();
        });
    }

    public override void RemoveAllOnlickButtons()
    {
        HomeBtn.onClick.RemoveAllListeners();
        NextBtn.onClick.RemoveAllListeners();
    }

}
