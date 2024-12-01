using System;
using System.Collections.Generic;
using Interactables;
using Interactables.Behaviours;
using Player;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject centerPoint;

        private static EventHandler<bool> activateCenterPoint;
        
        private void Awake()
        {
            InteractableCamLook.showUICursor += ActiveCursor;
            UIManager.activateCenterPoint += ActivateCenterPoint;
        }

        public static void ActiveCursor(object sender, bool enable)
        {
            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
            activateCenterPoint?.Invoke(sender, !enable);
        }

        private void ActivateCenterPoint(object sender, bool activate)
        {
            centerPoint.SetActive(activate);
        }

        /*[SerializeField] private UIPrompts _prompts;

    public UIData UIData;
    public UICanvasControl CanvasControl;
    public UIReadable Readable;
    public UISubtitles Subtitles;
    public UILanguage Language;

    public bool HideUI;

    private bool _pause;

    public bool SetPause(bool isPaused) => _pause = isPaused;
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else Instance = this; 
    }

    void Update()
    {
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
            /*if (PlayerController.Instance != null && PlayerController.Instance.IsInspecting)
            {
                UIData.CenterPoint.SetActive(false);
            }
            else
            {
                UIData.CenterPoint.SetActive(true);
            }*/
        /*}
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
    }*/
    }
}