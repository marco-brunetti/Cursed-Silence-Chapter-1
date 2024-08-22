using UnityEngine;

public class Behaviour_SetCurrentLayout : MonoBehaviour, IBehaviour
{
    [SerializeField] private LevelLayout _currentLayout;
    [SerializeField] private bool _onInteraction;
    [SerializeField] private bool _onInspection;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        LevelLayoutManager.Instance.SetCurrentLayout(_currentLayout);
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