using System.Collections;
using UnityEngine;

public class Behaviour_TeleportPlayer : MonoBehaviour, IBehaviour
{
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
    [SerializeField] private float _teleportClipVolume;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (_onInteraction && isInteracting) StartCoroutine(Teleport());
        else if (_onInspection && isInteracting) StartCoroutine(Teleport());
        else if (_onInspection && _onInteraction) StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        if(_activateDarkMask) StartCoroutine(ActivateBlackMask());

        if (_teleportClip) GameController.Instance.GeneralAudioSource.PlayOneShot(_teleportClip, _teleportClipVolume);

        PlayerController playerController = PlayerController.Instance;
        var player = playerController.Player;
        var playerData = playerController.PlayerData;
        var tvDistortion = playerController.Camera.GetComponent<BadTVEffect>();

        playerController.IsTeleporting = true;

        tvDistortion.fineDistort = _distortCamera ? playerData.MaxFineDistortion : playerData.DefaultFineDistortion;
        tvDistortion.thickDistort = _distortCamera ? playerData.MaxthickDistortion : playerData.DefaultThickDistortion;

        if (_bounce) StartCoroutine(BounceDelay(player, playerData, tvDistortion));
        else SetPlayerPositionAndRotation(player, _newPosition, _newRotation);

        yield return new WaitForEndOfFrame();

        playerController.IsTeleporting = false;
    }

    private IEnumerator ActivateBlackMask()
    {
        UIManager.Instance.ActivateDarkMask(true);
        yield return new WaitForSecondsRealtime(_blackMaskDuration);
        UIManager.Instance.ActivateDarkMask(false);
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
        PlayerController.Instance.IsTeleporting = false;
    }
}