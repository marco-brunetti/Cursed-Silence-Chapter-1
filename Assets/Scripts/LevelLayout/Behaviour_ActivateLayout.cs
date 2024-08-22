using UnityEngine;

public class Behaviour_ActivateLayout : MonoBehaviour, IBehaviour
{
    [SerializeField] private int _layoutId;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private RoomDecorator[] _decorators;
    [SerializeField] private bool _onInteraction;
    [SerializeField] private bool _onInspection;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        LevelLayoutManager.Instance.ActivateLayout(_layoutId, transform.position + _position, Quaternion.Euler(transform.rotation.eulerAngles + _rotation), _decorators);
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