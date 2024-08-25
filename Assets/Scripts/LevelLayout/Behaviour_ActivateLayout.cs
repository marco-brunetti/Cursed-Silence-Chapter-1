using UnityEngine;

public class Behaviour_ActivateLayout : MonoBehaviour, IBehaviour
{
    [SerializeField] private int _layoutId;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private LevelDecorator[] _decorators;
    [SerializeField] private bool _deactivateWhenReady = true;
    [SerializeField] private bool _onInteraction;
    [SerializeField] private bool _onInspection;

    private bool _deactivated;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (_deactivated) return;
        //LevelLayoutManager.Instance.ActivateLayout(_layoutId, transform.position + _position, Quaternion.Euler(_rotation), _decorators);
        if(_deactivateWhenReady) _deactivated = true;
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