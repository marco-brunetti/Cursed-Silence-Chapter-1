using Player;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    
    [SerializeField] private UIPrompts _prompts;

    public UIData UIData;
    public UICanvasControl CanvasControl;
    public UIReadable Readable;
    public UISubtitles Subtitles;
    public UILanguage Language;

    public bool HideUI;

    private bool _pause;
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else Instance = this; 
    }

    void Update()
    {
        if(GameController.Instance != null)
        {
            _pause = GameController.Instance.Pause;
        }

        CenterPointControl();
        ManagePrompts();
        ManageCanvases();
    }

    public void ActivateDarkMask(bool setActive)
    {
        UIData.DarkMask.gameObject.SetActive(setActive);
    }

    private void CenterPointControl()
    {
        if(_pause == false)
        {
            if (PlayerController.Instance != null && PlayerController.Instance.Inspector.IsInspecting)
            {
                UIData.CenterPoint.SetActive(false);
            }
            else
            {
                UIData.CenterPoint.SetActive(true);
            }
        }
    }

    private void ManagePrompts()
    {
        if(_pause == false)
        {
            _prompts.ManagePrompts(UIData);
        }
    }

    private void ManageCanvases()
    {
        CanvasControl.ManageCanvases(UIData, _pause);
    }

    public void SetBlackboardButtons(UnityAction rotateAction, UnityAction applyAction, UnityAction cancelAction)
    {
        UIData.BlackboardRotateItemButton.onClick.AddListener(rotateAction);
        UIData.ApplyRotationButton.onClick.AddListener(applyAction);
        UIData.CancelRotationButton.onClick.AddListener(cancelAction);
    }

    public void ShowBlackboardImage(bool show = true, Sprite sprite = null, float zAngle = 0)
    {
        if(sprite) UIData.BlackboardImage.sprite = sprite;

        UIData.BlackboardImage.transform.localRotation = Quaternion.Euler(0, 0, zAngle);
        UIData.BlackboardUI.SetActive(show);
        GameController.Instance.ShowCursor = show;
    }

    public void ShowRotateItemButton(bool show)
    {
        if (show) UIData.BlackboardRotateImage.color = Color.white;
        else UIData.BlackboardRotateImage.color = new Color(0, 0, 0, 0);
    }
}