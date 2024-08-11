using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInspect : MonoBehaviour
{
    public bool IsInspecting { get; private set; }

    private float _goToInspectorSpeed = 300f;
    private float _resetInspectorTimer = 0.7f;
    private float _rotationSpeed = 0.2f;
    private float _timer;

    private bool _initialSetup;
    private bool _returnItemToPreviousPosition;
    private bool[] _rotateXY;

    private Transform _interactable;
    private Interactable _interactableComponent;
    private Transform _previousParent;

    private Vector3[] _previousPositionAndRotation;
    private Vector3 _currentVelocity;

    private Vector3 _targetRotation;
    private Vector3 _targetPosition;
    public void ManageInspection(PlayerData playerData, IPlayerInput playerInput)
    {
        if (_interactable != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _returnItemToPreviousPosition = true;
                IsInspecting = false;
            }

            if (IsInspecting)
            {
                Inspect(playerData, playerInput);
            }
            else
            {
                ResetInspector(_returnItemToPreviousPosition);
            }
        }
    }

    public void StartInspection(Transform interactable)
    {
        //Forces previous interactable reset if needed
        if(_interactable != null)
        {
            Vector3 targetPosition = _previousPositionAndRotation[0];
            Quaternion targetRotation = Quaternion.Euler(_previousPositionAndRotation[1]);
            ResetInspectable(targetPosition, targetRotation);
        }

        interactable.GetComponent<Collider>().enabled = false;
        _interactableComponent = interactable.GetComponent<Interactable>();
        _interactable = interactable;

        _rotateXY = _interactableComponent.RotateXY();

        _previousParent = interactable.parent;
        _previousPositionAndRotation = new Vector3[] { interactable.localPosition, interactable.localRotation.eulerAngles };

        _timer = _resetInspectorTimer;

        PlayerController playerController = PlayerController.Instance;
        PlayerData playerData = playerController.PlayerData;

        playerController.InspectablesSource.pitch = 1;
        playerController.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

        playerController.FreezePlayerMovement = true;
        playerController.FreezePlayerRotation = true;
        IsInspecting = true;
    }

    private void Inspect(PlayerData playerData, IPlayerInput playerInput)
    {
        if (_initialSetup && !_interactableComponent.InspectableOnly && Input.GetMouseButtonDown(0))
        {
            _interactableComponent.Interact(PlayerController.Instance, true, false);
            _returnItemToPreviousPosition = false;
            IsInspecting = false;
        }


        if (IsInspecting)
        {
            if (!_initialSetup)
            {
                _interactable.parent = PlayerController.Instance.InspectorParent;
                _targetRotation = _interactableComponent.InspectableInitialRotation;
                _targetPosition = _interactableComponent.InspectablePosition;
                _initialSetup = true;
            }

            _interactable.localPosition = Vector3.SmoothDamp(_interactable.localPosition, _targetPosition, ref _currentVelocity, 0.1f, _goToInspectorSpeed);

            SetRotation(playerInput);
        }
    }

    private void SetRotation(IPlayerInput playerInput)
    {
        if (playerInput.mouseMovementInput.x != 0 || playerInput.mouseMovementInput.y != 0)
        {
            Vector2 rotationVector = new Vector2(playerInput.mouseMovementInput.x * _rotationSpeed,
                playerInput.mouseMovementInput.y * _rotationSpeed);

            if (_rotateXY[0]) _targetRotation.x += rotationVector.y;
            if (_rotateXY[1]) _targetRotation.y -= rotationVector.x;

            _interactable.localRotation = Quaternion.Euler(_targetRotation);
        }
    }

    private void ResetInspector(bool returnItemToPreviousPosition)
    {
        if(_interactable != null)
        {
            PlayerController playerController = PlayerController.Instance;
            PlayerData playerData = playerController.PlayerData;

            playerController.FreezePlayerMovement = false;
            playerController.FreezePlayerRotation = false;

            if (returnItemToPreviousPosition)
            {
                _interactableComponent.Interact(playerController, false, false);

                if(_timer == _resetInspectorTimer)
                {
                    playerController.InspectablesSource.pitch = 0.9f;
                    playerController.InspectablesSource.PlayOneShot(playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);
                }

                Vector3 targetPosition = _previousPositionAndRotation[0];
                Quaternion targetRotation = Quaternion.Euler(_previousPositionAndRotation[1]);

                if (_interactable.parent != PlayerController.Instance.InventoryHolder)
                {
                    _interactable.parent = _previousParent;
                    _interactable.localPosition = Vector3.SmoothDamp(_interactable.localPosition, targetPosition, ref _currentVelocity, 0.1f, _goToInspectorSpeed);
                    _interactable.localRotation = Quaternion.Lerp(_interactable.localRotation, targetRotation, _goToInspectorSpeed * Time.deltaTime / 20);
                } 

                if (_timer > 0)
                {
                    _timer -= Time.deltaTime;
                }
                else
                {
                    ResetInspectable(targetPosition, targetRotation);
                }
            }
            else
            {
                if (_interactable.parent != PlayerController.Instance.InventoryHolder)
                {
                    _interactable.parent = _previousParent;
                }

                _interactable = null;
                _interactableComponent = null;
                _initialSetup = false;
                _timer = _resetInspectorTimer;
            }
        }    
    }

    //Forces previous reset of inspectable if needed
    private void ResetInspectable(Vector3 targetPosition, Quaternion targetRotation)
    {
        if (_interactable.parent != PlayerController.Instance.InventoryHolder)
        {
            _interactable.parent = _previousParent;
            _interactable.localPosition = targetPosition;
            _interactable.localRotation = targetRotation;
        }

        _interactable.GetComponent<Collider>().enabled = true;
        _interactable = null;
        _interactableComponent = null;
        _initialSetup = false;
        _timer = _resetInspectorTimer;
    }

    public GameObject CurrentInspectable()
    {
        if (_interactable != null) return _interactable.gameObject;
        else return null;
    }
}