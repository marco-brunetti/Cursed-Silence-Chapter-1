using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerRotate), typeof(PlayerInput))]
[RequireComponent (typeof(PlayerInteract), typeof(PlayerInspect), typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerStressControl), typeof(PlayerAudio))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [field: SerializeField] public PlayerData PlayerData { get; private set; }
    [field: SerializeField] public PlayerInventory Inventory { get; private set; }
    [field: SerializeField] public PlayerInspect Inspector { get; private set; }
    [field: SerializeField] public PlayerStressControl PlayerStress { get; private set; }

    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerRotate _rotator;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private PlayerInteract _interactor;
    [SerializeField] private PlayerAudio _audio;

    [NonSerialized] public Interactable InteractableInSight;
    [NonSerialized] public bool FreezePlayerMovement;
    [NonSerialized] public bool FreezePlayerRotation;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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
            Move();
            Interact();
            InventoryManage();
            PlayerAudio();
            ManageStress();
        }
    }

    private void Rotate()
    {
        _rotator.Rotate(PlayerData, _input, FreezePlayerRotation);
    }

    private void Move()
    {
        if (FreezePlayerMovement == false)
        {
            _movement.PlayerMove(PlayerData, _input);
        }
        else
        {
            PlayerData.Character.Move(Vector3.zero);
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
