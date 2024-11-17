using UnityEngine;
using SnowHorse.Components;

namespace Interactables.Behaviours
{
    public class Behaviour_DoorControl : MonoBehaviour, IBehaviour
    {
        [SerializeField] private TriggerEnterDetector playerEnterDetector;
        [SerializeField] private TriggerExitDetector playerExitDetector;

        [SerializeField] private InteractableInventoryRequirement inventoryRequirement;
        [SerializeField] private DoorState currentDoorState = DoorState.Closed;

        private Vector3 initialRotation;

        private void Awake()
        {
            playerEnterDetector.Init(new() { "player" });
            playerExitDetector.Init(new() { "player" });

            playerEnterDetector.TagEntered += (sender, e) => inventoryRequirement.ShowItems();
            playerExitDetector.TagExited += (sender, e) => inventoryRequirement.HideItems();

            initialRotation = transform.localRotation.eulerAngles;
        }

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(isInteracting)
            {
                switch(currentDoorState)
                {
                    case DoorState.Locked:
                        break;
                    case DoorState.Closed:
                        transform.localRotation = Quaternion.Euler(initialRotation.x, initialRotation.y - 90f, initialRotation.z);
                        currentDoorState = DoorState.Open;
                        break;
                    case DoorState.Open:
                        transform.localRotation = Quaternion.Euler(initialRotation.x, initialRotation.y, initialRotation.z);
                        currentDoorState = DoorState.Closed;
                        break;
                }

            }
        }


        public bool IsInteractable() => true;
        public bool IsInspectable() => false;

        private enum DoorState {
            Locked,
            Closed,
            Open
        }
    }
}