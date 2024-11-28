using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public interface IInteractable
    {
        public GameObject gameObject { get; }
        public Vector3 InspectableInitialRotation { get; }
        public Vector3 InspectablePosition { get; }
        public List<GameObject> RequiredInventoryItems{ get; }
        public void Inspect();
        public void Interact();
        public bool[] RotateXY();
    }
}