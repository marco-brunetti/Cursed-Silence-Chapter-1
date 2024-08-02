using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILanguage : MonoBehaviour
{
    private UITextList _uiTextList;
    private TextAsset _uiAsset;

    public void SetLanguage(Language selectedLanguage)
    {
        ParseJSON(selectedLanguage);
        SetUILanguage();
    }

    private void ParseJSON(Language selectedLanguange)
    {
        if (selectedLanguange == Language.English)
            _uiAsset = Resources.Load<TextAsset>("JSON/UI/en-US");
        else if (selectedLanguange == Language.Spanish)
            _uiAsset = Resources.Load<TextAsset>("JSON/UI/es-LA");
        else
            _uiAsset = Resources.Load<TextAsset>("JSON/UI/en-US");

        string json = _uiAsset.text;
        _uiTextList = JsonConvert.DeserializeObject<UITextList>(json);
    }

    private void SetUILanguage()
    {
        for(int i = 0; i < UIManager.Instance.UIData.UITexts.Length; i++)
        {
            UIManager.Instance.UIData.UITexts[i].text = _uiTextList.UITexts[i].Text;
        }
    }
}

[Serializable]
public class UITextList
{
    public UITexts[] UITexts;
}

[Serializable]
public class UITexts
{
    public string Text;
}
