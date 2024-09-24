using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_RequireObject : MonoBehaviour, IBehaviour
    {
        [SerializeField] private InventoryItem _requiredObject;
        [SerializeField] private bool _onInteraction = true;
        [SerializeField] private bool _onInspection;

        [SerializeField] private List<GameObject> _successBehaviours = new List<GameObject>();
        [SerializeField] private List<GameObject> _failedBehaviours = new List<GameObject>();

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(PlayerController.Instance.Inventory.Contains(_requiredObject, removeItem: true, destroyItem: false))
            {
                if (_onInteraction && isInteracting)
                {
                    ManageSuccessBehaviours(isInteracting, false);
                }
                if (_onInspection && isInspecting)
                {
                    ManageSuccessBehaviours(false, isInspecting);
                }
            }
            else if(!_requiredObject)
            {
                if (_onInteraction && isInteracting)
                {
                    ManageSuccessBehaviours(isInteracting, false);
                }
                if (_onInspection && isInspecting)
                {
                    ManageSuccessBehaviours(false, isInspecting);
                }
            }
            else
            {
                if (_onInteraction && isInteracting)
                {
                    ManageFailedBehaviours(isInteracting, false);
                }
                if (_onInspection && isInspecting)
                {
                    ManageFailedBehaviours(false, isInspecting);
                }
            }
        }

        private void ManageSuccessBehaviours(bool isInteracting, bool isInspecting)
        {
            if (_successBehaviours.Count > 0)
            {
                for (int i = 0; i < _successBehaviours.Count; i++)
                {
                    _successBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
                }
            }
        }

        private void ManageFailedBehaviours(bool isInteracting, bool isInspecting)
        {
            if (_failedBehaviours.Count > 0)
            {
                for (int i = 0; i < _failedBehaviours.Count; i++)
                {
                    _failedBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
                }
            }
        }

        public bool IsInteractable()
        {
            return _onInteraction;
        }

        public bool IsInspectable()
        {
            return _onInspection;
        }
    }
}
