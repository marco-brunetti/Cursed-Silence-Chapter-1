using Game.General;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class SetupFinalScene : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_santaFood")]
        [Header("Food for characters")]
        [SerializeField] private GameObject santaFood;
        [FormerlySerializedAs("_guardianFood")] [SerializeField] private GameObject guardianFood;

        [FormerlySerializedAs("_santa")]
        [Header("Setup characters")]
        [SerializeField] private GameObject santa;
        [FormerlySerializedAs("_guardian")] [SerializeField] private GameObject guardian;
        [FormerlySerializedAs("_emily")] [SerializeField] private GameObject emily;

        [FormerlySerializedAs("_kitchenSwitch")]
        [Header("Setup lights")]
        [SerializeField] private LightSwitch kitchenSwitch;
        [FormerlySerializedAs("_livingRoomLamp")] [SerializeField] private LightSwitch livingRoomLamp;

        [FormerlySerializedAs("_deactivateColliders")]
        [Header("Deactivate interactables")]
        [SerializeField] private Collider[] deactivateColliders;

        [FormerlySerializedAs("_lightExplodeClip")]
        [Header("Setup sounds")]
        [SerializeField] private AudioClip lightExplodeClip;
        [FormerlySerializedAs("_lightAudioSource")] [SerializeField] private AudioSource lightAudioSource;

        [FormerlySerializedAs("_tv")]
        [Header("Setup TV")]
        [SerializeField] private TV tv;

        [FormerlySerializedAs("_santaBehaviours")]
        [Header("Generic Behaviours")]
        [SerializeField] private GameObject[] santaBehaviours;
        [FormerlySerializedAs("_guardianBehaviours")] [SerializeField] private GameObject[] guardianBehaviours;

        [FormerlySerializedAs("_subtitles")]
        [Header("Subtitles (order: 079, 077, 080, 078)")]
        [SerializeField] private GameObject[] subtitles;


        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            // PlayerInventory inventory = PlayerController.Instance.Inventory;
            // GameObject currentInteractable = PlayerController.Instance.InteractableInSight.gameObject;
            //
            // if(inventory.SelectedItem() == null)
            // {
            //     RunGenericBehaviours(currentInteractable);
            // }
            // else if (inventory.SelectedItem() != null)
            // {
            //     if(inventory.SelectedItem() != _santaFood && inventory.SelectedItem() != _guardianFood)
            //     {
            //         RunGenericBehaviours(currentInteractable);
            //     }
            //
            //     if (_santaFood != null && inventory.SelectedItem() == _santaFood)
            //     {
            //         if(currentInteractable == _santa)
            //         {
            //             ConsumeFood(_santaFood, 0, inventory);
            //         }
            //         else if(currentInteractable == _guardian)
            //         {
            //             _subtitles[1].GetComponent<IBehaviour>().Behaviour(true, false);
            //         }
            //     }
            //
            //     if (_guardianFood != null && inventory.SelectedItem() == _guardianFood)
            //     {
            //         if(currentInteractable == _guardian)
            //         {
            //             ConsumeFood(_guardianFood, 2, inventory);
            //         }
            //         else if(currentInteractable == _santa)
            //         {
            //             _subtitles[3].GetComponent<IBehaviour>().Behaviour(true, false);
            //         }
            //     }
            // }
            //
            // if (_santaFood == null && _guardianFood == null)
            // {
            //     FinalSetup();
            // }
        }

        private void ConsumeFood(GameObject food, int subtitleIndex, PlayerInventory inventory)
        {

            PlayerData playerData = PlayerController.Instance.PlayerData;

            PlayerController.Instance.InspectablesSource.pitch = 0.9f;
            PlayerController.Instance.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

            subtitles[subtitleIndex].GetComponent<IBehaviour>().Behaviour(true, false);
            //inventory.Remove(food);

            if(food == santaFood)
            {
                santaFood = null;
            }
            else if(food == guardianFood)
            {
                guardianFood = null;
            }
        }

        private void RunGenericBehaviours(GameObject currentInteractable)
        {
            if(currentInteractable == santa)
            {
                for (int i = 0; i < santaBehaviours.Length; i++)
                {
                    santaBehaviours[i].GetComponent<IBehaviour>().Behaviour(true, false);
                }
            }
            else if(currentInteractable == guardian)
            {
                for (int i = 0; i < guardianBehaviours.Length; i++)
                {
                    guardianBehaviours[i].GetComponent<IBehaviour>().Behaviour(true, false);
                }
            }
        }

        private void FinalSetup()
        {
            //set to the opposite of desired behaviour for now
            kitchenSwitch.isOn = true; 
            livingRoomLamp.isOn = false;

            kitchenSwitch.Behaviour(false, false);
            livingRoomLamp.Behaviour(false, false);

            lightAudioSource.PlayOneShot(lightExplodeClip, 0.5f * GameController.Instance.GlobalVolume);

            santa.gameObject.SetActive(false);
            guardian.gameObject.SetActive(false);
            emily.gameObject.SetActive(true);
            kitchenSwitch.gameObject.SetActive(false);

            tv.videoDuration = 1000;

            for(int i = 0; i < deactivateColliders.Length; i++)
            {
                deactivateColliders[i].enabled = false;
            }
        }

        public bool IsInteractable()
        {
            return true;
        }

        public bool IsInspectable()
        {
            throw new System.NotImplementedException();
        }
    }
}
