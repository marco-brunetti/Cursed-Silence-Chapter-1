using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Game.General
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject mainMenuObject;
        [SerializeField] private float initialWait = 0.5f;
        [SerializeField] private float endingWait = 1f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            videoPlayer.Prepare();
            StartCoroutine(PlayVideoLogo());
        }

        private IEnumerator PlayVideoLogo()
        {
            yield return new WaitUntil(() => videoPlayer.isPrepared);

            yield return new WaitForSeconds(initialWait);
            
            videoPlayer.Play();

            yield return new WaitForSeconds((float)videoPlayer.clip.length + endingWait);

            mainMenuObject.SetActive(true);
            videoPlayer.gameObject.SetActive(false);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
