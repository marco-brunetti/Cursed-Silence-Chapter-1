using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_AddToInventory : MonoBehaviour, IBehaviour
    {
        [SerializeField] private InventoryItem inventoryItem;

        private bool _addedToInventory;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if (!_addedToInventory && isInteracting)
            {
                if (!inventoryItem && !gameObject.TryGetComponent(out inventoryItem) && !gameObject.transform.parent.TryGetComponent(out inventoryItem))
                {
                    Debug.Log("Didn't find inventory object! object: " + gameObject.name + "; parent: " + transform.parent.name);
                }

                PlayerController.Instance.Inventory.Add(inventoryItem);
                _addedToInventory = true;
            }
        }

        public bool IsInteractable()
        {
            return true;
        }

        public bool IsInspectable()
        {
            return false;
        }
    }
}
