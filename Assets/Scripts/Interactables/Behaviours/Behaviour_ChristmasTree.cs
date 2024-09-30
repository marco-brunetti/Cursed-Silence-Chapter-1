using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_ChristmasTree : MonoBehaviour, IBehaviour
{
    [SerializeField] private GameObject _requiredGift;

    [SerializeField] private Behaviour_DisplaySubtitles _ifGiftSubtitles;
    [SerializeField] private Behaviour_DisplaySubtitles _ifNotGiftSubtitles;

    [SerializeField] private GameObject _bodyGiftBox;
    [SerializeField] private GameObject _emilyGiftBox;
    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        var playerController = PlayerController.Instance;
        PlayerData playerData = PlayerController.Instance.PlayerData;

        if(playerController.Inventory.SelectedItem() != null && playerController.Inventory.SelectedItem() == _requiredGift)
        {
            playerController.Inventory.Remove(_requiredGift);

            playerController.InspectablesSource.pitch = 0.9f;
            playerController.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

            _requiredGift = null;

            _ifGiftSubtitles.Behaviour(isInteracting, isInspecting);
            _bodyGiftBox.gameObject.SetActive(true);
            _emilyGiftBox.gameObject.SetActive(true);

            gameObject.GetComponent<Collider>().enabled = false;
        }
        else
        {
            _ifNotGiftSubtitles.Behaviour(isInteracting, isInspecting);
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
