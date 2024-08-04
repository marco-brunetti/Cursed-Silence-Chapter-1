using UnityEngine;
using UnityEngine.SceneManagement;

public interface IPlayerAudio
{
    void PlayerAudioControl(PlayerData playerData, IPlayerInput playerInput);
}

public class PlayerAudio : MonoBehaviour, IPlayerAudio
{
    [SerializeField] private AudioSource _heartbeatSource;
    [SerializeField] private AudioSource _breathSource;
    [SerializeField] private AudioSource _footstepsSource;
    private int _currentHeartbeatIndex = 0, _currentBreathIndex = 0, currentFootstepIndex = 0;
    private float _footStepTimer, _breathingTimer = 0, _heartbeatTimer = 0;

    public void PlayerAudioControl(PlayerData playerData, IPlayerInput playerInput)
    {
        PlayerFootsteps(playerData, playerInput);
        PlayerHeartbeat(playerData);
        PlayerBreath(playerData);
        AmbienceSound();
    }


    private void PlayerFootsteps(PlayerData playerData, IPlayerInput playerInput)
    {

        if(playerInput.UnsmoothedPlayerMovementInput != Vector2.zero)
        {
            if(/*_playerBreathSource != null &&*/ _footStepTimer <= 0)
            {
                int i = 0;

                if(PlayerController.Instance.IsOutside)
                {
                    do { i = Random.Range(0, playerData.GrassFootstepClips.Length); }
                    while (i == currentFootstepIndex);

                    var volume = playerInput.playerRunInput ? playerData.GrassFootstepClipsVolume : playerData.GrassFootstepClipsVolume * 0.4f;

                    _footstepsSource.PlayOneShot(playerData.GrassFootstepClips[i], volume * GameController.Instance.GlobalVolume);
                    currentFootstepIndex = i;
                }
                else
                {
                    do { i = Random.Range(0, playerData.WoodFootstepClips.Length); }
                    while (i == currentFootstepIndex);

                    var volume = playerInput.playerRunInput ? playerData.WoodFootstepClipsVolume : playerData.WoodFootstepClipsVolume * 0.4f;

                    _footstepsSource.PlayOneShot(playerData.WoodFootstepClips[i], volume * GameController.Instance.GlobalVolume);
                    currentFootstepIndex = i;
                }

                if (playerInput.playerRunInput) _footStepTimer = playerData.FootstepsRunningTime;
                else _footStepTimer = playerData.FootstepWalkingTime;
            }
            _footStepTimer -= Time.deltaTime;
        }
        else _footStepTimer = 0;
    }

    private void PlayerHeartbeat(PlayerData playerData)
    {
        if (SceneManager.GetActiveScene().name == "Level_Dream")
        {
            if (_heartbeatTimer <= 0)
            {
                float currentStressLevel = PlayerController.Instance.PlayerStress.StressLevel();
                float volume = playerData.PlayerHeartbeatClipsVolume * currentStressLevel;

                _heartbeatSource.PlayOneShot(playerData.PlayerHeartbeatClips[_currentHeartbeatIndex], volume * GameController.Instance.GlobalVolume);

                //Heartbeat rate isn't equal; the first beat and the second are close together, the second and third are not. So we check if i is even to decide wait time.
                if (_currentHeartbeatIndex == 0 || _currentHeartbeatIndex == 2 || _currentHeartbeatIndex == 4)
                    _heartbeatTimer += playerData.HeartbeatMinimumRate / currentStressLevel;
                if (_currentHeartbeatIndex == 1 || _currentHeartbeatIndex == 3 || _currentHeartbeatIndex == 5)
                    _heartbeatTimer += (playerData.HeartbeatMinimumRate / currentStressLevel) * 1.5f;

                //Loops from 0 to heatbeat clips length
                if (_currentHeartbeatIndex < playerData.PlayerHeartbeatClips.Length - 1)
                    _currentHeartbeatIndex++;
                else _currentHeartbeatIndex = 0;
            }

            _heartbeatTimer -= Time.deltaTime;
        }  
    }

    private void PlayerBreath(PlayerData playerData)
    {
        if (SceneManager.GetActiveScene().name == "Level_House") //Level_House
        {
            if (_breathingTimer <= 0)
            {
                float currentStressLevel = PlayerController.Instance.PlayerStress.StressLevel();
                float volume = playerData.PlayerBreathClipsVolume * currentStressLevel;

                _breathSource.pitch = 0.8f + (currentStressLevel / 12.5f); //Minimum pitch is 0.8; To get to maximum of 1, sum 0.2 when maximum stress level of 2.5f;
                _breathSource.PlayOneShot(playerData.PlayerBreathClips[_currentBreathIndex], volume * GameController.Instance.GlobalVolume);

                if (_currentBreathIndex == 0 || _currentBreathIndex == 2 || _currentBreathIndex == 4)
                    _breathingTimer += playerData.BreathingMinimumRate / currentStressLevel;
                if (_currentBreathIndex == 1 || _currentBreathIndex == 3 || _currentBreathIndex == 5)
                    _breathingTimer += (playerData.BreathingMinimumRate / currentStressLevel) * 2;


                //Loops from 0 to heatbeat clips length
                if (_currentBreathIndex < playerData.PlayerBreathClips.Length - 1)
                    _currentBreathIndex++;
                else _currentBreathIndex = 0;
            }

            _breathingTimer -= Time.deltaTime;
        }
    }

    private void AmbienceSound()
    {
        if (PlayerController.Instance.IsOutside) GameController.Instance.ActivateAmbienceSounds(true);
        else GameController.Instance.ActivateAmbienceSounds(false);
    }
}