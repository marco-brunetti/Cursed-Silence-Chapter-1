using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class MultiSwapObjects : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_requiredObjects")]
        [Header("If objects required, keep order of activate and deactivate object the same")]
        [SerializeField] private List<InventoryItem> requiredObjects;
        [FormerlySerializedAs("_activateObjects")] [SerializeField] private List<GameObject> activateObjects;
        [FormerlySerializedAs("_deactivateObjects")] [SerializeField] private List<GameObject> deactivateObjects;

        [FormerlySerializedAs("_onInteraction")] [SerializeField] private bool onInteraction = true;
        [FormerlySerializedAs("_onInspection")] [SerializeField] private bool onInspection;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(onInteraction && isInteracting)
            {
                SwapObjects();
            }
            else if(onInspection && isInspecting)
            {
                SwapObjects();
            }
        }

        public bool IsInspectable()
        {
            return onInspection;
        }

        public bool IsInteractable()
        {
            return onInteraction;
        }

        private void SwapObjects()
        {
            if(requiredObjects.Count > 0 )
            {
                var playerController = PlayerController.Instance;

                for(int i = 0; i < requiredObjects.Count; i++)
                {
                    if(playerController.Inventory.Contains(requiredObjects[i], removeItem:true, destroyItem:true))
                    {
                        PlayerData playerData = PlayerController.Instance.PlayerData;
                        playerController.InspectablesSource.pitch = 0.9f;
                        //playerController.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

                        requiredObjects.Remove(requiredObjects[i]);


                        if (i < activateObjects.Count)
                        {
                            if (activateObjects[i])
                            {
                                activateObjects[i].SetActive(true);
                                activateObjects.Remove(activateObjects[i]);
                            }
                        }

                        if (i < deactivateObjects.Count)
                        {
                            if (deactivateObjects[i])
                            {
                                deactivateObjects[i].SetActive(false);
                                deactivateObjects.Remove(deactivateObjects[i]);
                            }
                        }
                    }
                }
            }

            if(requiredObjects.Count <= 0)
            {
                if(activateObjects.Count > 0)
                {
                    for(int i = 0; i < activateObjects.Count; i++)
                    {
                        if (activateObjects[i]) activateObjects[i].SetActive(true);
                    }
                }

                if(deactivateObjects.Count > 0)
                {
                    for(int i = 0; i < deactivateObjects.Count; i++)
                    {
                        if (deactivateObjects[i]) deactivateObjects[i].SetActive(false);
                    }
                }

            }
        }
    }
}
