using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IInteractable
    {
        public InteractableType Type { get; }
        public GameObject gameObject { get; }
        public Vector3 InspectableInitialRotation { get; }
        public Vector3 InspectablePosition { get; }
        public List<InventoryItem> RequiredInventoryItems{ get; }
        public void InspectBehaviours();
        public void InteractBehaviours();
        public bool[] RotateXY();
    }
    
    public enum InteractableType
    {
        Inspectable,
        Interactable
    }
}