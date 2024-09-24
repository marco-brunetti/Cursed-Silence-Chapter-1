using System.Collections;
using UnityEngine;
using SnowHorse.Utils;
using Player;

public class Behaviour_Oven : MonoBehaviour, IBehaviour
{
    [SerializeField] private GameObject _requiredIngredients;

    [SerializeField] private float _initialDelay = 1.2f;
    [SerializeField] private Light _light;
    [SerializeField] private Behaviour_DoorControl _door;
    [SerializeField] private GameObject _insideInteractable;
    [SerializeField] private Collider[] _doorColliders;

    [Header("Rotating plate is optional")]
    [SerializeField] private GameObject _plate;
    [SerializeField] private float _rotationSpeed = 50;

    [Header("Oven audio")]
    [SerializeField] private AudioClip _cookingClip;
    [SerializeField] private AudioClip _finishedClip;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _volume;

    [Header("Cooked Items")]
    [SerializeField] private GameObject _cookedItem;
    [SerializeField] private Transform _ingredientsHolder;

    private bool _isCooking;
    private bool _ingredientsReady;

    private bool _retrievingIngredients;
    private float _currentLerpTime;
    private Vector3 _previousPosition;
    private Quaternion _previousRotation;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        PrepareIngredientsForRetrieval();

        if (_ingredientsReady &&  isInteracting && !_isCooking && _door.CurrentDoorState == DoorState.Open && _cookedItem != null)
        {
            StartCoroutine(ManageCooking());
        }
    }

    private void PrepareIngredientsForRetrieval()
    {
        PlayerInventory inventory = PlayerController.Instance.Inventory;

        if (inventory.Contains(_requiredIngredients, removeItem:true, destroyItem:false))
        {
            _requiredIngredients.transform.SetParent(_ingredientsHolder);
            _previousPosition = _requiredIngredients.transform.position;
            _previousRotation = _requiredIngredients.transform.rotation;
            _retrievingIngredients = true;
            _ingredientsReady = true;
        }
    }

    private IEnumerator ManageCooking()
    {
        yield return new WaitForSecondsRealtime(_initialDelay);
        CookingStatus(true, _cookingClip);

        yield return new WaitForSecondsRealtime(_cookingClip.length);
        CookingStatus(false, _finishedClip);

        _insideInteractable.SetActive(false);
        _requiredIngredients.SetActive(false);
        _cookedItem.SetActive(true);
        _requiredIngredients = null;
        _cookedItem = null;
    }

    private void CookingStatus(bool cookingStatus, AudioClip cookingStatusClip)
    {
        _audioSource.PlayOneShot(cookingStatusClip, _volume * GameController.Instance.GlobalVolume);

        for (int i = 0; i < _doorColliders.Length; i++)
        {
            _doorColliders[i].enabled = !cookingStatus;
        }

        _isCooking = cookingStatus;
        _light.enabled = cookingStatus;
    }

    private void Update()
    {
        if (_isCooking && _plate != null)
        {
            _plate.transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
        }

        RetrieveIngredients();
    }

    private void RetrieveIngredients()
    {
        if (_retrievingIngredients)
        {
            float percentage = Interpolation.Smoother(1, ref _currentLerpTime);

            _requiredIngredients.transform.position = Vector3.Lerp(_previousPosition, _ingredientsHolder.position, percentage);
            _requiredIngredients.transform.rotation = Quaternion.Lerp(_previousRotation, _ingredientsHolder.rotation, percentage);

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