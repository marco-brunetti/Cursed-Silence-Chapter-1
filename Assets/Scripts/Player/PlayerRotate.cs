using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerRotate
{
    void Rotate(PlayerData playerData, PlayerInput input, bool freezeRotation);
}

public class PlayerRotate : MonoBehaviour, IPlayerRotate
{
    private CinemachinePOV _cinemachinePOV;

    public void Rotate(PlayerData playerData, PlayerInput input, bool freezeRotation)
    {
        if (_cinemachinePOV == null) _cinemachinePOV = PlayerController.Instance.VirtualCamera.GetCinemachineComponent<CinemachinePOV>();

        if(input.mouseMovementInput.magnitude > 0 && !freezeRotation && (GameController.Instance == null || !GameController.Instance.Pause))
        {
            _cinemachinePOV.m_HorizontalAxis.m_DecelTime = 0.1f;
            _cinemachinePOV.m_VerticalAxis.m_DecelTime = 0.1f;

            float multiplier = 1;

            if(GameController.Instance != null)
            {
                multiplier = GameController.Instance.MouseSensibilityMultiplier;
            }
            else
            {
                multiplier = 1;
            }

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
