using Game.General;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class ChristmasTree : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_requiredGift")] [SerializeField] private InventoryItem requiredGift;

        [FormerlySerializedAs("_ifGiftSubtitles")] [SerializeField] private BehaviourDisplaySubtitles ifGiftSubtitles;
        [FormerlySerializedAs("_ifNotGiftSubtitles")] [SerializeField] private BehaviourDisplaySubtitles ifNotGiftSubtitles;

        [FormerlySerializedAs("_bodyGiftBox")] [SerializeField] private GameObject bodyGiftBox;
        [FormerlySerializedAs("_emilyGiftBox")] [SerializeField] private GameObject emilyGiftBox;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            var playerController = PlayerController.Instance;
            PlayerData playerData = PlayerController.Instance.PlayerData;

            if(playerController.Inventory.Contains(requiredGift, removeItem: true, destroyItem: true))
            {
                playerController.InspectablesSource.pitch = 0.9f;
                playerController.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

                requiredGift = null;

                ifGiftSubtitles.Behaviour(isInteracting, isInspecting);
                bodyGiftBox.gameObject.SetActive(true);
                emilyGiftBox.gameObject.SetActive(true);

                gameObject.GetComponent<Collider>().enabled = false;
            }
            else
            {
                ifNotGiftSubtitles.Behaviour(isInteracting, isInspecting);
            }
        }

        public bool IsInspectable()
        {
            return false;
        }

        public bool IsInteractable()
        {
            return true;
        }
    }
}
