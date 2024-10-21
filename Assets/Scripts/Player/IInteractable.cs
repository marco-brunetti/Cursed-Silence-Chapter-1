using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IInteractable
    {
        public GameObject gameObject { get; }
        public bool NonInspectable { get; }
        public bool InspectableOnly { get; }
        public Vector3 InspectableInitialRotation { get; }
        public Vector3 InspectablePosition { get; }
        public List<GameObject> RequiredInventoryItems{ get; }
        public void Interact(PlayerController playerController, bool isInteracting, bool isInspecting);
        public bool[] RotateXY();
    }
}