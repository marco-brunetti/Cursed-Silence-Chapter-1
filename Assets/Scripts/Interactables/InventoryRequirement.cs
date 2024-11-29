using System;
using System.Collections.Generic;
using Interactables.Behaviours;
using UnityEngine;
using UnityEngine.Serialization;
using Behaviour = Interactables.Behaviours.Behaviour;

namespace Interactables
{
    public class InventoryRequirement : MonoBehaviour
    {
        [field:SerializeField] public List<InventoryItem> Items { get; private set; }
        
        [SerializeReference] private List<Behaviour> successBehaviours;
        [SerializeReference] private List<Behaviour> failBehaviours;
        
        public static EventHandler<List<InventoryItem>> showRequiredItems;
        public static EventHandler<List<InventoryItem>> hideRequiredItems;

        public void ShowItems() => showRequiredItems?.Invoke(this, Items);
        public void HideItems() => hideRequiredItems?.Invoke(this, Items);
    }
}