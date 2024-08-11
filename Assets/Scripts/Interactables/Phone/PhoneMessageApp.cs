using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessageApp : MonoBehaviour
{
    [SerializeField] private GameObject _conversationContainer;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private GameObject _recievedMessagePrefab;
    [SerializeField] private GameObject _sentMessagePrefab;
    [SerializeField] private GameObject _datePrefab;

    [SerializeField] private GameObject _conversationButtonsContainer;
    [SerializeField] private Button _angieConversationButton;
    [SerializeField] private TextAsset _conversationTextJson;

    private List<Message> _messages = new();

    private void Start()
    {
        var messageString = _conversationTextJson.ToString();
        _messages = JsonConvert.DeserializeObject<List<Message>>(messageString);

        _angieConversationButton.onClick.AddListener(() => SetConversation(MessageSender.Angie));

        
    }

    public void SetConversation(MessageSender sender)
    {
        _conversationButtonsContainer.SetActive(false);
        _conversationContainer.SetActive(true);

        for(int i = 0; i < _messages.Count; i++)
        {
            if (_messages[i].conversation == sender)
            {
                if(i == 0 || _messages[i - 1].date != _messages[i].date)
                {
                    SetMessage(_messages[i], _datePrefab, isDate: true);
                }

                if (_messages[i].sender == sender)
                {
                    SetMessage(_messages[i], _recievedMessagePrefab);
                }
                else if (_messages[i].sender == MessageSender.Emily)
                {
                    SetMessage(_messages[i], _sentMessagePrefab);
                }
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_messageContainer.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        _messageContainer.gameObject.SetActive(false);
        _messageContainer.gameObject.SetActive(true);
    }

    private void SetMessage(Message message, GameObject messagePrefab, bool isDate = false)
    {
        PhoneMessageItem messageItem = Instantiate(messagePrefab, _messageContainer).GetComponent<PhoneMessageItem>();
        if(isDate) messageItem.SetMessage("", "", message.date);
        else messageItem.SetMessage(message.messageText, message.time);
        messageItem.gameObject.SetActive(true);
    }
}

public record Message()
{
    public MessageSender conversation;
    public MessageSender sender;
    public string messageText;
    public string time;
    public string date;
}

public enum MessageSender
{
    Emily,
    Angie,
}