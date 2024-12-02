using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Behaviours;
using Player;
using UnityEngine;
using Behaviour = Interactables.Behaviours.Behaviour;

namespace Interactables
{
    public class InventoryRequirement : MonoBehaviour
    {
        [SerializeField] private List<InventoryItem> items;
        [SerializeField] private bool waitForAllItems = true;
        [SerializeField] private bool removeFromInventory;
        [SerializeField] private bool destroyItems;
        [SerializeField] private List<Behaviour> successBehaviours;
        [SerializeField] private List<Behaviour> failBehaviours;
        
        public static EventHandler<List<IInventoryItem>> showRequiredItems;
        public static EventHandler hideRequiredItems;

        private bool displayingRequirements;

        public List<IInventoryItem> Items()
        {
            var inventoryItems = new List<IInventoryItem>();
            items.ForEach(x=> inventoryItems.Add(x));
            return inventoryItems;
        }

        public void ShowItems(bool show)
        {
            if (show && !displayingRequirements)
            {
                showRequiredItems?.Invoke(this, Items());
                displayingRequirements = true;
            }
            else if(!show && displayingRequirements)
            {
                hideRequiredItems?.Invoke(this, null);
                displayingRequirements = false;
            }
        }

        public void SearchItemsInInventory()
        {
            foreach (var item in items.ToList())
            {
                if (waitForAllItems && !AllItemsInInventory())
                {
                    break;
                }
                
                if(!PlayerController.Instance.Inventory.Contains(item)) continue;
                
                if(removeFromInventory) PlayerController.Instance.Inventory.Remove(item);
                if(destroyItems) PlayerController.Instance.Inventory.RemoveAndDestroy(item);
                    
                items.Remove(item);
            }

            if (items.Count == 0) successBehaviours.ForEach(x => x.Activate());
            else failBehaviours.ForEach(x => x.Activate());
        }

        private bool AllItemsInInventory()
        {
            foreach (var item in items)
            {
                if(!PlayerController.Instance.Inventory.Contains(item)) return false;
            }

            return true;
        }
    }
}