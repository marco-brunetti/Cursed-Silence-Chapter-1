using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SnowHorse.Systems
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer musicMixer;
        [SerializeField] private float defaultBlendTime = 3f;
        [SerializeField] private AudioSource[] musicSources;
        [SerializeField] private AudioSource playerSource;

        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void PlayMusic(string snapshotName, float blendTime = 0f)
        {
            var source = musicSources.FirstOrDefault(x => string.Equals($"{snapshotName}_audio_source", x.gameObject.name));

            if (source)
            {
                source.Stop();
                source.Play();
            }

            musicMixer.TransitionToSnapshots(snapshots: new[] { musicMixer.FindSnapshot($"{snapshotName}Snapshot") }, weights: new[] { 1f }, timeToReach: blendTime == 0f ? defaultBlendTime : blendTime);
        }

        public void PlayAudio(string id, AudioClip clip, float volume = 1f, float pitch = 1)
        {
            switch(id)
            {
                case "player":
                    playerSource.pitch = pitch;
                    playerSource.PlayOneShot(clip, volume);
                    break;
                default:
                    break;
            }
        }
    }
}