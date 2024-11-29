using System.Collections.Generic;
using Interactables;
using UnityEngine;

namespace Player
{
    public interface IInteractable
    {
        public InteractableType Type { get; }
        public GameObject gameObject { get; }
        public bool TryGetModifier(out PlayerInspectModifier modifier);
        public List<InventoryItem> RequiredInventoryItems{ get; }
        public void InspectBehaviours();
        public void InteractBehaviours();
    }
    
    public enum InteractableType
    {
        Inspectable,
        Interactable
    }
}