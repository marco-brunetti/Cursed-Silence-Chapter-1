using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class PlayAudio : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_delay")] [SerializeField] private float delay = 0f;
        [FormerlySerializedAs("_audioClip")] [SerializeField] private AudioClip audioClip;

        [FormerlySerializedAs("_onInteraction")] [SerializeField] private bool onInteraction;
        [FormerlySerializedAs("_onInspection")] [SerializeField] private bool onInspection;
        [FormerlySerializedAs("_playOnce")] [SerializeField] private bool playOnce;

        [FormerlySerializedAs("_audioSource")]
        [Header("If audiosource is null, will create new")]
        [SerializeField] private AudioSource audioSource;
        [FormerlySerializedAs("_volume")] [SerializeField] private float volume = 0.5f;
        [SerializeField, Range(0, 1)] private float _3DBlend;
        [FormerlySerializedAs("_pitch")] [SerializeField, Range(0, 1)] private float pitch = 1;

        private bool _alreadyPlayed;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.spatialBlend = _3DBlend;
            audioSource.pitch = pitch;
        }

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            StartCoroutine(ManageAudioDelay(delay, isInteracting, isInspecting));
        }

        private IEnumerator ManageAudioDelay(float delay, bool isInteracting, bool isInspecting)
        {
            yield return new WaitForSecondsRealtime(delay);

            if(onInteraction && isInteracting)
            {
                ManageAudioRepetition();
            }
            else if(onInspection && isInspecting)
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
            if (playOnce)
            {
                if (!_alreadyPlayed)
                {
                    Play();
                    _alreadyPlayed = true;
                }
            }
            else
            {
                Play();
            }
        }

        private void Play()
        {
            audioSource.PlayOneShot(audioClip, volume * GameController.Instance.GlobalVolume);
        }

        public bool IsInteractable()
        {
            return onInteraction;
        }

        public bool IsInspectable()
        {
            return onInspection;
        }
    }
}
