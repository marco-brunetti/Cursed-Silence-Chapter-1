using System.Collections;
using Player;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class Oven : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_requiredIngredients")] [SerializeField] private InventoryItem requiredIngredients;

        [FormerlySerializedAs("_initialDelay")] [SerializeField] private float initialDelay = 1.2f;
        [FormerlySerializedAs("_light")] [SerializeField] private new Light light;
        [FormerlySerializedAs("_door")] [SerializeField] private DoorControl door;
        [FormerlySerializedAs("_insideInteractable")] [SerializeField] private GameObject insideInteractable;
        [FormerlySerializedAs("_doorColliders")] [SerializeField] private Collider[] doorColliders;

        [FormerlySerializedAs("_plate")]
        [Header("Rotating plate is optional")]
        [SerializeField] private GameObject plate;
        [FormerlySerializedAs("_rotationSpeed")] [SerializeField] private float rotationSpeed = 50;

        [FormerlySerializedAs("_cookingClip")]
        [Header("Oven audio")]
        [SerializeField] private AudioClip cookingClip;
        [FormerlySerializedAs("_finishedClip")] [SerializeField] private AudioClip finishedClip;
        [FormerlySerializedAs("_audioSource")] [SerializeField] private AudioSource audioSource;
        [FormerlySerializedAs("_volume")] [SerializeField] private float volume;

        [FormerlySerializedAs("_cookedItem")]
        [Header("Cooked Items")]
        [SerializeField] private GameObject cookedItem;
        [FormerlySerializedAs("_ingredientsHolder")] [SerializeField] private Transform ingredientsHolder;

        private bool _isCooking;
        private bool _ingredientsReady;

        private bool _retrievingIngredients;
        private float _currentLerpTime;
        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            PrepareIngredientsForRetrieval();

            if (_ingredientsReady &&  isInteracting && !_isCooking && door.currentDoorState == DoorState.Open && cookedItem != null)
            {
                StartCoroutine(ManageCooking());
            }
        }

        private void PrepareIngredientsForRetrieval()
        {
            PlayerInventory inventory = PlayerController.Instance.Inventory;

            if (inventory.Contains(requiredIngredients, removeItem:true, destroyItem:false))
            {
                requiredIngredients.transform.SetParent(ingredientsHolder);
                _previousPosition = requiredIngredients.transform.position;
                _previousRotation = requiredIngredients.transform.rotation;
                _retrievingIngredients = true;
                _ingredientsReady = true;
            }
        }

        private IEnumerator ManageCooking()
        {
            yield return new WaitForSecondsRealtime(initialDelay);
            CookingStatus(true, cookingClip);

            yield return new WaitForSecondsRealtime(cookingClip.length);
            CookingStatus(false, finishedClip);

            insideInteractable.SetActive(false);
            requiredIngredients.gameObject.SetActive(false);
            cookedItem.SetActive(true);
            requiredIngredients = null;
            cookedItem = null;
        }

        private void CookingStatus(bool cookingStatus, AudioClip cookingStatusClip)
        {
            //audioSource.PlayOneShot(cookingStatusClip, volume * GameController.Instance.GlobalVolume);

            for (int i = 0; i < doorColliders.Length; i++)
            {
                doorColliders[i].enabled = !cookingStatus;
            }

            _isCooking = cookingStatus;
            light.enabled = cookingStatus;
        }

        private void Update()
        {
            if (_isCooking && plate != null)
            {
                plate.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }

            RetrieveIngredients();
        }

        private void RetrieveIngredients()
        {
            if (_retrievingIngredients)
            {
                float percentage = Interpolation.Smoother(1, ref _currentLerpTime);

                requiredIngredients.transform.position = Vector3.Lerp(_previousPosition, ingredientsHolder.position, percentage);
                requiredIngredients.transform.rotation = Quaternion.Lerp(_previousRotation, ingredientsHolder.rotation, percentage);

                if(percentage  >= 1) 
                {
                    _retrievingIngredients = false;
                }
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