using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class Readable : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_message")] [SerializeField, TextArea(20, 20)] private string message;
        [FormerlySerializedAs("_spanishMessage")] [SerializeField, TextArea(20, 20)] private string spanishMessage;

        private string currentLanguage;

        public void SetCurrentLanguage(string language)=> currentLanguage = language;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            UIReadable readable = UIManager.Instance.Readable;
            if(isInspecting)
            {
                string message;

                if(currentLanguage == "english")
                {
                    message = this.message;
                }
                else if(currentLanguage == "spanish")
                {
                    message = spanishMessage;
                }
                else
                {
                    message = this.message;
                }

                if(message == "" || message == null)
                {
                    Debug.Log($"Please check readable text! gameObject: {gameObject}");
                }

                readable.Display(message);
            }
            else
            {
                readable.Display(null);
            }
        }

        public bool IsInspectable()
        {
            return true;
        }

        public bool IsInteractable()
        {
            return false;
        }
    }
}
