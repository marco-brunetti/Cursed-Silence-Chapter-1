using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_TV : MonoBehaviour, IBehaviour
{
    [SerializeField] private GameObject _opaqueScreen;
    [SerializeField] private GameObject _videoScreen;
    [SerializeField] private AudioSource _tvSource;
    [SerializeField] private float _volume = 0.5f;
    public float VideoDuration = 10f;

    private bool _isPlaying;
    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(!_isPlaying && isInteracting || !_isPlaying && isInspecting)
        {
            _tvSource.volume = _volume * GameController.Instance.GlobalVolume;
            StartCoroutine(ManangeVideoPlayback());
        }
    }

    private IEnumerator ManangeVideoPlayback()
    {
        TurnScreenOn(true);
        yield return new WaitForSeconds(VideoDuration);
        TurnScreenOn(false);
    }

    private void TurnScreenOn(bool enable)
    {
        _isPlaying = enable;
        _videoScreen.SetActive(enable);
        _opaqueScreen.SetActive(!enable);
    }

    public bool IsInteractable()
    {
        return true;
    }

    public bool IsInspectable()
    {
        return false;
    }
}
