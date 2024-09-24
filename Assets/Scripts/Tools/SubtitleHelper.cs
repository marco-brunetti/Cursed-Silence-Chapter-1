using Interactables.Behaviours;
using Newtonsoft.Json;
using UnityEngine;

[ExecuteInEditMode]
public class SubtitleHelper : MonoBehaviour
{
    [Header("Attach this script to a game object \nwith Behaviour_DisplaySubtitles script")]
    [Header("Disable and reenable script to refresh subtitles")]

    [SerializeField] private string[] currentSubtitles;

    private Behaviour_DisplaySubtitles _subtitleTrigger;
    private SubtitleTextList _subtitleTextList;
    private TextAsset _subtitleAsset;

    private void OnEnable()
    {
        if(_subtitleTrigger == null)
        {
            _subtitleTrigger = gameObject.GetComponent<Behaviour_DisplaySubtitles>();
        }

        if (_subtitleTrigger != null && _subtitleTrigger.SubtitleIndex.Length > 0)
        {
            currentSubtitles = new string[_subtitleTrigger.SubtitleIndex.Length];

            ParseJSON();

            for (int i = 0; i < currentSubtitles.Length; i++)
            {
                int subtitleIndex = _subtitleTrigger.SubtitleIndex[i];
                currentSubtitles[i] = _subtitleTextList.SubtitleTexts[subtitleIndex].Text;
            }
        }
    }

    private void ParseJSON()
    {
        _subtitleAsset = Resources.Load<TextAsset>("JSON/Subtitles/en-US");

        string json = _subtitleAsset.text;

        _subtitleTextList = JsonConvert.DeserializeObject<SubtitleTextList>(json); //JsonUtility.FromJson<SubtitleTextList>(json);
    }
}
