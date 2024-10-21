using System;
using System.Collections;
using Game.General;
using SnowHorse.Utils;
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

    private GameControllerV2 gameController;
    public static AudioManager Instance;
    private Coroutine modifyFightMusicVolume;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        gameController = GameControllerV2.Instance;
        GameControllerV2.EnemiesActive += OnEnemiesActive;
        GameControllerV2.EnemiesInactive += OnEnemiesInactive;
        GameControllerV2.LayoutStyleChanged += OnLayoutStyleChanged;
    }

    private void OnEnemiesActive(object sender, EventArgs e)
    {
        ActivateMusicStyle("FightSnapshot", null);
    }

    private void OnEnemiesInactive(object sender, EventArgs e)
    {
        ActivateLayoutMusic(gameController.CurrentLayoutStyle);
    }

    private void OnLayoutStyleChanged(object sender, CurrentLayoutStyle e)
    {
        ActivateLayoutMusic(e);
    }

    public void ActivateLayoutMusic(CurrentLayoutStyle style)
    {
        switch(style)
        {
            case CurrentLayoutStyle.Style0:
                ActivateMusicStyle("Style0Snapshot", null);
                break;
            case CurrentLayoutStyle.Style1:
                ActivateMusicStyle("Style1Snapshot", style1MusicSource);
                break;
            case CurrentLayoutStyle.Style2:
                ActivateMusicStyle("Style2Snapshot", style2MusicSource);
                break;
            case CurrentLayoutStyle.Style3:
                ActivateMusicStyle("Style3Snapshot", style3MusicSource);
                break;
            case CurrentLayoutStyle.Style4:
                ActivateMusicStyle("Style4Snapshot", style4MusicSource);
                break;
        }
    }

    private void ActivateMusicStyle(string snapshot, AudioSource source)
    {
        if(source)
        {
            source.Stop();
            source.Play();
        }

        musicMixer.TransitionToSnapshots(snapshots: new[] { musicMixer.FindSnapshot(snapshot) }, weights: new[] { 1f }, timeToReach: blendTime);
    }


    private IEnumerator ModifyFightMusicVolume(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float timeElapsed = 0f;
        var percent = 0f;

        while (percent < 1)
        {
            percent = Interpolation.Exponential(duration, ref timeElapsed);

            source.volume = Mathf.Lerp(startVolume, targetVolume, percent);

            yield return new WaitForSeconds(Time.deltaTime);
            yield return null;
        }

        modifyFightMusicVolume = null;
    }
}