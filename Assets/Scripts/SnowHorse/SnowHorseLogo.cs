using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SnowHorseLogo : MonoBehaviour
{
    [SerializeField] private float _delay = 0.5f;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private VideoClip _videoClip;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        StartCoroutine(PlayVideoLogo());
    }

    IEnumerator PlayVideoLogo()
    {
        yield return new WaitForSeconds(_delay);
        _videoPlayer.Play();
        yield return new WaitForSeconds((float) _videoClip.length);
        SceneManager.LoadScene("Main_Menu");
    }
}
