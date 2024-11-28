using System;
using System.Collections.Generic;
using Interactables.Behaviours;
using UnityEngine;

namespace Interactables
{
    public class InventoryRequirement : MonoBehaviour
    {
        [field:SerializeField] public List<InventoryItem> RequiredInventoryItems { get; private set; }
        
        [SerializeReference] private List<IBehaviour> requirementSuccessBehaviours;
        [SerializeReference] private List<IBehaviour> requirementFailBehaviours;
        
        public static EventHandler<List<InventoryItem>> showRequiredItems;
        public static EventHandler<List<InventoryItem>> hideRequiredItems;

        public void ShowItems() => showRequiredItems?.Invoke(this, RequiredInventoryItems);
        public void HideItems() => hideRequiredItems?.Invoke(this, RequiredInventoryItems);
    }
}