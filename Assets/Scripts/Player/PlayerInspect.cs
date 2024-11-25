using System.Collections;
using SnowHorse.Utils;
using UnityEngine;

namespace Player
{
    public class PlayerInspect : MonoBehaviour
    {
        public bool IsInspecting { get; private set; }
        
        private readonly float _rotationSpeed = 0.2f;
        private readonly float _inspectableMoveDuration = 0.3f;
        
        private bool[] _rotateXY;

        private Transform _interactable;
        private IInteractable _interactableComponent;
        private Collider _interactableCollider;
        private Transform _previousParent;
        
        private Vector3 _previousPosition;
        private Quaternion _previousRotation;

        private Vector3 _targetRotation;

        public void ManageInspection()
        {
            if (_interactable)
            {
                if (Input.GetMouseButtonDown(1)) IsInspecting = false;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void StartInspection(Transform interactable)
        {
            IsInspecting = true;
            PlayerController.Instance.FreezePlayerMovement = true;
            PlayerController.Instance.FreezePlayerRotation = true;
            
            if (_interactable != null) ResetInspectable();
            
            _interactable = interactable;
            _interactableCollider = _interactable.GetComponent<Collider>();
            _interactableCollider.enabled = false;
            _interactableComponent = _interactable.GetComponent<IInteractable>();
            _previousParent = _interactable.parent;
            _previousPosition = _interactable.position;
            _previousRotation = _interactable.rotation;
            _rotateXY = _interactableComponent.RotateXY();
            
            StartCoroutine(GoToInspectionPosition());
        }

        private IEnumerator GoToInspectionPosition()
        {
            _interactable.parent = PlayerController.Instance.InspectorParent;
            
            var localPos = _interactable.localPosition;
            var localRot = _interactable.localRotation;
            var lerpTime = 0f;
            
            while(IsInspecting && !Mathf.Approximately(lerpTime, _inspectableMoveDuration))
            {
                var percent = Interpolation.Smoother(_inspectableMoveDuration, ref lerpTime);
                _interactable.localPosition = Vector3.Lerp(localPos, _interactableComponent.InspectablePosition, percent);
                _interactable.localRotation = Quaternion.Lerp(localRot, Quaternion.Euler(_interactableComponent.InspectableInitialRotation), percent);
                yield return null;
            }

            StartCoroutine(Inspecting());
        }

        private IEnumerator Inspecting()
        {
            _targetRotation = _interactable.localRotation.eulerAngles;
            while (IsInspecting)
            {
                SetRotation(PlayerController.Instance.Input);
                yield return null;
            }

            StartCoroutine(ReturnInspectable());
        }

        private IEnumerator ReturnInspectable()
        {
            var position = _interactable.position;
            var rotation = _interactable.rotation;
            var lerpTime = 0f;

            while(!Mathf.Approximately(lerpTime, _inspectableMoveDuration))
            {
                var percent = Interpolation.Smoother(_inspectableMoveDuration, ref lerpTime);
                _interactable.position = Vector3.Lerp(position, _previousPosition, percent);
                _interactable.rotation = Quaternion.Lerp(rotation, _previousRotation, percent);
                yield return null;
            }
            
            ResetInspectable();
            
            PlayerController.Instance.FreezePlayerMovement = false;
            PlayerController.Instance.FreezePlayerRotation = false;
            IsInspecting = false;
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

        private void ResetInspectable()
        {
            if (_interactable.parent != PlayerController.Instance.InventoryHolder)
            {
                _interactable.parent = _previousParent;
                _interactable.position = _previousPosition;
                _interactable.rotation = _previousRotation;
            }

            _interactableCollider.enabled = true;
            _interactable = null;
            _interactableComponent = null;
            _interactableCollider = null;
        }
    }
}