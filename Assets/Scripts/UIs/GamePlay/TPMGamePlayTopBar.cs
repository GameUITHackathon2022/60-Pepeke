using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TPMGamePlayTopBar : DTNView
{
    public Text LevelText;
    public Text StarText;
    public Text CountdownText;
    public Button PauseButton;
    public Action OnEndOfCountdown;
    private Coroutine countdownCoroutine;

    public override void Init()
    {
        SetButtonOnclick();
    }

    public void SetButtonOnclick()
    {
        PauseButton.onClick.RemoveAllListeners();
        PauseButton.onClick.AddListener(() =>
        {
            parentView.ShowSubView(typeof(TPMPauseScreen));
        });
    }

    public void SetUpGames(int level, float time)
    {
        LevelText.text = "Level " + level;
        StartCountdown(time);
    }

    public void StartCountdown(float stopTime)
    {
        countdownCoroutine = StartCoroutine(IEnumCountdown(stopTime));
    }

    public void StopCountdown()
    {
       // StopCoroutine(countdownCoroutine);
        countdownCoroutine = null;
    }

    IEnumerator IEnumCountdown(float stopTime)
    {
        var deltaTime = stopTime - Time.time;

        while (deltaTime >= 0)
        {
            yield return null;
            deltaTime = stopTime - Time.time;
            var minutes = (int)(deltaTime / 60);
            var seconds = (int)(deltaTime % 60);
            CountdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        Debug.Log("End !!!!!!");
        if(OnEndOfCountdown != null)
            OnEndOfCountdown();
    }
}
