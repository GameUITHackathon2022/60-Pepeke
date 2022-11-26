using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PUIHomeScreen : DTNView
{
    public Button PlayBtn;
    public Button ChooseLevelBtn;

    public override void Init()
    {
        SetButtonOnclick();
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
        PlayBtn.onClick.RemoveAllListeners();
        ChooseLevelBtn.onClick.RemoveAllListeners();

        PlayBtn.onClick.AddListener(() =>
        {
            PlayBtnOnclick();
        });

        ChooseLevelBtn.onClick.AddListener(() =>
        {
            ChooseLevelBtnOnclick();
        });
    }

    public void PlayBtnOnclick()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void ChooseLevelBtnOnclick()
    {

    }
}
