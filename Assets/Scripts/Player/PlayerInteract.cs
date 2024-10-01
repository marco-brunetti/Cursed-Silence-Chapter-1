using SnowHorse.Utils;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private PlayerController _playerController;

        private void Start()
        {
            _playerController = PlayerController.Instance;
        }

        public void Interact(PlayerData playerData, IPlayerInput input, PlayerInspect inspector)
        {
            if(_playerController.IsInspecting)
            {
                _playerController.InteractableInSight = null;
            }
            else if (!GameController.Instance.IsInDream && 
            (Input.GetMouseButtonDown(0) || input.mouseMovementInput != Vector2.zero || input.playerMovementInput != Vector2.zero))
            {
                var interactable = Raycaster.Cast<Interactable>(new() { origin = _playerController.Camera.position, direction = _playerController.Camera.forward },
                                                                out Vector3 hitPoint,
                                                                maxDistance: playerData.InteractDistance,
                                                                layerMask: playerData.InteractLayer);

                if(interactable) ManageInteraction(interactable, inspector);
                else _playerController.InteractableInSight = null;
            }
        }

        private void ManageInteraction(Interactable interactable, PlayerInspect inspector)
        {
            _playerController.InteractableInSight = interactable;

            if (interactable.RequiredInventoryItems.Count > 0)
            {
                //Show inventory required object in UI
            }

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
                    inspector.StartInspection(interactable.transform);
                }
            }
        }
    }
}