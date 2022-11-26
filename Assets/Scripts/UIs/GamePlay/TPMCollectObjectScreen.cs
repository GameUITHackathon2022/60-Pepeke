using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TPMCollectObjectScreen : DTNView
{
    [Header("Card Update Info")]
    public PDTLevelDataSystem ObjectDataSystem;
    public GameObject ViewportContent;
    public GameObject CardPrefab;
    public override void Init()
    {
        var objectInfoList = ObjectDataSystem.GetObjectInfo(PlayerPrefs.GetInt("Level", 1)).ObjectList;

        for (int i = 0; i < objectInfoList.Count; i++)
        {
            if(objectInfoList[i].ObjectToSpawn.ObjectType == ObjectDataSystem.GetObjectInfo(PlayerPrefs.GetInt("Level", 1)).LevelType)
            {
                var card = Instantiate(CardPrefab, ViewportContent.transform);
                var cardIcon = card.transform.GetChild(0).GetComponent<Image>();
                var cardText = card.transform.GetChild(1).GetComponent<Text>();

                cardIcon.sprite = objectInfoList[i].ObjectToSpawn.Icon;
                cardText.text = objectInfoList[i].ObjectToSpawn.Name.ToString() + "";
            }
            
        }
        CardPrefab.SetActive(false);


    }

    public override void Show()
    {
        base.Show();
        StartCoroutine(IEnumInActive());
    }

    IEnumerator IEnumInActive()
    {
        yield return new WaitForSeconds(5f);
        Hide();
    }

    public override void Hide()
    {
        base.Hide();
    }
}

