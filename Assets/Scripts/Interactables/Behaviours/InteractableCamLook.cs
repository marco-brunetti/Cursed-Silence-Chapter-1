using System.Collections;
using UnityEngine;
using Player;

namespace Interactables.Behaviours
{
    public class InteractableCamLook : Behaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float fov;
        [SerializeField] private float lookDuration;

        private PlayerController controller;
        private Coroutine lookAtInteractable;
        
        public override void Activate() => lookAtInteractable ??= StartCoroutine(LookAtInteractable());
        
        private IEnumerator LookAtInteractable()
        {
            controller = PlayerController.Instance;

            controller.FreezePlayer(true);
            controller.ActivateModel(false);
            EnableLookCamera(true);
            
            yield return new WaitForSeconds(lookDuration);
            
            EnableLookCamera(false);
            controller.ActivateModel(true);
            controller.FreezePlayer(false);
            lookAtInteractable = null;
        }

        private void EnableLookCamera(bool enable)
        {
            controller.VirtualCamera.gameObject.SetActive(!enable);
            controller.SecondaryVirtualCamera.transform.position = enable ? transform.position : controller.Player.transform.position;
            controller.SecondaryVirtualCamera.transform.rotation = enable ? transform.rotation : controller.Player.transform.rotation;
            controller.SecondaryVirtualCamera.gameObject.SetActive(enable);
            
            controller.SecondaryVirtualCamera.m_Lens.FieldOfView = fov > 0 ? fov : 50;
        }
    }
}