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
        if(isInspecting)
        {
            StartCoroutine(ActivateCanvas(true));

            PlayerController.Instance.InventoryCamera.SetActive(false);
            PlayerController.Instance.InventoryCamera.SetActive(true);

            _isInteracting = true;
            GameController.Instance.ShowCursor = true;
            UIManager.Instance.HideUI = true;
        }
        if(!isInspecting && !isInspecting)
        {
            StartCoroutine(ActivateCanvas(false));

            PlayerController.Instance.FreezePlayerMovement = false;
            PlayerController.Instance.FreezePlayerRotation = false;

            _isInteracting = false;

            GameController.Instance.ShowCursor = false;
            UIManager.Instance.HideUI = false;
        }
    }

    private IEnumerator ActivateCanvas(bool enable)
    {
        yield return new WaitForSecondsRealtime(0.4f);
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
