using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer musicMixer;
    [SerializeField] private float blendTime = 3f;
    [SerializeField] private AudioSource fightMusicSource;
    [SerializeField] private AudioSource style1MusicSource;
    [SerializeField] private AudioSource style2MusicSource;
    [SerializeField] private AudioSource style3MusicSource;
    [SerializeField] private AudioSource style4MusicSource;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Init()
    {
        
    }

    public void ActivateMixerMusic(string snapshotName, AudioSource source = null, float time = 0f)
    {
        if(source)
        {
            source.Stop();
            source.Play();
        }

        musicMixer.TransitionToSnapshots(snapshots: new[] { musicMixer.FindSnapshot($"{snapshotName}Snapshot") }, weights: new[] { 1f }, timeToReach: time == 0f ? blendTime : time);
    }
}