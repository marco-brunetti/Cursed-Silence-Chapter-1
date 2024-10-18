using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Game.General
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject mainMenuObject;
    
        private readonly WaitForSeconds initialWait = new(0.5f);
        private WaitForSeconds clipDurationWait;
        private VideoClip videoClip;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            StartCoroutine(PlayVideoLogo());
            videoClip = videoPlayer.clip;
            clipDurationWait = new WaitForSeconds((float)videoClip.length);
        }

        private IEnumerator PlayVideoLogo()
        {
            yield return initialWait;
            videoPlayer.Play();
            yield return clipDurationWait;
            mainMenuObject.SetActive(true);
            videoPlayer.gameObject.SetActive(false);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
