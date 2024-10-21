using Game.General;
using SnowHorse.Utils;
using UnityEngine;

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
                var rayData = new RaycastData
                {
                    Origin = _playerController.Camera.position,
                    Direction = _playerController.Camera.forward,
                    MaxDistance = playerData.InteractDistance,
                    LayerMask = playerData.InteractLayer,
                    //Debug = true
                };

                var interactable = Raycaster.Find<Interactable>(rayData)?.HitObject;

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