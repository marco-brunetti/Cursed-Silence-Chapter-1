using System.Collections;
using Interactables;
using SnowHorse.Utils;
using UnityEngine;

namespace Player
{
    public class PlayerInspect : MonoBehaviour
    {
        private readonly float rotationSpeed = 0.2f;
        private readonly float inspectableMoveDuration = 0.3f;
        
        private bool[] rotateXY;
        private Vector2 currentRotation;
        private Vector3 previousPosition;
        private Vector3 previousLocalScale;
        private Quaternion previousRotation;
        private Collider interactableCollider;
        private Transform previousParent;
        private Transform interactable;
        private IInteractable interactableComponent;
        private Coroutine currentInspection;

        public bool IsInspecting { get; private set; }

        public void StopInspection() => IsInspecting = false;

        public void Interact()
        {
            if(currentInspection == null) return;
            
            StopCoroutine(currentInspection);
            currentInspection = null;
            IsInspecting = false;
            PlayerController.Instance.FreezePlayer(false);
            interactableComponent.InteractBehaviours();
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
            
            if (interactableComponent.TryGetModifier(out var modifier))
            {
                rotateXY = modifier.RotateXY;
                position = modifier.Position;
                rotation = modifier.Rotation;
                localScale = modifier.Scale;
            }
            else
            {
                rotateXY = new[] { false, false };
            }
            
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
                yield return null;
            }
            
            currentInspection = StartCoroutine(Inspecting());
        }

        private IEnumerator Inspecting()
        {
            currentRotation = interactable.localRotation.eulerAngles;
            while (IsInspecting)
            {
                SetRotation();

                /*if (Input.GetMouseButtonDown(0) && !interactableComponent.InspectableOnly)
                {
                    interactableComponent.Interact();
                }*/
                
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

            if (rotateXY[0]) currentRotation.x += playerInput.mouseMovementInput.y * rotationSpeed;
            if (rotateXY[1]) currentRotation.y -= playerInput.mouseMovementInput.x * rotationSpeed;

            interactable.localRotation = Quaternion.Euler(currentRotation);
        }

        private void ResetInspectable()
        {
            if (interactable.parent != PlayerController.Instance.InventoryHolder)
            {
                interactable.parent = previousParent;
                interactable.position = previousPosition;
                interactable.rotation = previousRotation;
            }

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