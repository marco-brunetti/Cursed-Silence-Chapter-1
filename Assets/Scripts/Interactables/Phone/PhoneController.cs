using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneController : MonoBehaviour
{
    [SerializeField] private GameObject _messageAppContainer;

    [SerializeField] private TextMeshProUGUI _timeText;

    [Header("App buttons")]
    [SerializeField] private Button _messageAppButton;

    private void OnEnable()
    {
        //add camera to canvas
    }


    // Start is called before the first frame update
    void Start()
    {
        //_messageAppButton.onClick.AddListener(() => ActivateMessageApp());


    }

    // Update is called once per frame
    void Update()
    {
        _timeText.text = string.Format("{0:hh:mm tt}", DateTime.Now);
    }

    private void ActivateMessageApp()
    {

    }

    private void MessageAppControl()
    {
        if (_messageAppContainer != null && _messageAppContainer.activeInHierarchy)
        {

        }
    }
}
