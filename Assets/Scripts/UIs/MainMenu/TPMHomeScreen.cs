using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TPMHomeScreen : DTNView
{
    public Button PlayButton;
    public Text LevelText;
    public override void Init()
    {
        int LevelIndex = PlayerPrefs.GetInt("Level", 1);
        LevelText.text = LevelIndex + "";
        PlayButton.onClick.RemoveAllListeners();
        PlayButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("GamePlay");
        });
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
