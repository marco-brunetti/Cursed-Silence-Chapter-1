using System;
using System.Collections.Generic;
using Interactables.Behaviours;
using Player;
using UnityEngine;
using Behaviour = Interactables.Behaviours.Behaviour;

namespace Interactables
{
    public class InventoryRequirement : MonoBehaviour
    {
        [SerializeField] private List<InventoryItem> items;
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
    }
}