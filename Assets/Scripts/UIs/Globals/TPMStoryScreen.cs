using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;

public class TPMStoryScreen : DTNView
{
    public Text LevelText;
    public Text DescriptionText;
    public List<Sprite> StorySprites;
    public Action OnEndStoryAndStartGame;
    public SimpleScrollSnap SimpleScrollSnap;
    public Image StoryState;
    public Button NextBtn;
    public GameObject LevelIntro;

    public override void Init()
    {
        NextBtn.onClick.AddListener(() =>
        {
            NextButtonOnclick();
        });
    }

    public void NextButtonOnclick()
    {
        Debug.Log(SimpleScrollSnap.CurrentPanel);
        if(SimpleScrollSnap.CurrentPanel == StorySprites.Count - 1)
        {
            EndStory();
            Hide();
        }
    }

    public void SetUpStory(int level, string description, List<Sprite> storySprites)
    {
        LevelText.text = "Level " + level;
        DescriptionText.text = "" + description;
        GenerateState(storySprites);
        StartCoroutine(IEnumLevelIntro());
    }

    IEnumerator IEnumLevelIntro()
    {
        yield return new WaitForSeconds(2f);
        if (StorySprites.Count <= 0)
        {
            EndStory();
            Hide();
        }
        else
        {
            LevelIntro.SetActive(false);
        }
    }

    public void GenerateState(List<Sprite> storySprites)
    {
        StorySprites = storySprites;
        if (StorySprites.Count > 0)
        {
            StoryState.sprite = StorySprites[0];
            for (int i = 1; i < StorySprites.Count; i++)
            {
                Image image = Instantiate(StoryState, StoryState.transform.parent);
                image.sprite = StorySprites[i];
            }
        }
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void EndStory()
    {
        OnEndStoryAndStartGame();
    }
}
