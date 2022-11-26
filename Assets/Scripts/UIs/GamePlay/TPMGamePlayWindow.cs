using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPMGamePlayWindow : DTNWindow
{
    public PGPController Controller;
    public TPMGamePlayTopBar GamePlayTopBar;
    public TPMGamePlayBottomBar GamePlayBottomBar;
    public TPMStoryScreen StoryScreen;
    public TPMCollectObjectScreen CollectObjectScreen;
    public int StarCount;

    public TPMGamePlayWindow()
    {

    }

    public override void Init()
    {
        base.Init();
        SetActions();
    }

    public void SetUpGame(PDTLevelInfo levelInfo)
    {
        GamePlayTopBar.SetUpGames(levelInfo.Level, levelInfo.Time);
        StoryScreen.SetUpStory(levelInfo.Level, levelInfo.Desciption, levelInfo.StoryList);
    }

    public void StartGame()
    {
        CollectObjectScreen.Show();
        Controller.SetUpInitState();
    }

    public void SetActions()
    {
        Controller.WinGameAction = WinGame;
        Controller.LoseGameAction = LoseGame;
        GamePlayTopBar.OnEndOfCountdown = LoseGame;
        StoryScreen.OnEndStoryAndStartGame = StartGame;
    }

    public void WinGame()
    {
        GamePlayTopBar.StopCountdown();
        TPMGamePlayWinScreen winView = ShowSubView(typeof(TPMGamePlayWinScreen)) as TPMGamePlayWinScreen;
      //  winView.OnNextLevel = Controller.;
    }

    public void LoseGame()
    {
        GamePlayTopBar.StopCountdown();
        TPMGamePlayLoseScreen loseView = ShowSubView(typeof(TPMGamePlayLoseScreen)) as TPMGamePlayLoseScreen;
       // loseView.OnRetryLevel = Controller.LoseGameAction;
    }
}
