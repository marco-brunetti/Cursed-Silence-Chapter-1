using UnityEngine;
using SnowHorse.Components;
using System.Collections;
using SnowHorse.Utils;

namespace Interactables.Behaviours
{
    public class Behaviour_DoorControl : MonoBehaviour, IBehaviour
    {
        [SerializeField] private float maxOpenAngle = 90f;
        [SerializeField] private TriggerEnterDetector playerEnterDetector;
        [SerializeField] private TriggerExitDetector playerExitDetector;

        [SerializeField] private InteractableInventoryRequirement inventoryRequirement;
        [SerializeField] private new InteractablePlayAudioEvent audio;
        [SerializeField] private DoorState currentDoorState = DoorState.Closed;
        [SerializeField] private new Collider collider;

        [SerializeField] private AudioClip openClip;
        [SerializeField] private AudioClip closeClip;

        private Vector3 initialRotation;

        private Coroutine moveDoor;
        private float doorMoveDuration = 1f;

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
                        moveDoor ??= StartCoroutine(MoveDoor(targetYRotation: -maxOpenAngle));
                        audio.Play(openClip);
                        currentDoorState = DoorState.Open;
                        break;
                    case DoorState.Open:
                        moveDoor ??= StartCoroutine(MoveDoor(targetYRotation: 0));
                        audio.Play(closeClip);
                        currentDoorState = DoorState.Closed;
                        break;
                }
            }
        }


        private IEnumerator MoveDoor(float targetYRotation)
        {
            collider.enabled = false;
            var lerpRef = 0f;

            while(!Mathf.Approximately(lerpRef, doorMoveDuration))
            {
                var percent = Interpolation.Smooth(doorMoveDuration, ref lerpRef);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(initialRotation.x, initialRotation.y + targetYRotation, initialRotation.z), percent);
                yield return null;
            }

            collider.enabled = true;
            moveDoor = null;
            yield return null;
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