using SnowHorse.Utils;
using System.Collections;
using UnityEngine;

public class UICanvasControl : MonoBehaviour
{
    [SerializeField] private float _delay = 4f;
    [SerializeField] private float _transparencyDuration = 2f;

    private bool _isSceneChanged;
    private float _currentTransparency;

    public GameObject _blackMask { get; private set; } 

    public void ManageCanvases(UIData uIData, bool pause)
    {
        if(pause)
        {
            uIData.PlayerCanvas.SetActive(false);
        }
        else
        {
            uIData.PlayerCanvas.SetActive(true);
        }
    }

    public void OnSceneChanged(bool showTip)
    {
        UIData uiData = UIManager.Instance.UIData;

        uiData.SceneChangeCanvas.SetActive(true);
        uiData.SceneChangeBakground.color = Color.black;

        if (showTip)
        {
            uiData.SceneChangeTip.SetActive(true);
        }
        else
        {
            uiData.SceneChangeTip.SetActive(false);
        }

        StartCoroutine(ManageDelay());
    }

    private IEnumerator ManageDelay()
    {
        yield return new WaitForSecondsRealtime(_delay);
        UIManager.Instance.UIData.SceneChangeTip.SetActive(false);
        _isSceneChanged = true;
    }

    private void Update()
    {
        if(_isSceneChanged)
        {
            float transparency = Interpolation.Linear(_transparencyDuration, ref _currentTransparency);

            UIManager.Instance.UIData.SceneChangeBakground.color = new Color(0, 0, 0, 1 - transparency);

            if (transparency == 1)
            {
                UIManager.Instance.UIData.SceneChangeCanvas.SetActive(false);
                _currentTransparency = 0;
                transparency = 0;
                _isSceneChanged = false;
            }
        }
    }
}
