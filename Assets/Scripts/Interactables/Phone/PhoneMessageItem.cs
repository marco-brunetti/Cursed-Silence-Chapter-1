using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessageItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _messageTimeText;
    [SerializeField] private TextMeshProUGUI _dateText; //used for date indicator in conversation

    public void SetMessage(string message, string time, string date = "", bool isDeletedMessage = false)
    {
        if (_messageText != null) _messageText.text = message;
        if (_messageTimeText != null) _messageTimeText.text = time;
        if(_dateText != null) _dateText.text = date;

        if (isDeletedMessage)
        {
            _messageText.fontStyle = FontStyles.Italic;
        }
    }
}
