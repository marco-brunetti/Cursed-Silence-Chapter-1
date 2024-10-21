using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReadable : MonoBehaviour
{
    private UIData _uiData;
    private void Start()
    {
        _uiData = GetComponent<UIData>();
        _uiData.ReadableUI.SetActive(false);
    }

    public void Display(string message)
    {
        if(message == null)
        {
            _uiData.ReadableUI.SetActive(false);
            _uiData.ReturnPrompt.SetActive(false); //move this to prompts Controller
        }
        else
        {
            _uiData.ReadableUI.SetActive(true);
            _uiData.ReadablesText.text = message;
        }
    }
}
