using System.Collections;
using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_TeleportPlayer : MonoBehaviour, IBehaviour
    {
        [SerializeField] private float _delay = 0;
        [SerializeField] private bool toDream;

        [Header("Interactable properties")]
        [SerializeField] private bool _onInteraction;
        [SerializeField] private bool _onInspection;

        [Header("Teleportation")]
        [SerializeField] private bool _teleportRelative = true;
        [SerializeField] private bool _maintainRotation = true;
        [SerializeField] private Vector3 _newPosition;
        [SerializeField] private Vector3 _newRotation;

        [Header("Teleport the player back to origin")]
        [SerializeField] private bool _bounce = false;
        [SerializeField] private float _bounceDelay = 0.5f;

        [Header("Teleport Camera")]
        [SerializeField] private bool _distortCamera;
        [SerializeField] private bool _activateDarkMask;
        [SerializeField] private float _blackMaskDuration = 0.1f;

        [Header("Teleport Audio")]
        [SerializeField] private AudioClip _teleportClip; //add clip for bounce later
        [SerializeField] private float _teleportClipVolume = 1;

        private bool _isTeleporting;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if (_isTeleporting) return;

            if (_onInteraction && isInteracting) StartCoroutine(Teleport());
            else if (_onInspection && isInteracting) StartCoroutine(Teleport());
            else if (_onInspection && _onInteraction) StartCoroutine(Teleport());
        }

        private IEnumerator Teleport()
        {
            _isTeleporting = true;

            PlayerController playerController = PlayerController.Instance;

            if (toDream) AudioListener.volume = 0;
            else AudioListener.volume = 1;

            if (_delay != 0) yield return new WaitForSecondsRealtime(_delay);

            if(_activateDarkMask) UIManager.Instance.ActivateDarkMask(true);

            playerController.IsTeleporting = true;

            if (_teleportClip) GameController.Instance.GeneralAudioSource.PlayOneShot(_teleportClip, _teleportClipVolume);

            var player = playerController.Player;
            var playerData = playerController.PlayerData;
            var tvDistortion = playerController.Camera.GetComponent<BadTVEffect>();

            tvDistortion.fineDistort = _distortCamera ? playerData.MaxFineDistortion : playerData.DefaultFineDistortion;
            tvDistortion.thickDistort = _distortCamera ? playerData.MaxthickDistortion : playerData.DefaultThickDistortion;

            if (_bounce) yield return StartCoroutine(BounceDelay(player, playerData, tvDistortion));
            else SetPlayerPositionAndRotation(player, _newPosition, _newRotation);

            if (_activateDarkMask)
            {
                yield return new WaitForSecondsRealtime(_blackMaskDuration);
                UIManager.Instance.ActivateDarkMask(false);
            } 
            else yield return new WaitForEndOfFrame();

            _isTeleporting = false;
            playerController.IsTeleporting = false;
            GameController.Instance.IsInDream = toDream;
        }

        private IEnumerator BounceDelay(GameObject player, PlayerData playerData, BadTVEffect tvDistortion)
        {
            var previousPosition = player.transform.position;
            var previousRotation = player.transform.rotation.eulerAngles;
            SetPlayerPositionAndRotation(player, _newPosition, _newRotation);

            yield return new WaitForSecondsRealtime(_bounceDelay);

            SetPlayerPositionAndRotation(player, -_newPosition, previousRotation);

            //return defaults after bounce
            tvDistortion.fineDistort = _distortCamera ? playerData.DefaultFineDistortion : playerData.MaxFineDistortion;
            tvDistortion.thickDistort = _distortCamera ? playerData.DefaultThickDistortion : playerData.MaxthickDistortion;
        }

        private void SetPlayerPositionAndRotation(GameObject player, Vector3 position, Vector3 rotation)
        {
            if (_teleportRelative) player.transform.position += position;
            else player.transform.position = position;

            if (!_maintainRotation) player.transform.rotation = Quaternion.Euler(rotation);
        }

        public bool IsInspectable()
        {
            return _onInspection;
        }

        public bool IsInteractable()
        {
            return _onInteraction;
        }

        private void OnDisable()
        {
            _isTeleporting = false;
            PlayerController.Instance.IsTeleporting = false;
        }
    }
}