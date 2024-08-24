using UnityEngine;
using UnityEngine.Events;

public class Behaviour_GenericAction : MonoBehaviour, IBehaviour
{
    public bool onInteraction;
    public bool onInspection;
    private UnityAction action;

    public void Setup(UnityAction action, bool onInteraction, bool onInspection)
    {
        this.action = action;
        this.onInteraction = onInteraction;
        this.onInspection = onInspection;
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(onInteraction && isInteracting)
        {
            action?.Invoke();
        }
        else if (onInspection && isInteracting)
        {
            action?.Invoke();
        }
        else if(!onInteraction && !onInspection)
        {
            action?.Invoke();
        }
    }

    public bool IsInspectable()
    {
        return onInspection;
    }

    public bool IsInteractable()
    {
        return onInteraction;
    }
}
