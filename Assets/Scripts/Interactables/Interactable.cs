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
        
        public List<IInventoryItem> RequiredInventoryItems => inventoryRequirement ? inventoryRequirement.Items() : new List<IInventoryItem>();

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
            if(deactivate == DeactivationType.OnInteract) GetComponent<Collider>().enabled = false;
        }
        
        public void InspectBehaviours()
        {
            inspectBehaviours.ForEach(x=> x.Activate());
            if(deactivate == DeactivationType.OnInspect) GetComponent<Collider>().enabled = false;
        }

        private enum DeactivationType
        {
            NoDeactivate,
            OnInteract,
            OnInspect
        }
    }
}