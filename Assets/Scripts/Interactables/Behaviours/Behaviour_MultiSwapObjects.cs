using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_MultiSwapObjects : MonoBehaviour, IBehaviour
    {
        [Header("If objects required, keep order of activate and deactivate object the same")]
        [SerializeField] private List<InventoryItem> _requiredObjects;
        [SerializeField] private List<GameObject> _activateObjects;
        [SerializeField] private List<GameObject> _deactivateObjects;

        [SerializeField] private bool _onInteraction = true;
        [SerializeField] private bool _onInspection;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(_onInteraction && isInteracting)
            {
                SwapObjects();
            }
            else if(_onInspection && isInspecting)
            {
                SwapObjects();
            }
        }

        public bool IsInspectable()
        {
            return _onInspection;
        }

        public bool IsInteractable()
        {
            return _onInteraction;
        }

        private void SwapObjects()
        {
            if(_requiredObjects.Count > 0 )
            {
                var playerController = PlayerController.Instance;

                for(int i = 0; i < _requiredObjects.Count; i++)
                {
                    if(playerController.Inventory.Contains(_requiredObjects[i], removeItem:true, destroyItem:true))
                    {
                        PlayerData playerData = PlayerController.Instance.PlayerData;
                        playerController.InspectablesSource.pitch = 0.9f;
                        playerController.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

                        _requiredObjects.Remove(_requiredObjects[i]);


                        if (i < _activateObjects.Count)
                        {
                            if (_activateObjects[i])
                            {
                                _activateObjects[i].SetActive(true);
                                _activateObjects.Remove(_activateObjects[i]);
                            }
                        }

                        if (i < _deactivateObjects.Count)
                        {
                            if (_deactivateObjects[i])
                            {
                                _deactivateObjects[i].SetActive(false);
                                _deactivateObjects.Remove(_deactivateObjects[i]);
                            }
                        }
                    }
                }
            }

            if(_requiredObjects.Count <= 0)
            {
                if(_activateObjects.Count > 0)
                {
                    for(int i = 0; i < _activateObjects.Count; i++)
                    {
                        if (_activateObjects[i]) _activateObjects[i].SetActive(true);
                    }
                }

                if(_deactivateObjects.Count > 0)
                {
                    for(int i = 0; i < _deactivateObjects.Count; i++)
                    {
                        if (_deactivateObjects[i]) _deactivateObjects[i].SetActive(false);
                    }
                }

            }
        }
    }
}
