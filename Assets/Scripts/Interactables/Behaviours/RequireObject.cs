using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class RequireObject : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_requiredObject")] [SerializeField] private InventoryItem requiredObject;
        [FormerlySerializedAs("_onInteraction")] [SerializeField] private bool onInteraction = true;
        [FormerlySerializedAs("_onInspection")] [SerializeField] private bool onInspection;

        [FormerlySerializedAs("_successBehaviours")] [SerializeField] private List<GameObject> successBehaviours = new List<GameObject>();
        [FormerlySerializedAs("_failedBehaviours")] [SerializeField] private List<GameObject> failedBehaviours = new List<GameObject>();

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(PlayerController.Instance.Inventory.Contains(requiredObject, removeItem: true, destroyItem: false))
            {
                if (onInteraction && isInteracting)
                {
                    ManageSuccessBehaviours(isInteracting, false);
                }
                if (onInspection && isInspecting)
                {
                    ManageSuccessBehaviours(false, isInspecting);
                }
            }
            else if(!requiredObject)
            {
                if (onInteraction && isInteracting)
                {
                    ManageSuccessBehaviours(isInteracting, false);
                }
                if (onInspection && isInspecting)
                {
                    ManageSuccessBehaviours(false, isInspecting);
                }
            }
            else
            {
                if (onInteraction && isInteracting)
                {
                    ManageFailedBehaviours(isInteracting, false);
                }
                if (onInspection && isInspecting)
                {
                    ManageFailedBehaviours(false, isInspecting);
                }
            }
        }

        private void ManageSuccessBehaviours(bool isInteracting, bool isInspecting)
        {
            if (successBehaviours.Count > 0)
            {
                for (int i = 0; i < successBehaviours.Count; i++)
                {
                    successBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
                }
            }
        }

        private void ManageFailedBehaviours(bool isInteracting, bool isInspecting)
        {
            if (failedBehaviours.Count > 0)
            {
                for (int i = 0; i < failedBehaviours.Count; i++)
                {
                    failedBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
                }
            }
        }

        public bool IsInteractable()
        {
            return onInteraction;
        }

        public bool IsInspectable()
        {
            return onInspection;
        }
    }
}
