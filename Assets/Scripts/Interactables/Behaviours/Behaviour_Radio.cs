using SnowHorse.Utils;
using UnityEngine;

public class Behaviour_Radio : MonoBehaviour
{
    [SerializeField] private AudioSource _radioSource;
    [SerializeField] private float _volume;

    [SerializeField] private float _transitionToPauseDuration = 1f;
    [SerializeField] private float _transitionToGameDuration = 0.5f;

    private float _currentTime;
    bool _isReadyForStateChange;

    private void Start()
    {
        _radioSource.volume = _volume * GameController.Instance.GlobalVolume;
    }

    void Update()
    {
        if(GameController.Instance != null)
        {
            float percentage;

            if(GameController.Instance.Pause)
            {
                _radioSource.volume = _volume * GameController.Instance.GlobalVolume;

                if(_radioSource.spatialBlend <= 0)
                {
                    percentage = 0;
                    _currentTime = 0;
                    _isReadyForStateChange = false;
                }
                else
                {
                    if(!_isReadyForStateChange)
                    {
                        percentage = 0;
                        _currentTime = 0;
                        _isReadyForStateChange = true;
                    }

                    percentage = Interpolation.Linear(_transitionToPauseDuration, ref _currentTime, true);
                    _radioSource.spatialBlend = 1 - percentage;
                }
            }
            else
            {
                if(_radioSource.spatialBlend >= 1)
                {
                    percentage = 0;
                    _currentTime = 0;
                    _isReadyForStateChange = false;
                }
                else
                {
                    if (!_isReadyForStateChange)
                    {
                        percentage = 0;
                        _currentTime = 0;
                        _isReadyForStateChange = true;
                    }

                    percentage = Interpolation.Linear(_transitionToGameDuration, ref _currentTime, false);
                    _radioSource.spatialBlend = percentage;
                }
            }    
        }
    }
}
