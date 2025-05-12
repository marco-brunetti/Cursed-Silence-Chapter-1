using System.Collections;
using Interactables;
using SnowHorse.Utils;
using UnityEngine;

namespace Player
{
    public class PlayerInspect : MonoBehaviour
    {
        private readonly float rotationSpeed = 0.15f;
        private readonly float inspectableMoveDuration = 0.3f;
        private Vector2 currentRotation;
        private Vector3 previousPosition;
        private Vector3 previousLocalScale;
        private Quaternion previousRotation;
        private Collider interactableCollider;
        private Transform previousParent;
        private Transform interactable;
        private Coroutine currentInspection;
        private IInteractable interactableComponent;
        private PlayerInspectModifier modifier;

        public bool IsInspecting { get; private set; }
        public void StopInspection() => IsInspecting = false;

        public void Interact()
        {
            if(currentInspection == null || interactableComponent?.CanInteract == false) return;
            
            interactableComponent?.InteractBehaviours();
            
            StopCoroutine(currentInspection);
            currentInspection = null;
            IsInspecting = false;
            PlayerController.Instance.FreezePlayer(false);
            CleanVariables();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void Inspect(IInteractable item)
        {
            IsInspecting = true;
            PlayerController.Instance.FreezePlayer(true);
            
            if (interactable != null) ResetInspectable();
            
            interactable = item.gameObject.transform;
            interactableComponent = item;
            interactableCollider = item.gameObject.GetComponent<Collider>();
            interactableComponent = item.gameObject.GetComponent<IInteractable>();
            previousParent = interactable.parent;
            previousPosition = interactable.position;
            previousRotation = interactable.rotation;
            previousLocalScale = interactable.localScale;
            interactableCollider.enabled = false;
            
            interactableComponent.InspectBehaviours();

            var position = Vector3.zero;
            var rotation = Vector3.zero;
            var localScale = Vector3.zero;
            
            if (interactableComponent.TryGetModifier(out var inspectModifier))
            {
                modifier = inspectModifier;
                position = modifier.Position;
                rotation = modifier.Rotation;
                localScale = modifier.Scale;
            }
            
            PlayerController.ShowUIPoint?.Invoke(this, false);
            
            StartCoroutine(GoToInspectionPosition(position, rotation, localScale));
        }

        private IEnumerator GoToInspectionPosition(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            interactable.parent = PlayerController.Instance.InspectorParent;
            
            var localPos = interactable.localPosition;
            var localScale = interactable.localScale;
            var localRot = interactable.localRotation;
            var lerpTime = 0f;
            
            while(IsInspecting && !Mathf.Approximately(lerpTime, inspectableMoveDuration))
            {
                var percent = Interpolation.Smoother(inspectableMoveDuration, ref lerpTime);
                interactable.localPosition = Vector3.Lerp(localPos, position, percent);
                if(scale != Vector3.zero) interactable.localScale = Vector3.Lerp(localScale, scale, percent);
                interactable.localRotation = Quaternion.Lerp(localRot, Quaternion.Euler(rotation), percent);
                currentRotation = interactable.localRotation.eulerAngles;
                yield return null;
            }
            
            currentInspection = StartCoroutine(Inspecting());
        }

        private IEnumerator Inspecting()
        {
            while (IsInspecting)
            {
                SetRotation();
                yield return null;
            }

            currentInspection = null;
            StartCoroutine(ReturnInspectable());
        }

        private IEnumerator ReturnInspectable()
        {
            interactable.parent = previousParent;
            
            var position = interactable.position;
            var rotation = interactable.rotation;
            var scale = interactable.localScale;
            var lerpTime = 0f;

            while(!Mathf.Approximately(lerpTime, inspectableMoveDuration))
            {
                var percent = Interpolation.Smoother(inspectableMoveDuration, ref lerpTime);
                interactable.position = Vector3.Lerp(position, previousPosition, percent);
                interactable.localScale = Vector3.Lerp(scale, previousLocalScale, percent);
                interactable.rotation = Quaternion.Lerp(rotation, previousRotation, percent);
                yield return null;
            }
            
            ResetInspectable();
            
            PlayerController.Instance.FreezePlayer(false);
            IsInspecting = false;
        }
        
        private void SetRotation()
        {
            var playerInput = PlayerController.Instance.Input;
            
            if (playerInput.mouseMovementInput == Vector2.zero) return;

            if (modifier?.RotateX == true)
            {
                var speed = modifier ? rotationSpeed * modifier.SensitivityX : rotationSpeed;
                var rot = playerInput.mouseMovementInput.y * speed;
                currentRotation.x = modifier?.InvertX == true ? currentRotation.x - rot : currentRotation.x + rot;
            }

            if (modifier?.RotateY == true)
            {
                var speed = modifier ? rotationSpeed * modifier.SensitivityY : rotationSpeed;
                var rot = playerInput.mouseMovementInput.x * speed;
                currentRotation.y = modifier?.InvertY == true ? currentRotation.y + rot : currentRotation.y - rot;
            }

            if(modifier?.RotateX == true || modifier?.RotateY == true) interactable.localRotation = Quaternion.Euler(currentRotation);
        }

        private void ResetInspectable()
        {
            if (interactable.parent != PlayerController.Instance.InventoryHolder)
            {
                interactable.parent = previousParent;
                interactable.position = previousPosition;
                interactable.rotation = previousRotation;
            }

            PlayerController.ShowUIPoint?.Invoke(this, true);
            
            CleanVariables();
        }

        private void CleanVariables()
        {
            interactableCollider.enabled = true;
            interactable = null;
            interactableComponent = null;
            interactableCollider = null;
        }
    }
}