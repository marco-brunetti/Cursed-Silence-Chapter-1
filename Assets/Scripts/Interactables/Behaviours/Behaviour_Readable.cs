using SnowHorse.Utils;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_Readable : MonoBehaviour, IBehaviour
    {
        [SerializeField, TextArea(20, 20)] private string _message;
        [SerializeField, TextArea(20, 20)] private string _spanishMessage;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            UIReadable readable = UIManager.Instance.Readable;
            GameController gameController = GameController.Instance;

            if(isInspecting)
            {
                string message;

                if(gameController.SelectedLanguage == Language.English)
                {
                    message = _message;
                }
                else if(gameController.SelectedLanguage == Language.Spanish)
                {
                    message = _spanishMessage;
                }
                else
                {
                    message = _message;
                }

                if(message == "" || message == null)
                {
                    WarningTool.Print("Please check readable text!", gameObject);
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
