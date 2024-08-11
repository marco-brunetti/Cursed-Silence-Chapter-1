using Cinemachine;
using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerRotate), typeof(PlayerInput))]
[RequireComponent(typeof(PlayerInteract), typeof(PlayerInspect), typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerStressControl), typeof(PlayerAudio))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [field: SerializeField] public PlayerInventory Inventory { get; private set; }
    [field: SerializeField] public PlayerInspect Inspector { get; private set; }
    [field: SerializeField] public PlayerStressControl PlayerStress { get; private set; }
    [field: SerializeField] public GameObject Player { get; private set; }

    [SerializeField] private Transform _groundSpawnPoint;
    [SerializeField] private PlayerDataScript _data;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerRotate _rotator;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PlayerInteract _interactor;
    [SerializeField] private PlayerAudio _audio;

    [NonSerialized] public Interactable InteractableInSight;
    [NonSerialized] public bool FreezePlayerMovement;
    [NonSerialized] public bool FreezePlayerRotation;
    [NonSerialized] public bool IsOutside;
    [NonSerialized] public bool IsTeleporting;

    [Header("Player Components")]
    public GameObject CamHolder;
    public Transform Camera;
    public CinemachineVirtualCamera VirtualCamera;
    public GameObject InventoryCamera;
    public Transform InventoryHolder;
    public Transform InspectorParent;

    public AudioSource InspectablesSource;
    public CharacterController Character;
    public PlayerData PlayerData { get; private set; }

    public bool IsSprinting { get; private set; }
    public bool IsDistorted { get; private set; }

    private BadTVEffect _camDistortion;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;


        PlayerData = _data.dataObject;

        _camDistortion = Camera.GetComponent<BadTVEffect>();
    }

    private void Update()
    {
        Rotate();

        if (GameController.Instance != null && GameController.Instance.Pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;

            Interact();
            Move();
            InventoryManage();
            PlayerAudio();
            ManageStress();
            ManageCameraDistortion();
        }
    }

    private void ManageCameraDistortion()
    {
        if(IsOutside)
        {
            _camDistortion.fineDistort = Mathf.MoveTowards(_camDistortion.fineDistort, PlayerData.DefaultFineDistortion, 0.05f);
            _camDistortion.thickDistort = Mathf.MoveTowards(_camDistortion.thickDistort, PlayerData.DefaultThickDistortion, 0.05f);
        }
    }

    private void Rotate()
    {
        _rotator.Rotate(PlayerData, _input, FreezePlayerRotation);
    }

    private void Move()
    {
        if(!IsTeleporting)
        {
            if (FreezePlayerMovement == false)
            {
                _movement.PlayerMove(PlayerData, _input, _groundSpawnPoint);
                IsSprinting = _input.playerMovementInput != Vector2.zero && _input.playerRunInput;
            }
            else
            {
                Character.Move(Vector3.zero);
            }
        }
    }

    private void Interact()
    {
        _interactor.Interact(PlayerData, _input, Inspector);
        Inspector.ManageInspection(PlayerData, _input);
    }

    private void InventoryManage()
    {
        Inventory.Manage();
    }

    private void PlayerAudio()
    {
        _audio.PlayerAudioControl(PlayerData, _input);
    }

    private void ManageStress()
    {
        PlayerStress.ManageStress(PlayerData);
    }
}
