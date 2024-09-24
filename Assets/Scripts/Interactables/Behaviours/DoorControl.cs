using System.Collections;
using Player;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public enum DoorState { Locked, Closed, Open };
    public class DoorControl : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_key")] [SerializeField] private InventoryItem key;

        [FormerlySerializedAs("_doorPivot")]
        [Header("Door Properties")]
        [SerializeField] private Transform doorPivot;
        [FormerlySerializedAs("_openRotation")] [SerializeField] private Transform openRotation;
        [FormerlySerializedAs("_closedRotation")] [SerializeField] private Transform closedRotation;
        [FormerlySerializedAs("_colliders")] [SerializeField] private Collider[] colliders;
        [FormerlySerializedAs("_movementDurationInSeconds")] [SerializeField] private float movementDurationInSeconds;

        [FormerlySerializedAs("_isLockedClip")]
        [Header("Door Audio")]
        [SerializeField] private AudioClip isLockedClip;
        [FormerlySerializedAs("_isUnlockedClip")] [SerializeField] private AudioClip isUnlockedClip;
        [FormerlySerializedAs("_isOpeningClip")] [SerializeField] private AudioClip isOpeningClip;
        [FormerlySerializedAs("_isClosingClip")] [SerializeField] private AudioClip isClosingClip;
        [FormerlySerializedAs("_audioSource")] [SerializeField] private AudioSource audioSource;
        [FormerlySerializedAs("_volume")] [SerializeField] private float volume;

        [FormerlySerializedAs("_renderer")]
        [Header("Use in case door illumination is needed")]
        [SerializeField] private new Renderer renderer;
        [FormerlySerializedAs("_emmisiveMat")] [SerializeField] private Material emmisiveMat;
        [FormerlySerializedAs("_opaqueMat")] [SerializeField] private Material opaqueMat;
        [FormerlySerializedAs("_lights")] [SerializeField] private GameObject lights;


        [FormerlySerializedAs("CurrentDoorState")] public DoorState currentDoorState;
        private Vector3 _initialRotation;
        private bool _changeDoorState;
        private float _currentLerpPercentage;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            switch(currentDoorState)
            {
                case DoorState.Locked:
                {
                    PlayerInventory inventory = PlayerController.Instance.Inventory;

                    if (key && inventory.Contains(key, removeItem:true, destroyItem:true))
                    {
                        audioSource.PlayOneShot(isUnlockedClip, volume * GameController.Instance.GlobalVolume); //Unlock sound

                        currentDoorState = DoorState.Closed;
                        _changeDoorState = true;
                        StartCoroutine(ManageDuration(movementDurationInSeconds));
                    }
                    else
                    {
                        audioSource.PlayOneShot(isLockedClip, volume * GameController.Instance.GlobalVolume); //Locked sound
                    }
                    break;
                }
                default:
                {
                    _initialRotation = doorPivot.localRotation.eulerAngles;
                    _changeDoorState = true;
                    StartCoroutine(ManageDuration(movementDurationInSeconds));
                    break;
                }
            }
        }

        void Update()
        {
            if(renderer != null)
            {
                ManageLights();
            }

            ManageRotation();
        }

        private void ManageRotation()
        {
            if (_changeDoorState)
            {
                float percentage = Interpolation.Smoother(movementDurationInSeconds, ref _currentLerpPercentage);

                switch (currentDoorState)
                {
                    case DoorState.Open:
                    {
                        doorPivot.localRotation = Quaternion.Lerp(Quaternion.Euler(_initialRotation), closedRotation.localRotation, percentage);
                        break;
                    }
                    case DoorState.Closed:
                    {
                        doorPivot.localRotation = Quaternion.Lerp(Quaternion.Euler(_initialRotation), openRotation.localRotation, percentage);
                        break;
                    }
                }
            }
        }

        private IEnumerator ManageDuration(float duration)
        {
            switch (currentDoorState)
            {
                case DoorState.Open:
                {
                    audioSource.PlayOneShot(isClosingClip, volume * GameController.Instance.GlobalVolume);
                    break;
                }
                case DoorState.Closed:
                {
                    audioSource.PlayOneShot(isOpeningClip, volume * GameController.Instance.GlobalVolume);
                    break;
                }
            }

            for(int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }

            yield return new WaitForSecondsRealtime(duration);

            switch (currentDoorState)
            {
                case DoorState.Open:
                {
                    currentDoorState = DoorState.Closed;
                    break;
                }
                case DoorState.Closed:
                {
                    currentDoorState = DoorState.Open;
                    break;
                }
            }

            _changeDoorState = false;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;
            }

            _currentLerpPercentage = 0; //reset lerp percentage for next time
        }

        private void ManageLights()
        {
            if (currentDoorState == DoorState.Locked)
            {
                if (PlayerController.Instance.Inventory.Contains(key, removeItem: false, destroyItem:false))
                {
                    lights.SetActive(true);
                    renderer.material = emmisiveMat;
                }
                else
                {
                    lights.SetActive(false);
                    renderer.material = opaqueMat;
                }
            }
            else
            {
                lights.SetActive(true);
                renderer.material = opaqueMat;
            }
        }

        public bool IsInteractable()
        {
            return true;
        }

        public bool IsInspectable()
        {
            return false;
        }
    }
}