using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Player Camera")]
    [SerializeField] private bool _distortCamera;
    [SerializeField] private float _thickDistortLevel = 1;
    [SerializeField] private float _fineDistortLevel = 5;

    private float previousFineDistort;
    private float previousThickDistort;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (_onInteraction && isInteracting)
        {
            Teleport();
        }
        else if (_onInspection && isInteracting)
        {
            Teleport();
        }
        else if (_onInspection && _onInteraction)
        {
            Teleport();
        }
    }

    private void Teleport()
    {
        var player = PlayerController.Instance.Player;
        if (_bounce) StartCoroutine(BounceDelay(player));
        else SetPlayerPositionAndRotation(player, _newPosition, _newRotation);
    }

    private IEnumerator BounceDelay(GameObject player)
    {
        var tvDistortion = PlayerController.Instance.Camera.GetComponent<BadTVEffect>();

        var previousPosition = player.transform.position;
        var previousRotation = player.transform.rotation.eulerAngles;
        SetPlayerPositionAndRotation(player, _newPosition, _newRotation);

        if(_distortCamera)
        {
            previousFineDistort = tvDistortion.fineDistort;
            previousThickDistort = tvDistortion.thickDistort;
            tvDistortion.fineDistort = _fineDistortLevel;
            tvDistortion.thickDistort = _thickDistortLevel;
        }


        yield return new WaitForSeconds(_bounceDelay);
        SetPlayerPositionAndRotation(player, -_newPosition, previousRotation);

        if (_distortCamera)
        {
            tvDistortion.fineDistort = previousFineDistort;
            tvDistortion.thickDistort = previousThickDistort;
        }
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
}