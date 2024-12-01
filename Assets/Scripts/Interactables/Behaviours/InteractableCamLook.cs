using System.Collections;
using UnityEngine;
using Player;

namespace Interactables.Behaviours
{
    public class InteractableCamLook : Behaviour
    {
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
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> !controller.CinemachineBrain.IsBlending);
            
            controller.ActivateModel(true);
            controller.FreezePlayer(false);
            lookAtInteractable = null;
        }

        private void EnableLookCamera(bool enable)
        {
            controller.VirtualCamera.gameObject.SetActive(!enable);

            if (enable)
            {
                controller.SecondaryVirtualCamera.transform.position = transform.position;
                controller.SecondaryVirtualCamera.transform.rotation = transform.rotation;
                controller.SecondaryVirtualCamera.m_Lens.FieldOfView = fov > 0 ? fov : 50;
            }
                
            controller.SecondaryVirtualCamera.gameObject.SetActive(enable);
        }
    }
}