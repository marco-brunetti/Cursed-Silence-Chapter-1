using System;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class InteractablePlayAudioEvent : MonoBehaviour
    {
        public static EventHandler<InteractableAudioEventArgs> InteractablePlayAudio;

        public void Play(AudioClip clip, float volume = 1)
        {
            var args = new InteractableAudioEventArgs("interactable", clip, volume);

            InteractablePlayAudio?.Invoke(this, args);
        }
    }

    public class InteractableAudioEventArgs : EventArgs
    {
        public string Id { get; private set; }
        public AudioClip Clip { get; private set; }
        public float Volume { get; private set; }
        public float Pitch { get; private set; }

        public InteractableAudioEventArgs(string id, AudioClip clip, float volume = 1f, float pitch = 1)
        {
            Id = id;
            Clip = clip;
            Volume = volume;
            Pitch = pitch;
        }
    }
}