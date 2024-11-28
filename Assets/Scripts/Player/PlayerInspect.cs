using System.Collections;
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
        private Collider interactableCollider;
        private Quaternion previousRotation;
        private Transform previousParent;
        private Transform interactable;
        private IInteractable interactableComponent;

        public bool IsInspecting { get; private set; }

        public void StopInspection() => IsInspecting = false;

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
            rotateXY = interactableComponent.RotateXY();
            interactableCollider.enabled = false;
            
            interactableComponent.InspectBehaviours();
            
            StartCoroutine(GoToInspectionPosition());
        }

        private IEnumerator GoToInspectionPosition()
        {
            interactable.parent = PlayerController.Instance.InspectorParent;
            
            var localPos = interactable.localPosition;
            var localRot = interactable.localRotation;
            var lerpTime = 0f;
            
            while(IsInspecting && !Mathf.Approximately(lerpTime, inspectableMoveDuration))
            {
                var percent = Interpolation.Smoother(inspectableMoveDuration, ref lerpTime);
                interactable.localPosition = Vector3.Lerp(localPos, interactableComponent.InspectablePosition, percent);
                interactable.localRotation = Quaternion.Lerp(localRot, Quaternion.Euler(interactableComponent.InspectableInitialRotation), percent);
                yield return null;
            }

            StartCoroutine(Inspecting());
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

            StartCoroutine(ReturnInspectable());
        }

        private IEnumerator ReturnInspectable()
        {
            var position = interactable.position;
            var rotation = interactable.rotation;
            var lerpTime = 0f;

            while(!Mathf.Approximately(lerpTime, inspectableMoveDuration))
            {
                var percent = Interpolation.Smoother(inspectableMoveDuration, ref lerpTime);
                interactable.position = Vector3.Lerp(position, previousPosition, percent);
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

            interactableCollider.enabled = true;
            interactable = null;
            interactableComponent = null;
            interactableCollider = null;
        }
    }
}