using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace SnowHorse.Systems
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer musicMixer;
        [SerializeField] private float defaultBlendTime = 3f;
        [SerializeField] private AudioSource[] audioSources;

        public static AudioManager Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void ActivateMixerMusic(string snapshotName, float blendTime = 0f)
        {
            var source = audioSources.FirstOrDefault(x => string.Equals($"{snapshotName}_audio_source", x.gameObject.name));

            if (source)
            {
                source.Stop();
                source.Play();
            }

            musicMixer.TransitionToSnapshots(snapshots: new[] { musicMixer.FindSnapshot($"{snapshotName}Snapshot") }, weights: new[] { 1f }, timeToReach: blendTime == 0f ? defaultBlendTime : blendTime);
        }
    }
}