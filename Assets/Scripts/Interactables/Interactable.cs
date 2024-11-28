using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Behaviours;
using JetBrains.Annotations;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables
{
    public class Interactable : MonoBehaviour, IInteractable
    { 
        [field: SerializeField] public InteractableType Type { get; private set; }
        
        [Header("These behaviours will run independently from the inventory requirement.")]
        [SerializeReference] private List<IBehaviour> interactBehaviours = new();
        [SerializeReference] private List<IBehaviour> inspectBehaviours = new();
        
        private InspectableModifier inspectableModifier;

        public bool DeactivateBehaviours;

        public Vector3 InspectableInitialRotation => inspectableModifier ? inspectableModifier.Rotation : Vector3.zero; 
        public Vector3 InspectablePosition => inspectableModifier ? inspectableModifier.Position : Vector3.zero;
        public bool[] RotateXY() => new[] { inspectableModifier && inspectableModifier.RotateX, inspectableModifier && inspectableModifier.RotateY };
        public List<GameObject> RequiredInventoryItems { get; }

        private void Awake()
        {
            TryGetComponent(out inspectableModifier);
        }

        public void Inspect()
        {
            inspectBehaviours.ForEach(x=> x.Behaviour());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Interact()
        {
            interactBehaviours.ForEach(x=> x.Behaviour());

            if (DeactivateBehaviours) GetComponent<Collider>().enabled = false;
        }
    
    
        public enum InteractableType
        {
            Inspectable,
            Interactable,
            Mixed
        }
    }
}