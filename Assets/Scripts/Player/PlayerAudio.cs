using UnityEngine;
using Player;
using System;

public class PlayerAudio : MonoBehaviour
{
    private int _heartbeatIndex = 0, _breathIndex = 0, currentFootstepIndex = 0;
    private float _footStepTimer, _breathingTimer = 0, _heartbeatTimer = 0;

    public static EventHandler<string> PlayAudio;

    public void PlayerAudioControl(PlayerData playerData, IPlayerInput playerInput)
    {
        PlayerFootsteps(playerData, playerInput);
        //PlayerHeartbeat(playerData);
        PlayerBreath(playerData);
    }


    private void PlayerFootsteps(PlayerData playerData, IPlayerInput playerInput)
    {
        if (playerInput.UnsmoothedPlayerMovementInput != Vector2.zero)
        {
            if (_footStepTimer <= 0)
            {
                int i;

                do i = UnityEngine.Random.Range(0, playerData.GrassFootstepClips.Length);
                while (i == currentFootstepIndex);

                    var volume = playerInput.playerRunInput ? playerData.GrassFootstepClipsVolume : playerData.GrassFootstepClipsVolume * 0.4f;
                    
                    PlayAudio?.Invoke(this, "footstep");

                    currentFootstepIndex = i;

                _footStepTimer = playerInput.playerRunInput ? playerData.FootstepsRunningTime : playerData.FootstepWalkingTime;
            }

            _footStepTimer -= Time.deltaTime;
        }
        else 
        {
            _footStepTimer = 0;
        }
    }

    private void PlayerBreath(PlayerData playerData)
    {
        if (_breathingTimer <= 0)
        {
            float currentStressLevel = PlayerController.Instance.PlayerStress.StressLevel();
            float volume = playerData.PlayerBreathClipsVolume * currentStressLevel;
            
            PlayAudio?.Invoke(this, "breath");

            //float pitch = 0.8f + (currentStressLevel / 12.5f); //Minimum pitch is 0.8; To get to maximum of 1, sum 0.2 when maximum stress level of 2.5f;
            //AudioManager.Instance.PlayAudio("player", playerData.PlayerBreathClips[_breathIndex], volume/*, pitch*/);

            var isEven = _breathIndex == 0 ? true : _breathIndex % 2 == 0;
            if(isEven) _breathingTimer += playerData.BreathingMinimumRate / currentStressLevel;
            else _breathingTimer += (playerData.BreathingMinimumRate / currentStressLevel) * 2;
            
            _breathIndex = _breathIndex < playerData.PlayerBreathClips.Length - 1 ? _breathIndex++ : 0;
        }

        _breathingTimer -= Time.deltaTime;
    }

    private void PlayerHeartbeat(PlayerData playerData)
    {
        if (_heartbeatTimer <= 0)
        {
            float currentStressLevel = PlayerController.Instance.PlayerStress.StressLevel();
            float volume = playerData.PlayerHeartbeatClipsVolume * currentStressLevel;

            PlayAudio?.Invoke(this, "heartbeat");

            //AudioManager.Instance.PlayAudio("player", playerData.PlayerHeartbeatClips[_heartbeatIndex], volume);

            //Heartbeat rate isn't equal; the first beat and the second are close together, the second and third are not. So we check if index is even to decide wait time.
            var isEven = _heartbeatIndex == 0 ? true : _heartbeatIndex % 2 == 0;
            if(isEven) _heartbeatTimer += playerData.HeartbeatMinimumRate / currentStressLevel;
            else _heartbeatTimer += (playerData.HeartbeatMinimumRate / currentStressLevel) * 1.5f;

            _heartbeatIndex = _heartbeatIndex < playerData.PlayerHeartbeatClips.Length - 1 ? _heartbeatIndex++ : 0;
        }

        _heartbeatTimer -= Time.deltaTime;
    }

    
}