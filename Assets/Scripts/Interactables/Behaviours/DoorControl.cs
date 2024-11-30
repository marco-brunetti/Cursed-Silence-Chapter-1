using UnityEngine;
using System.Collections;
using SnowHorse.Utils;

namespace Interactables.Behaviours
{
    public class DoorControl : Behaviour
    {
        [SerializeField] private DoorState currentDoorState = DoorState.Closed;
        [SerializeField] private float maxOpenAngle = 90f;
        [SerializeField] private new Collider collider;
        [SerializeField] private new InteractablePlayAudioEvent audio;
        [SerializeField] private AudioClip openClip;
        [SerializeField] private AudioClip closeClip;

        private readonly float doorMoveDuration = 1f;
        private Vector3 initialRotation;
        private Coroutine moveDoor;

        private void Awake()
        {
            initialRotation = transform.localRotation.eulerAngles;
        }

        public override void Activate()
        {
            switch(currentDoorState)
            {
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

        private enum DoorState {
            Closed,
            Open
        }
    }
}