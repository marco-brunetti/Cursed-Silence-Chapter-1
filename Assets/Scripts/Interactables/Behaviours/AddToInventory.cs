using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    [RequireComponent(typeof(InventoryItem))]
    public class AddToInventory : Behaviour
    {
        
        private InventoryItem inventoryItem;

        public override BehaviourType Type => BehaviourType.Interactable;

        public override void Activate()
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