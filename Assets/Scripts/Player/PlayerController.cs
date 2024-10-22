using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }
        [field: SerializeField] public PlayerInventory Inventory { get; private set; }
        [field: SerializeField] public PlayerStressControl PlayerStress { get; private set; }
        [field: SerializeField] public PlayerAnimation Animation { get; private set; }
        [field: SerializeField] public GameObject Player { get; private set; }

        [SerializeField] private Transform _groundSpawnPoint;
        [SerializeField] private PlayerDataScript _data;
        [SerializeField] private PlayerMovement _movement;
        [SerializeField] private PlayerRotate _rotator;
        [SerializeField] private PlayerInput _input;
        [SerializeField] private PlayerInspect _inspector;
        [SerializeField] private PlayerInteract _interactor;
        [SerializeField] private PlayerAudio _audio;
        [SerializeField] private PlayerCombat _combat;
        [SerializeField] private PostProcessVolume _postProcessVolume;

        [NonSerialized] public IInteractable InteractableInSight;
        [NonSerialized] public bool FreezePlayerMovement;
        [NonSerialized] public bool FreezePlayerRotation;
        [NonSerialized] public bool IsOutside;
        [NonSerialized] public bool IsTeleporting;

        [Header("Player Components")] public GameObject CamHolder;
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
        public bool IsInspecting { get => _inspector.IsInspecting; }
        private BadTVEffect _camDistortion;
        public static EventHandler<Transform> SetPlayerTransform; 

        private bool pause;

        public void Pause(bool isPause)=> pause = isPause;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else Instance = this;

            PlayerData = _data.dataObject;

            _camDistortion = Camera.GetComponent<BadTVEffect>();

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
            _rotator.Rotate(PlayerData, _input, FreezePlayerRotation, pause);
        }

        private void Move()
        {
            if (!IsTeleporting)
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
            _interactor.Interact(PlayerData, _input, _inspector);
            _inspector.ManageInspection(PlayerData, _input);
        }

        private void PlayerAudio()
        {
            _audio.PlayerAudioControl(PlayerData, _input);
        }

        private void ManageStress()
        {
            PlayerStress.ManageStress(PlayerData);
        }

        private void ManageCombat()
        {
            if (InteractableInSight == null) _combat.Manage();
        }

        public void ActivateDepthOfField(bool enable, float currentValue = -1)
        {
            _postProcessVolume.gameObject.SetActive(enable);

            if (currentValue == -1)
                _postProcessVolume.profile.GetSetting<DepthOfField>().focalLength.value =
                    PlayerData.defaultDepthOfField;
            else _postProcessVolume.profile.GetSetting<DepthOfField>().focalLength.value = currentValue;
        }
    }
}