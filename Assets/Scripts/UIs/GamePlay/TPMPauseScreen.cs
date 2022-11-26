using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TPMPauseScreen : TPMBaseScreen
{
    public Button ExitBtn;
    public Button ResumeBtn;
    public Button HomeBtn;
    public Button RetryBtn;

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

    public void FreezeTime()
    {
        Time.timeScale = 0f;
    }

    public void NormalTime()
    {
        Time.timeScale = 1f;
    }

    public override void AddAllOnlickButtons()
    {
        ExitBtn.onClick.AddListener(() => {
            NormalTime();
            Hide();
        });

        ResumeBtn.onClick.AddListener(() => {
            NormalTime();
            Hide();
        });

        HomeBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("MainMenu");
        });

        RetryBtn.onClick.AddListener(() => {
            SceneManager.LoadScene("GamePlay");
        });
    }

    public override void RemoveAllOnlickButtons()
    {
        ExitBtn.onClick.RemoveAllListeners();
        ResumeBtn.onClick.RemoveAllListeners();
        HomeBtn.onClick.RemoveAllListeners();
        RetryBtn.onClick.RemoveAllListeners();
    }
}
