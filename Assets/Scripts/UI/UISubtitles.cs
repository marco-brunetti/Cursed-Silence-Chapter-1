using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;

public class UISubtitles : MonoBehaviour
{
    private SubtitleTextList _subtitleTextList;
    private TextAsset _subtitleAsset;

    private void Start()
    {
        ParseJSON();
    }

    public void Display(int subtitleIndex, float subtitleDuration)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayMessage(_subtitleTextList.SubtitleTexts[subtitleIndex].Text, subtitleDuration));
    }

    private IEnumerator DisplayMessage(string message, float duration)
    {
        message = message.Remove(0, 6); //removes index number included in the json files

        UIManager.Instance.UIData.SubtitlesText.text = message;
        yield return new WaitForSeconds(duration);
        UIManager.Instance.UIData.SubtitlesText.text = "";
    }

    public void ParseText()
    {
        ParseJSON();
    }

    private void ParseJSON()
    {
        if (GameController.Instance != null && GameController.Instance.SelectedLanguage == Language.English)
            _subtitleAsset = Resources.Load<TextAsset>("JSON/Subtitles/en-US");
        else if (GameController.Instance != null && GameController.Instance.SelectedLanguage == Language.Spanish)
            _subtitleAsset = Resources.Load<TextAsset>("JSON/Subtitles/es-LA");
        else
            _subtitleAsset = Resources.Load<TextAsset>("JSON/Subtitles/en-US");

        string json = _subtitleAsset.text;
        _subtitleTextList = JsonConvert.DeserializeObject<SubtitleTextList>(json);
    }
}

[Serializable]
public class SubtitleTextList
{
    public SubtitleTexts[] SubtitleTexts;
}

[Serializable]
public class SubtitleTexts
{
    public string Text;
}