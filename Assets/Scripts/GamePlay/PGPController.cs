using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PGPController : MonoBehaviour
{
    public int Score;
    public float secondsToPlay = 60;
    public PGPGround Ground;

    public GameStateEnum GameState = GameStateEnum.Normal;

    public Action LoseGameAction;
    public Action WinGameAction;
    public TPMGamePlayWindow GamePlayWindow;
    public PDTLevelDataSystem LevelDataSystem;
    public int LevelIndex;
    private void Awake()
    {
        LevelIndex = PlayerPrefs.GetInt("Level", 1);
        GamePlayWindow.SetUpGame(LevelDataSystem.GetObjectInfo(LevelIndex));
    }

    bool isStart = false;

    private void Update()
    {
        if (!isStart)
            return;
        //Update State
        if(Ground.SlotOnGround.IsFull())
        {
            GameState = GameStateEnum.Lose;
        }
        else if(Ground.PGPSpawner.IsEmpty())
        {
            GameState = GameStateEnum.Win;
        }

        //Check State
        if(GameState != GameStateEnum.Normal)
        {
            if(GameState == GameStateEnum.Lose)
            {
                if(LoseGameAction != null)
                {
                    LoseGameAction();
                    LoseGameAction = null;
                }
                
            }
            else if (GameState == GameStateEnum.Win)
            {
                if (WinGameAction != null)
                {
                    WinGameAction();
                    WinGameAction = null;
                }
            }
        }
        else
        {
            
        }
    }

    public void SetUpInitState()
    {
        GameState = GameStateEnum.Normal;
        Score = 0;
        Ground.SetUpGround();
        isStart = true;
    }

    public void DecreaseTime()
    {
        if(secondsToPlay > 0)
        {
            secondsToPlay -= Time.deltaTime;
        }
        else
        {
            GameState = GameStateEnum.Lose;
        }
    }
}

public enum GameStateEnum
{
    Normal,
    Win,
    Lose
}