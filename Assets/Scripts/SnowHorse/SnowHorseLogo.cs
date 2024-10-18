using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class SnowHorseLogo : MonoBehaviour
{
    [SerializeField] private float _delay = 0.5f;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private VideoClip _videoClip;
    [SerializeField] private GameObject _mainMenuObject;


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
        _mainMenuObject.SetActive(true);
        _videoPlayer.gameObject.SetActive(false);
        //SceneManager.LoadScene("Main_Menu");
    }
}
