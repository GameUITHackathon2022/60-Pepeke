using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OutOfTime();

public class PGPCountdown : MonoBehaviour
{
    public Text countdownText;
    float stopTime;

    public event OutOfTime OnOutOfTime;
    void Start()
    {
        stopTime = Time.time;
    }
    
    // Update is called once per frame
    void Update()
    {
        var deltaTime = stopTime - Time.time;

        if (deltaTime < 0) {
            OutOfTimeEvent();
            return;
        }
        var minutes = (int)(deltaTime / 60);
        var seconds = (int)(deltaTime % 60);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);        
        
    }

    private void OutOfTimeEvent()
    {
        OnOutOfTime?.Invoke();
    }

    public void Reset(float seconds)
    {
        stopTime = Time.time + seconds;
    }
}
