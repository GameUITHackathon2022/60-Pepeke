using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PUIWinScreen : DTNView
{
    public Button HomeBtn;
    public Button RetryBtn;
    public Button NextBtn;

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

    public void SetButtonOnclick()
    {
        HomeBtn.onClick.RemoveAllListeners();
        RetryBtn.onClick.RemoveAllListeners();

        HomeBtn.onClick.AddListener(() =>
        {
            HomeBtnOnclick();
        });

        RetryBtn.onClick.AddListener(() =>
        {
            RetryBtnOnclick();
        });
    }

    private void RetryBtnOnclick()
    {

    }

    private void HomeBtnOnclick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
