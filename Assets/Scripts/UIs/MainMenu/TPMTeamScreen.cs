using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Achievement;
public class TPMTeamScreen : DTNView
{[Header("Achievement Update Info")]
    public PDTAchievementDataSystem AchievementDataSystem;
    public GameObject ViewportContent;
    public GameObject CardPrefab;
    public override void Init()
    {
        var objectInfoList = AchievementDataSystem.AchievementInfoList;



        for (int i = 0; i < objectInfoList.Count; i++)
        {
            var card = Instantiate(CardPrefab, ViewportContent.transform);
            var cardIcon = card.transform.GetChild(0).GetComponent<Image>();
            var cardText = card.transform.GetChild(1).GetComponent<Text>();

            cardIcon.sprite = objectInfoList[i].Icon;
            cardText.text = objectInfoList[i].Name + ": " + objectInfoList[i].Description;
        }
        CardPrefab.SetActive(false);
    }
}
