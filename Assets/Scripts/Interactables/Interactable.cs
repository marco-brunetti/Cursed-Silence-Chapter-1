using System.Collections.Generic;
using Player;
using UnityEngine;
using Behaviour = Interactables.Behaviours.Behaviour;

namespace Interactables
{
    public class Interactable : MonoBehaviour, IInteractable
    { 
        [field: SerializeField] public InteractableType Type { get; private set; }
        
        [SerializeField] private InventoryRequirement inventoryRequirement;
        
        [Header("These behaviours will run independently from the inventory requirement")]
        [SerializeField] private List<Behaviour> interactBehaviours = new();
        [SerializeField] private List<Behaviour> inspectBehaviours = new();

        private PlayerInspectModifier inspectModifier;
        
        [SerializeField] private DeactivationType deactivationType;
        public List<InventoryItem> RequiredInventoryItems => inventoryRequirement ? inventoryRequirement.Items : new List<InventoryItem>();

        public bool TryGetModifier(out PlayerInspectModifier modifier)
        {
            if (inspectModifier == null) return TryGetComponent(out modifier);
            modifier = inspectModifier;
            return true;
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public void InteractBehaviours()
        {
            interactBehaviours.ForEach(x=> x.Activate());
            if(deactivationType == DeactivationType.OnInteract) GetComponent<Collider>().enabled = false;
        }
        
        public void InspectBehaviours()
        {
            inspectBehaviours.ForEach(x=> x.Activate());
            if(deactivationType == DeactivationType.OnInspect) GetComponent<Collider>().enabled = false;
        }

        private enum DeactivationType
        {
            None,
            OnInteract,
            OnInspect
        }
    }
}