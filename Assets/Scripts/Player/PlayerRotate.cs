using Cinemachine;
using UnityEngine;

namespace Player
{
    public interface IPlayerRotate
    {
        void Rotate(PlayerData playerData, PlayerInput input, bool freezeRotation);
    }

    public class PlayerRotate : MonoBehaviour, IPlayerRotate
    {
        private CinemachinePOV _cinemachinePOV;

        public void Rotate(PlayerData playerData, PlayerInput input, bool freezeRotation)
        {
            if (!_cinemachinePOV) _cinemachinePOV = PlayerController.Instance.VirtualCamera.GetCinemachineComponent<CinemachinePOV>();

            if (input.mouseMovementInput.magnitude > 0 && !freezeRotation && (!GameController.Instance || !GameController.Instance.Pause))
            {
                _cinemachinePOV.m_HorizontalAxis.m_DecelTime = 0.1f;
                _cinemachinePOV.m_VerticalAxis.m_DecelTime = 0.1f;

                var multiplier = 1f;

                if (GameController.Instance) multiplier = GameController.Instance.MouseSensibilityMultiplier;

                _cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = playerData.MouseSensitivityX * multiplier;
                _cinemachinePOV.m_VerticalAxis.m_MaxSpeed = playerData.MouseSensitivityY * multiplier;
            }
            else
            {
                _cinemachinePOV.m_HorizontalAxis.m_DecelTime = 0f;
                _cinemachinePOV.m_VerticalAxis.m_DecelTime = 0f;
                _cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = 0f;
                _cinemachinePOV.m_VerticalAxis.m_MaxSpeed = 0f;
            }
        }
    }
}