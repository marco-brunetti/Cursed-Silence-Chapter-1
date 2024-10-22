using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SnowHorse.Systems
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer musicMixer;
        [SerializeField] private float defaultBlendTime = 3f;
        [SerializeField] private AudioSource[] sourcesList;

        private Dictionary<string, AudioSource> sources = new();

        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Initializes the audio sources by iterating through the sourcesList.
        /// Checks if each audio source's GameObject name contains the "_audio_source" suffix.
        /// Logs an error if the suffix is not present. If the suffix is present, adds the audio
        /// source to the sources dictionary with the prefix of the GameObject name as the key.
        /// </summary>
        public void Init()
        {
            foreach (var source in sourcesList)
            {
                var index = source.gameObject.name.IndexOf("_audio_source");

                if(index == -1)
                {
                    Debug.LogError($"Audio source {source.gameObject.name} does not have an _audio_source suffix. Please rename it.");
                }
                else
                {
                    var key = source.gameObject.name.Substring(0, index);
                    sources.Add(key, source);
                }
            }
        }

        /// <summary>
        /// Play a music snapshot, lowering the volume of any currently playing music in the process.
        /// </summary>
        /// <param name="snapshotName">The name of the music snapshot to play</param>
        /// <param name="blendTime">Optional, the time to take to transition to the new snapshot. Defaults to <see cref="defaultBlendTime"/></param>
        public void PlayMusic(string snapshotName, float blendTime = 0f)
        {
            if (sources.TryGetValue(snapshotName, out var source))
            {
                source.Stop();
                source.Play();
            }

            musicMixer.TransitionToSnapshots(snapshots: new[] { musicMixer.FindSnapshot($"{snapshotName}Snapshot") }, weights: new[] { 1f }, timeToReach: blendTime == 0f ? defaultBlendTime : blendTime);
        }

        /// <summary>
        /// Play an audio clip on the specified source, with optional volume and pitch modification.
        /// </summary>
        /// <param name="id">The id of the source to play the clip on.</param>
        /// <param name="clip">The audio clip to play.</param>
        /// <param name="volume">Optional, the volume to play the clip at. Defaults to 1f.</param>
        /// <param name="pitch">Optional, the pitch to play the clip at. Defaults to 1f.</param>
        public void PlayAudio(string id, AudioClip clip, float volume = 1f, float pitch = 1)
        {
            if (sources.TryGetValue(id, out var source))
            {
                source.pitch = pitch;
                source.PlayOneShot(clip, volume);
            }
            else
            {
                Debug.LogError($"Could not find audio source with id {id}");
            }
        }
    }
}