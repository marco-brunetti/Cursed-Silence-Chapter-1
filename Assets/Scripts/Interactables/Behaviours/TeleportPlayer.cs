using System.Collections;
using Game.General;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class TeleportPlayer : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_delay")] [SerializeField] private float delay = 0;
        [SerializeField] private bool toDream;

        [FormerlySerializedAs("_onInteraction")]
        [Header("Interactable properties")]
        [SerializeField] private bool onInteraction;
        [FormerlySerializedAs("_onInspection")] [SerializeField] private bool onInspection;

        [FormerlySerializedAs("_teleportRelative")]
        [Header("Teleportation")]
        [SerializeField] private bool teleportRelative = true;
        [FormerlySerializedAs("_maintainRotation")] [SerializeField] private bool maintainRotation = true;
        [FormerlySerializedAs("_newPosition")] [SerializeField] private Vector3 newPosition;
        [FormerlySerializedAs("_newRotation")] [SerializeField] private Vector3 newRotation;

        [FormerlySerializedAs("_bounce")]
        [Header("Teleport the player back to origin")]
        [SerializeField] private bool bounce = false;
        [FormerlySerializedAs("_bounceDelay")] [SerializeField] private float bounceDelay = 0.5f;

        [FormerlySerializedAs("_distortCamera")]
        [Header("Teleport Camera")]
        [SerializeField] private bool distortCamera;
        [FormerlySerializedAs("_activateDarkMask")] [SerializeField] private bool activateDarkMask;
        [FormerlySerializedAs("_blackMaskDuration")] [SerializeField] private float blackMaskDuration = 0.1f;

        [FormerlySerializedAs("_teleportClip")]
        [Header("Teleport Audio")]
        [SerializeField] private AudioClip teleportClip; //add clip for bounce later
        [FormerlySerializedAs("_teleportClipVolume")] [SerializeField] private float teleportClipVolume = 1;

        private bool _isTeleporting;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if (_isTeleporting) return;

            if (onInteraction && isInteracting) StartCoroutine(Teleport());
            else if (onInspection && isInteracting) StartCoroutine(Teleport());
            else if (onInspection && onInteraction) StartCoroutine(Teleport());
        }

        private IEnumerator Teleport()
        {
            _isTeleporting = true;

            PlayerController playerController = PlayerController.Instance;

            if (toDream) AudioListener.volume = 0;
            else AudioListener.volume = 1;

            if (delay != 0) yield return new WaitForSecondsRealtime(delay);

            if(activateDarkMask) UIManager.Instance.ActivateDarkMask(true);

            playerController.IsTeleporting = true;

            if (teleportClip) GameController.Instance.GeneralAudioSource.PlayOneShot(teleportClip, teleportClipVolume);

            var player = playerController.Player;
            var playerData = playerController.PlayerData;
            var tvDistortion = playerController.Camera.GetComponent<BadTVEffect>();

            tvDistortion.fineDistort = distortCamera ? playerData.MaxFineDistortion : playerData.DefaultFineDistortion;
            tvDistortion.thickDistort = distortCamera ? playerData.MaxthickDistortion : playerData.DefaultThickDistortion;

            if (bounce) yield return StartCoroutine(BounceDelay(player, playerData, tvDistortion));
            else SetPlayerPositionAndRotation(player, newPosition, newRotation);

            if (activateDarkMask)
            {
                yield return new WaitForSecondsRealtime(blackMaskDuration);
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
            SetPlayerPositionAndRotation(player, newPosition, newRotation);

            yield return new WaitForSecondsRealtime(bounceDelay);

            SetPlayerPositionAndRotation(player, -newPosition, previousRotation);

            //return defaults after bounce
            tvDistortion.fineDistort = distortCamera ? playerData.DefaultFineDistortion : playerData.MaxFineDistortion;
            tvDistortion.thickDistort = distortCamera ? playerData.DefaultThickDistortion : playerData.MaxthickDistortion;
        }

        private void SetPlayerPositionAndRotation(GameObject player, Vector3 position, Vector3 rotation)
        {
            if (teleportRelative) player.transform.position += position;
            else player.transform.position = position;

            if (!maintainRotation) player.transform.rotation = Quaternion.Euler(rotation);
        }

        public bool IsInspectable()
        {
            return onInspection;
        }

        public bool IsInteractable()
        {
            return onInteraction;
        }

        private void OnDisable()
        {
            _isTeleporting = false;
            PlayerController.Instance.IsTeleporting = false;
        }
    }
}