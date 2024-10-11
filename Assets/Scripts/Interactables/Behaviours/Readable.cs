using Game.General;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class Readable : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_message")] [SerializeField, TextArea(20, 20)] private string message;
        [FormerlySerializedAs("_spanishMessage")] [SerializeField, TextArea(20, 20)] private string spanishMessage;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            UIReadable readable = UIManager.Instance.Readable;
            GameController gameController = GameController.Instance;

            if(isInspecting)
            {
                string message;

                if(gameController.SelectedLanguage == Language.English)
                {
                    message = this.message;
                }
                else if(gameController.SelectedLanguage == Language.Spanish)
                {
                    message = spanishMessage;
                }
                else
                {
                    message = this.message;
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
