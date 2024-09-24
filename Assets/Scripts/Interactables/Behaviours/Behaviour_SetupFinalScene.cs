using Player;
using UnityEngine;

public class Behaviour_SetupFinalScene : MonoBehaviour, IBehaviour
{
    [Header("Food for characters")]
    [SerializeField] private GameObject _santaFood;
    [SerializeField] private GameObject _guardianFood;

    [Header("Setup characters")]
    [SerializeField] private GameObject _santa;
    [SerializeField] private GameObject _guardian;
    [SerializeField] private GameObject _emily;

    [Header("Setup lights")]
    [SerializeField] private Behaviour_LightSwitch _kitchenSwitch;
    [SerializeField] private Behaviour_LightSwitch _livingRoomLamp;

    [Header("Deactivate interactables")]
    [SerializeField] private Collider[] _deactivateColliders;

    [Header("Setup sounds")]
    [SerializeField] private AudioClip _lightExplodeClip;
    [SerializeField] private AudioSource _lightAudioSource;

    [Header("Setup TV")]
    [SerializeField] private Behaviour_TV _tv;

    [Header("Generic Behaviours")]
    [SerializeField] private GameObject[] _santaBehaviours;
    [SerializeField] private GameObject[] _guardianBehaviours;

    [Header("Subtitles (order: 079, 077, 080, 078)")]
    [SerializeField] private GameObject[] _subtitles;


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

        _subtitles[subtitleIndex].GetComponent<IBehaviour>().Behaviour(true, false);
        //inventory.Remove(food);

        if(food == _santaFood)
        {
            _santaFood = null;
        }
        else if(food == _guardianFood)
        {
            _guardianFood = null;
        }
    }

    private void RunGenericBehaviours(GameObject currentInteractable)
    {
        if(currentInteractable == _santa)
        {
            for (int i = 0; i < _santaBehaviours.Length; i++)
            {
                _santaBehaviours[i].GetComponent<IBehaviour>().Behaviour(true, false);
            }
        }
        else if(currentInteractable == _guardian)
        {
            for (int i = 0; i < _guardianBehaviours.Length; i++)
            {
                _guardianBehaviours[i].GetComponent<IBehaviour>().Behaviour(true, false);
            }
        }
    }

    private void FinalSetup()
    {
        //set to the opposite of desired behaviour for now
        _kitchenSwitch.isOn = true; 
        _livingRoomLamp.isOn = false;

        _kitchenSwitch.Behaviour(false, false);
        _livingRoomLamp.Behaviour(false, false);

        _lightAudioSource.PlayOneShot(_lightExplodeClip, 0.5f * GameController.Instance.GlobalVolume);

        _santa.gameObject.SetActive(false);
        _guardian.gameObject.SetActive(false);
        _emily.gameObject.SetActive(true);
        _kitchenSwitch.gameObject.SetActive(false);

        _tv.VideoDuration = 1000;

        for(int i = 0; i < _deactivateColliders.Length; i++)
        {
            _deactivateColliders[i].enabled = false;
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
