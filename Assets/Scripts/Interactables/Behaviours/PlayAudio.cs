using UnityEngine;

namespace Interactables.Behaviours
{
    public class PlayAudio : Behaviour
    {
        [SerializeField] private string reference; //Used for inspector ref only
        [SerializeField] private new InteractablePlayAudioEvent audio;
        [SerializeField] private AudioClip clip;
        [SerializeField] private float volume;
        public override void Activate() => audio.Play(clip, volume);
    }
}