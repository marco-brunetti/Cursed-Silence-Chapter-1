using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessageApp : MonoBehaviour
{
    [SerializeField] private GameObject _conversationContainer;
    [SerializeField] private TextMeshProUGUI _conversationName;
    [SerializeField] private Transform _messageContainer;
    [SerializeField] private GameObject _recievedMessagePrefab;
    [SerializeField] private GameObject _sentMessagePrefab;
    [SerializeField] private GameObject _datePrefab;
    [SerializeField] private GameObject _blockedContactPrefab;

    [SerializeField] private GameObject _conversationButtonsContainer;
    [SerializeField] private Button _angieConversationButton;
    [SerializeField] private Button _insuranceConversationButton;
    [SerializeField] private Button _phoneConversationButton;
    [SerializeField] private Button _autoFixConversationButton;
    [SerializeField] private Button _hairHavenConversationButton;
    [SerializeField] private Button _drLeeConversationButton;
    [SerializeField] private Button _miaConversationButton;
    [SerializeField] private Button _principalConversationButton;
    [SerializeField] private Button _husbandConversationButton;


    [SerializeField] private TextAsset _conversationTextJson;
    private List<Message> _messages = new();

    private void Start()
    {
        var messageString = _conversationTextJson.ToString();
        _messages = JsonConvert.DeserializeObject<List<Message>>(messageString);

        _angieConversationButton.onClick.AddListener(() => SetConversation(MessageSender.Angie, "Angie"));
        _insuranceConversationButton.onClick.AddListener(() => SetConversation(MessageSender.HealthFirstInsurance, "HealthFirst Insurance"));
        _phoneConversationButton.onClick.AddListener(() => SetConversation(MessageSender.HorizonSupport, "Horizon Support"));
        _autoFixConversationButton.onClick.AddListener(() => SetConversation(MessageSender.AutoFix, "AutoFix"));
        _hairHavenConversationButton.onClick.AddListener(() => SetConversation(MessageSender.HairHaven, "Hair Haven"));
        _drLeeConversationButton.onClick.AddListener(() => SetConversation(MessageSender.DrLee, "Dr. Lee"));
        _miaConversationButton.onClick.AddListener(() => SetConversation(MessageSender.Mia, "Mia"));
        _principalConversationButton.onClick.AddListener(() => SetConversation(MessageSender.Principal, "Principal Smith"));
        _husbandConversationButton.onClick.AddListener(() => {
            SetConversation(MessageSender.Husband, "Muffin");
            var blockedContactObject = Instantiate(_blockedContactPrefab, _messageContainer);
            blockedContactObject.SetActive(true);
        });
    }

    public void SetConversation(MessageSender sender, string contactName)
    {
        _conversationButtonsContainer.SetActive(false);
        _conversationContainer.SetActive(true);
        _conversationName.text = contactName;

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
        else
        {
            var isDeleted = message.messageText.Equals("You deleted this message") || message.messageText.Equals("This message has been deleted") ? true : false;

            messageItem.SetMessage(message.messageText, message.time, isDeletedMessage: isDeleted);
        }

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
    HealthFirstInsurance,
    HorizonSupport,
    AutoFix,
    HairHaven,
    DrLee,
    Husband,
    Mia,
    Principal
}