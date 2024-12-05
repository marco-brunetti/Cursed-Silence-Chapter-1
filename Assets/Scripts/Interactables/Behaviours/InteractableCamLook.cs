using System.Collections;
using Cinemachine;
using UnityEngine;
using Player;

namespace Interactables.Behaviours
{
    public class InteractableCamLook : Behaviour
    {
        [SerializeField] private bool showCursor;
        [SerializeField] private bool showUIPoint;
        [SerializeField] private float fov;
        [SerializeField] private float lookDuration = 1;
        [SerializeField] private float startLookBlend;
        [SerializeField] private float endLookBlend;
        [SerializeField] private NoiseSettings noise;
        
        public bool IsLooking { get; private set; }
        
        private PlayerController controller;
        private Coroutine lookAtInteractable;
        
        public override void Activate() => lookAtInteractable ??= StartCoroutine(LookAtInteractable());
        
        public void StopLooking() => IsLooking = false;
        
        private IEnumerator LookAtInteractable()
        {
            Interactable.showUIPoint?.Invoke(this, false);
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
                yield return new WaitForSeconds(startLookBlend);
                IsLooking = true;
                yield return new WaitUntil(() => !IsLooking);
            }
            
            EnableLookCamera(false);
            
            yield return new WaitForEndOfFrame();
            yield return new WaitUntil(()=> !controller.CinemachineBrain.IsBlending);
            
            controller.ActivateModel(true);
            controller.FreezePlayer(false);
            lookAtInteractable = null;
            
            Interactable.showUIPoint?.Invoke(this, true);
        }

        private void EnableLookCamera(bool enable)
        {
            if (enable) controller.DeactivateInteraction();
            else controller.ReactivateInteraction();
            
            if(showCursor) Interactable.showUICursor?.Invoke(this, enable);
            
            controller.CinemachineBrain.m_DefaultBlend.m_Time = enable ? startLookBlend : endLookBlend;
            controller.VirtualCamera.gameObject.SetActive(!enable);

            var vCam = controller.SecondaryVirtualCamera;

            if (enable)
            {
                vCam.transform.position = transform.position;
                vCam.transform.rotation = transform.rotation;
                vCam.m_Lens.FieldOfView = fov > 0 ? fov : 50;
            }

            vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = noise;
            vCam.gameObject.SetActive(enable);
        }
    }
}