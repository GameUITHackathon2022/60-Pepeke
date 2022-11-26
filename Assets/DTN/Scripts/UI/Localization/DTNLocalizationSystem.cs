using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTNLocalizationSystem
{
    [System.Serializable]
    public class DTNLanguageText
    {
        public string key;
        public string translated;
        public DTNLanguageText(string _key, string _tra)
        {
            key = _key;
            translated = _tra;
        }
    }

    [System.Serializable]
    public class DTNLocalLanguage
    {
        public List<DTNLanguageText> items;
    }
    private static DTNLocalLanguage localLanguage = null;
    private static Dictionary<string, string> localLanguageHasktable = new Dictionary<string, string>();


    public static string GetText(string key)
    {
        Debug.Log("_" + key);

        if (localLanguage == null)
        {
            TextAsset jsonFile = (TextAsset)Resources.Load("language/" + Language() + "", typeof(TextAsset));// duoi phai la duoi .json

            if (jsonFile!= null)
            {
                string jsonString = jsonFile.text;// Resources.Load("language/" + Language() + ".json");
                localLanguage = JsonUtility.FromJson<DTNLocalLanguage>(jsonString);

                if (localLanguage != null)
                {
                    localLanguageHasktable = new Dictionary<string, string>();
                    foreach (DTNLanguageText ltext in localLanguage.items)
                    {
                        localLanguageHasktable.Add(ltext.key, ltext.translated);
                    }
                }
                
            }
            
        }

        if (localLanguageHasktable.ContainsKey(key)){
            return localLanguageHasktable[key];
        }

        return key;
    }

    public static string Language()
    {
        if (Application.systemLanguage == SystemLanguage.Japanese)
        {
            return "Japanese";
        }else if (Application.systemLanguage == SystemLanguage.Korean)
        {
            return "Korean";
        }
        else if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
        {
            return "Chinese";
        }
        else if (Application.systemLanguage == SystemLanguage.Spanish)
        {
            return "Spanish";
        }
        else if (Application.systemLanguage == SystemLanguage.Portuguese)
        {
            return "Portuguese";
        }
        else if (Application.systemLanguage == SystemLanguage.Vietnamese)
        {
            return "Vietnam";
        }

        return "English";
    }

}
