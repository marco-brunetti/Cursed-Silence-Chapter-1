using UnityEngine;
using System;

namespace Interactables.Behaviours
{
    public class InteractableInventoryRequirement : MonoBehaviour
    {
        public InventoryItem[] RequiredInventoryItems;


        public static EventHandler<InventoryItem[]> ShowRequiredItems;
        public static EventHandler<InventoryItem[]> HideRequiredItems;

        public void ShowItems() => ShowRequiredItems?.Invoke(this, RequiredInventoryItems);
        public void HideItems() => HideRequiredItems?.Invoke(this, RequiredInventoryItems);
    }
}