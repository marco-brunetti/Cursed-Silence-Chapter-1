using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneController : MonoBehaviour, IBehaviour
{
    [SerializeField] private GameObject _canvas;

    [SerializeField] private TextMeshProUGUI _timeText;

    [Header("App buttons")]
    [SerializeField] private Button _messageAppButton;


    private bool _isInteracting;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        var playerController = PlayerController.Instance;

        if(isInspecting)
        {
            StartCoroutine(ActivateCanvas(true));

            playerController.InventoryCamera.SetActive(false);
            playerController.InventoryCamera.SetActive(true);

            _isInteracting = true;

            UIManager.Instance.HideUI = true;
        }
        if(!isInspecting && !isInspecting)
        {
            StartCoroutine(ActivateCanvas(false));

            playerController.FreezePlayerMovement = false;
            playerController.FreezePlayerRotation = false;

            _isInteracting = false;

            GameController.Instance.ShowCursor = false;
            UIManager.Instance.HideUI = false;
        }
    }

    private IEnumerator ActivateCanvas(bool enable)
    {
        yield return new WaitForSecondsRealtime(0.4f);
        GameController.Instance.ShowCursor = enable;
        _canvas.SetActive(enable);
    }

    public bool IsInteractable()
    {
        return false;
    }

    public bool IsInspectable()
    {
        return true;
    }


    private void OnEnable()
    {
        //add camera to canvas
    }

    void Update()
    {
        if(_isInteracting) _timeText.text = string.Format("{0:hh:mm tt}", DateTime.Now);
    }
}
