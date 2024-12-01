using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Player;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class InteractableCamLook : Behaviour
    {
        [SerializeField] private bool showCursor;
        [SerializeField] private float fov;
        [SerializeField] private float lookDuration = 1;
        [SerializeField] private float startLookBlend;
        [SerializeField] private float endLookBlend;
        [SerializeField] private NoiseSettings noise;
        
        public bool IsLooking { get; private set; }
        public static EventHandler<bool> showUICursor;

        private PlayerController controller;
        private Coroutine lookAtInteractable;

        
        public override void Activate() => lookAtInteractable ??= StartCoroutine(LookAtInteractable());
        
        public void StopLooking() => IsLooking = false;
        
        private IEnumerator LookAtInteractable()
        {
            controller = PlayerController.Instance;
            
            controller.FreezePlayer(true);
            controller.ActivateModel(false);
            EnableLookCamera(true);

            if (lookDuration > 0)
            {
                yield return new WaitForSeconds(lookDuration);
            }
            else
            {
                IsLooking = true;
                yield return new WaitUntil(() => !IsLooking);
            }
            
            EnableLookCamera(false);
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> !controller.CinemachineBrain.IsBlending);
            
            controller.ActivateModel(true);
            controller.FreezePlayer(false);
            lookAtInteractable = null;
        }

        private void EnableLookCamera(bool enable)
        {
            controller.CanInteract = !enable;
            
            if(showCursor) showUICursor?.Invoke(this, enable);
            
            controller.CinemachineBrain.m_DefaultBlend.m_Time = enable ? startLookBlend : endLookBlend;
            controller.VirtualCamera.gameObject.SetActive(!enable);

            if (enable)
            {
                controller.SecondaryVirtualCamera.transform.position = transform.position;
                controller.SecondaryVirtualCamera.transform.rotation = transform.rotation;
                controller.SecondaryVirtualCamera.m_Lens.FieldOfView = fov > 0 ? fov : 50;
            }

            controller.SecondaryVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>()
                .m_NoiseProfile = noise;

            controller.SecondaryVirtualCamera.gameObject.SetActive(enable);
        }
    }
}