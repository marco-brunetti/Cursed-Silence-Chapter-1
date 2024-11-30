using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class InventoryItem : Behaviour, IInventoryItem
    {
        [field: SerializeField, Header("This acts as a behaviour that adds itself to the inventory.")] public Sprite UiIcon { get; private set; }

        private bool addedToInventory;

        public override void Activate()
        {
            if (addedToInventory) return;
            
            PlayerController.Instance.Inventory.Add(this);
            addedToInventory = true;
        }

    }
}