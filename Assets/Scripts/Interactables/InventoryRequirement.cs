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
        [SerializeField] private bool removeFromInventory;
        [SerializeField] private bool destroyItems;
        [SerializeField] private List<Behaviour> successBehaviours;
        [SerializeField] private List<Behaviour> failBehaviours;
        
        public static EventHandler<List<IInventoryItem>> showRequiredItems;
        public static EventHandler<List<IInventoryItem>> hideRequiredItems;

        public List<IInventoryItem> Items()
        {
            var inventoryItems = new List<IInventoryItem>();
            items.ForEach(x=> inventoryItems.Add(x));
            return inventoryItems;
        }
        public void ShowItems() => showRequiredItems?.Invoke(this, Items());
        public void HideItems() => hideRequiredItems?.Invoke(this, Items());

        public void SearchItemsInInventory()
        {
            foreach (var item in items.ToList())
            {
                IInventoryItem inventoryItem = item;
                if (PlayerController.Instance.Inventory.Contains(inventoryItem, removeItem: true, destroyItem: false))
                {
                    if(removeFromInventory) PlayerController.Instance.Inventory.Remove(inventoryItem);
                    if(destroyItems) PlayerController.Instance.Inventory.RemoveAndDestroy(inventoryItem);
                    
                    items.Remove(item);
                    Destroy(item.gameObject);
                }
            }

            if (items.Count == 0) successBehaviours.ForEach(x => x.Activate());
            else failBehaviours.ForEach(x => x.Activate());
        }
    }
}