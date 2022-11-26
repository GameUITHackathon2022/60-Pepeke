using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Achievement
{


    [System.Serializable]
    public class PDTAchievementInfo
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public string Address;
    }

    [System.Serializable]
    [CreateAssetMenuAttribute(fileName = "AchievementDataSystem", menuName = "Data/Scriptable/Achievement Data System")]
    public class PDTAchievementDataSystem : ScriptableObject
    {
        public List<PDTAchievementInfo> AchievementInfoList;
        private Hashtable AchievementInfoHashTable = new Hashtable();

        private void GenerateHashTable()
        {
            for (int i = 0; i < AchievementInfoList.Count; i++)
            {
                AchievementInfoHashTable.Add(AchievementInfoList[i].Name, AchievementInfoList[i]);
            }
        }

        public PDTAchievementInfo GetAchievementInfo(string nameAchievement)
        {
            if (AchievementInfoHashTable.Count <= 0)
                GenerateHashTable();

            return AchievementInfoHashTable[nameAchievement] as PDTAchievementInfo;
        }
    }

}