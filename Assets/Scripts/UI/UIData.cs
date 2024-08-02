using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIData : MonoBehaviour
{
    [field: SerializeField, Header("Canvases")] public GameObject PlayerCanvas { get; private set; }
    [field: SerializeField, Header("SceneChanger")] public GameObject SceneChangeCanvas { get; private set; }
    [field: SerializeField] public Image SceneChangeBakground { get; private set; }
    [field: SerializeField] public GameObject SceneChangeTip { get; private set; }
    [field: SerializeField, Header("Prompts")] public GameObject InteractPrompt {  get; private set; }
    [field: SerializeField] public GameObject InspectPrompt { get; private set; }
    [field: SerializeField] public GameObject ReturnPrompt { get; private set; }
    [field: SerializeField] public GameObject InteractOrReturnPrompt { get; private set; }
    [field: SerializeField, Header("Interactables")] public GameObject CenterPoint { get; private set; }
    [field: SerializeField, Header("Readables")] public GameObject ReadableUI { get; private set; }
    [field:SerializeField] public TextMeshProUGUI ReadablesText { get; private set; }
    [field: SerializeField, Header("Subtitles")] public TextMeshProUGUI SubtitlesText { get; private set; }
    [field: SerializeField, Header("UITexts")] public TextMeshProUGUI[] UITexts { get; private set; }
}
