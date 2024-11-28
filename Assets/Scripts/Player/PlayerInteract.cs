using SnowHorse.Utils;
using UnityEngine;

namespace Player
{
    public class PlayerInteract : MonoBehaviour
    {
        private PlayerController playerController;

        private void Start()
        {
            playerController = PlayerController.Instance;
        }

        public void Interact(PlayerData playerData, IPlayerInput input, PlayerInspect inspector)
        {
            if(playerController.IsInspecting)
            {
                playerController.InteractableInSight = null;
            }
            else if (Input.GetMouseButtonDown(0) || input.mouseMovementInput != Vector2.zero || input.playerMovementInput != Vector2.zero)
            {
                var rayData = new RaycastData
                {
                    Origin = playerController.Camera.position,
                    Direction = playerController.Camera.forward,
                    MaxDistance = playerData.InteractDistance,
                    LayerMask = playerData.InteractLayer,
                    //Debug = true
                };

                var interactable = Raycaster.Find<IInteractable>(rayData)?.HitObject;

                if(interactable != null) ManageInteraction(interactable, inspector);
                else playerController.InteractableInSight = null;
            }
        }

        private void ManageInteraction(IInteractable interactable, PlayerInspect inspector)
        {
            playerController.InteractableInSight = interactable;

            if (interactable.RequiredInventoryItems.Count > 0)
            {
                //Show inventory required object in UI
            }

            if (Input.GetMouseButtonDown(0))
            {
                switch (interactable.Type)
                {
                    case InteractableType.Inspectable:
                        inspector.Inspect(interactable);
                        break;
                    case InteractableType.Interactable:
                        interactable.InteractBehaviours();
                        break;
                }
            }
        }
    }
}