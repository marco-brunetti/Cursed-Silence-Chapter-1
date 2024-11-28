using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    [RequireComponent(typeof(InventoryItem))]
    public class AddToInventory : MonoBehaviour, IBehaviour
    {
        
        private InventoryItem inventoryItem;

        public BehaviourType Type => BehaviourType.Interactable;

        public void Behaviour()
        {
            if (!addedToInventory)
            {
                PlayerController.Instance.Inventory.Add(inventoryItem);
                addedToInventory = true;
            }
        }

        private void Awake()
        {
            inventoryItem = GetComponent<InventoryItem>();
        }

        private bool addedToInventory;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            
        }
    }
}