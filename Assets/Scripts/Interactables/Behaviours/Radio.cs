using Game.General;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class Radio : MonoBehaviour
    {
        [FormerlySerializedAs("_radioSource")] [SerializeField] private AudioSource radioSource;
        [FormerlySerializedAs("_volume")] [SerializeField] private float volume;

        [FormerlySerializedAs("_transitionToPauseDuration")] [SerializeField] private float transitionToPauseDuration = 1f;
        [FormerlySerializedAs("_transitionToGameDuration")] [SerializeField] private float transitionToGameDuration = 0.5f;

        private float _currentTime;
        bool _isReadyForStateChange;

        private void Start()
        {
            radioSource.volume = volume * GameController.Instance.GlobalVolume;
        }

        void Update()
        {
            if(GameController.Instance != null)
            {
                float percentage;

                if(GameController.Instance.Pause)
                {
                    radioSource.volume = volume * GameController.Instance.GlobalVolume;

                    if(radioSource.spatialBlend <= 0)
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

                        percentage = Interpolation.Linear(transitionToPauseDuration, ref _currentTime, true);
                        radioSource.spatialBlend = 1 - percentage;
                    }
                }
                else
                {
                    if(radioSource.spatialBlend >= 1)
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

                        percentage = Interpolation.Linear(transitionToGameDuration, ref _currentTime, false);
                        radioSource.spatialBlend = percentage;
                    }
                }    
            }
        }
    }
}
