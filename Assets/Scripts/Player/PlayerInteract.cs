using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private PlayerController _playerController;
    private Ray _interactRay;

    private void Start()
    {
        _playerController = PlayerController.Instance;
    }

    public void Interact(PlayerData playerData, IPlayerInput input, PlayerInspect inspector)
    {
        if (Input.GetMouseButtonDown(0) || input.mouseMovementInput != Vector2.zero || input.playerMovementInput != Vector2.zero)
        {
            _interactRay.origin = PlayerController.Instance.Camera.position;
            _interactRay.direction = PlayerController.Instance.Camera.forward;

            RaycastHit hit;

            if (Physics.Raycast(_interactRay, out hit, playerData.InteractDistance, playerData.InteractLayer))
            {
                if (!PlayerController.Instance.Inspector.IsInspecting && hit.collider != null)
                {
                    ManageInteraction(hit.collider.gameObject, inspector);
                }
            }
            else
            {
                _playerController.InteractableInSight = null;
            }
        }   
    }

    private void ManageInteraction(GameObject interactableObject, PlayerInspect inspector)
    {
        if (interactableObject.TryGetComponent(out Interactable interactable))
        {
            _playerController.InteractableInSight = interactable;

            if (Input.GetMouseButtonDown(0))
            {
                if (interactable.NonInspectable)
                {
                    interactable.Interact(_playerController, true, false);
                    _playerController.InteractableInSight = null;
                }
                else
                {
                    interactable.Interact(_playerController, false, true);
                    inspector.StartInspection(interactableObject.transform);
                }
            }
        }
        else if (inspector.IsInspecting == false)
        {
            _playerController.InteractableInSight = null;
        }
    }
}