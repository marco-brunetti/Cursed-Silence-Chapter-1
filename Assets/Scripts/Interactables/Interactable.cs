using System;
using System.Collections.Generic;
using Interactables.Behaviours;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Behaviour = Interactables.Behaviours.Behaviour;

namespace Interactables
{
    public class Interactable : MonoBehaviour, IInteractable
    { 
        [field: SerializeField] public InteractableType Type { get; private set; }
        [SerializeField] private DeactivationType deactivate;
        
        [SerializeField] private InventoryRequirement inventoryRequirement;
        
        [Header("These behaviours will run independently from the inventory requirement")]
        [SerializeField] private List<Behaviour> interactBehaviours = new();
        [SerializeField] private List<Behaviour> inspectBehaviours = new();

        private PlayerInspectModifier inspectModifier;

        public bool CanInteract => interactBehaviours.Count > 0;
        public List<IInventoryItem> RequiredInventoryItems => inventoryRequirement ? inventoryRequirement.Items() : new List<IInventoryItem>();

        public void ShowInventoryRequirement(bool show) => inventoryRequirement?.ShowItems(show);
        public void SetInteractionType(InteractableType type) => Type = type;
        public void AddInteractionBehaviour(Behaviour behaviour) => interactBehaviours.Add(behaviour);

        public bool TryGetModifier(out PlayerInspectModifier modifier)
        {
            if (inspectModifier == null) return TryGetComponent(out modifier);
            modifier = inspectModifier;
            return true;
        }
        
        public void InteractBehaviours()
        {
            if(Type == InteractableType.Interactable && inventoryRequirement) inventoryRequirement.SearchItemsInInventory();
            
            interactBehaviours.ForEach(x=> x.Activate());
            if(deactivate == DeactivationType.OnInteract) GetComponent<Collider>().enabled = false;
        }
        
        public void InspectBehaviours()
        {
            if(Type == InteractableType.Inspectable && inventoryRequirement) inventoryRequirement.SearchItemsInInventory();
            
            inspectBehaviours.ForEach(x=> x.Activate());
            if(deactivate == DeactivationType.OnInspect) GetComponent<Collider>().enabled = false;
        }

        private enum DeactivationType
        {
            NoDeactivate,
            OnInteract,
            OnInspect
        }
        
        public static EventHandler<bool> showUICursor;
        public static EventHandler<bool> ShowUIPoint;
    }
}