using UnityEngine;

public class Behaviour_AddStress : MonoBehaviour, IBehaviour
{
    [SerializeField] private bool _onInteraction;
    [SerializeField] private bool _onInspection;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (_onInteraction && isInteracting)
        {
            AddStress();
        }
        else if (_onInspection && isInspecting)
        {
            AddStress();
        }
        else if(!isInteracting && !isInspecting)
        {
            AddStress();
        }
    }

    public bool IsInspectable()
    {
        return _onInspection;
    }

    public bool IsInteractable()
    {
        return _onInteraction;
    }

    private void AddStress()
    {
        PlayerController.Instance.PlayerStress.AddStress();
    }
}
