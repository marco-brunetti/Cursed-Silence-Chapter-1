using System.Collections;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_PlayAudio : MonoBehaviour, IBehaviour
    {
        [SerializeField] private float _delay = 0f;
        [SerializeField] private AudioClip _audioClip;

        [SerializeField] private bool _onInteraction;
        [SerializeField] private bool _onInspection;
        [SerializeField] private bool _playOnce;

        [Header("If audiosource is null, will create new")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _volume = 0.5f;
        [SerializeField, Range(0, 1)] private float _3DBlend;
        [SerializeField, Range(0, 1)] private float _pitch = 1;

        private bool _alreadyPlayed;

        private void Awake()
        {
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();

            _audioSource.spatialBlend = _3DBlend;
            _audioSource.pitch = _pitch;
        }

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            StartCoroutine(ManageAudioDelay(_delay, isInteracting, isInspecting));
        }

        private IEnumerator ManageAudioDelay(float delay, bool isInteracting, bool isInspecting)
        {
            yield return new WaitForSecondsRealtime(delay);

            if(_onInteraction && isInteracting)
            {
                ManageAudioRepetition();
            }
            else if(_onInspection && isInspecting)
            {
                ManageAudioRepetition();
            }
            else
            {
                ManageAudioRepetition();
            }
        
        }


        private void ManageAudioRepetition()
        {
            if (_playOnce)
            {
                if (!_alreadyPlayed)
                {
                    PlayAudio();
                    _alreadyPlayed = true;
                }
            }
            else
            {
                PlayAudio();
            }
        }

        private void PlayAudio()
        {
            _audioSource.PlayOneShot(_audioClip, _volume * GameController.Instance.GlobalVolume);
        }

        public bool IsInteractable()
        {
            return _onInteraction;
        }

        public bool IsInspectable()
        {
            return _onInspection;
        }
    }
}
