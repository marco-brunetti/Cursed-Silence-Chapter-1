using System.Collections;
using Game.General;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class TV : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_opaqueScreen")] [SerializeField] private GameObject opaqueScreen;
        [FormerlySerializedAs("_videoScreen")] [SerializeField] private GameObject videoScreen;
        [FormerlySerializedAs("_tvSource")] [SerializeField] private AudioSource tvSource;
        [FormerlySerializedAs("_volume")] [SerializeField] private float volume = 0.5f;
        [FormerlySerializedAs("VideoDuration")] public float videoDuration = 10f;

        private bool _isPlaying;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(!_isPlaying && isInteracting || !_isPlaying && isInspecting)
            {
                tvSource.volume = volume * GameController.Instance.GlobalVolume;
                StartCoroutine(ManangeVideoPlayback());
            }
        }

        private IEnumerator ManangeVideoPlayback()
        {
            TurnScreenOn(true);
            yield return new WaitForSeconds(videoDuration);
            TurnScreenOn(false);
        }

        private void TurnScreenOn(bool enable)
        {
            _isPlaying = enable;
            videoScreen.SetActive(enable);
            opaqueScreen.SetActive(!enable);
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
}
