using TMPro;
using UnityEngine;

public class PhoneMessageItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _messageTimeText;
    [SerializeField] private TextMeshProUGUI _dateText; //used for date indicator in conversation

    public void SetMessage(string message, string time, string date = "")
    {
        if (_messageText != null) _messageText.text = message;
        if (_messageTimeText != null) _messageTimeText.text = time;
        if(_dateText != null) _dateText.text = date;
    }
}
