using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        [field: SerializeField] public PlayerInventory Inventory { get; private set; }
        [field: SerializeField] public PlayerStressControl PlayerStress { get; private set; }
        [field: SerializeField] public PlayerAnimation Animation { get; private set; }
        [field: SerializeField] public GameObject Player { get; private set; }
        [field: SerializeField] public PlayerInput Input { get; private set; }

        [SerializeField] private Transform _groundSpawnPoint;
        [SerializeField] private PlayerDataScript _data;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerRotate _rotator;
        [SerializeField] private PlayerInspect _inspector;
        [SerializeField] private PlayerInteract _interactor;
        [SerializeField] private PlayerAudio _audio;
        [SerializeField] private PlayerCombat _combat;
        [SerializeField] private PostProcessVolume _postProcessVolume;
        [SerializeField] private GameObject _playerModel;

        [NonSerialized] public IInteractable InteractableInSight;
        [NonSerialized] public bool FreezePlayerMovement;
        [NonSerialized] public bool FreezePlayerRotation;
        [NonSerialized] public bool IsOutside;
        [NonSerialized] public bool IsTeleporting;

        [Header("Player Components")] public GameObject CamHolder;
        public Transform Camera;
        public CinemachineBrain CinemachineBrain;
        public CinemachineVirtualCamera VirtualCamera;
        public CinemachineVirtualCamera SecondaryVirtualCamera;
        public Transform InventoryHolder;
        public Transform InspectorParent;

        public AudioSource InspectablesSource;
        public CharacterController Character;
        public PlayerData PlayerData { get; private set; }

        public bool IsSprinting { get; private set; }
        public bool IsDistorted { get; private set; }
        public bool IsInspecting { get => _inspector.IsInspecting; }
        private BadTVEffect _camDistortion;
        public static EventHandler<Transform> SetPlayerTransform;
        public static EventHandler<bool> showUIPoint;

        private bool pause;
        private bool canInteract = true;

        public void Pause(bool isPause)=> pause = isPause;
        
        public bool FreezePlayer(bool freeze) => FreezePlayerMovement = FreezePlayerRotation = freeze;
        
        public void ActivateModel(bool activate) => _playerModel.SetActive(activate);

        public void DeactivateInteraction() => canInteract = false;

        public void ReactivateInteraction()
        {
            canInteract = true;
            Interact();
        }
        

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else Instance = this;

            PlayerData = _data.dataObject;

            _camDistortion = Camera.GetComponent<BadTVEffect>();
        }

        private void Start()
        {
            SetPlayerTransform?.Invoke(this, Player.transform);
        }

        private void Update()
        {
            Rotate();

            if (pause)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;

                Interact();
                ManageCombat();
                Move();
                PlayerAudio();
                ManageStress();
                ManageCameraDistortion();
            }
        }

        private void ManageCameraDistortion()
        {
            if (IsOutside)
            {
                _camDistortion.fineDistort =
                    Mathf.MoveTowards(_camDistortion.fineDistort, PlayerData.DefaultFineDistortion, 0.05f);
                _camDistortion.thickDistort = Mathf.MoveTowards(_camDistortion.thickDistort,
                    PlayerData.DefaultThickDistortion, 0.05f);
            }
        }

        private void Rotate()
        {
            _rotator.Rotate(PlayerData, Input, FreezePlayerRotation, pause);
        }

        private void Move()
        {
            if (!IsTeleporting)
            {
                if (FreezePlayerMovement == false)
                {
                    _movement.PlayerMove(PlayerData, Input, Character.velocity.magnitude);
                    IsSprinting = Input.playerMovementInput != Vector2.zero && Input.playerRunInput;
                }
                else
                {
                    Character.Move(Vector3.zero);
                }
            }
        }

        private void Interact()
        {
            if(!canInteract) return;

            if (IsInspecting)
            {
                if(UnityEngine.Input.GetMouseButtonDown(0)) _inspector.Interact();
                else if(UnityEngine.Input.GetMouseButtonDown(1)) _inspector.StopInspection();
            }
            else
            {
                _interactor.Interact(PlayerData, Input, _inspector);
            }
        }

        private void PlayerAudio()
        {
            _audio.PlayerAudioControl(PlayerData, Input);
        }

        private void ManageStress()
        {
            PlayerStress.ManageStress(PlayerData);
        }

        private void ManageCombat()
        {
            _combat.Manage(currentVelocity: Character.velocity.magnitude);
        }

        public void ActivateDepthOfField(bool enable, float currentValue = -1)
        {
            //_postProcessVolume.gameObject.SetActive(enable);

            /*if (currentValue == -1)
                _postProcessVolume.profile.GetSetting<DepthOfField>().focalLength.value =
                    PlayerData.defaultDepthOfField;
            else _postProcessVolume.profile.GetSetting<DepthOfField>().focalLength.value = currentValue;*/
        }
    }
}