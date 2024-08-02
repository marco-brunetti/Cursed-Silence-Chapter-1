using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_SwapGameObject : MonoBehaviour, IBehaviour
{
    [SerializeField] private GameObject _activateObject;
    [SerializeField] private GameObject _deactivateObject;

    [SerializeField] private bool _onInteraction;
    [SerializeField] private bool _onInspection;
    [SerializeField] private bool _onReleasingInteractable;
    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(_onInteraction && isInteracting) 
        {
            SwapObject();
        }
        else if(_onInspection && isInspecting)
        {
            SwapObject();
        }
        else if(_onReleasingInteractable && !isInteracting && !isInspecting)
        {
            SwapObject();
        }
        else if(!_onInspection && !_onInteraction && !_onReleasingInteractable)
        {
            print("Please mark gameobject swap option in " + transform.parent.name + "!");
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

    private void SwapObject()
    {
        if (_activateObject != null) _activateObject.SetActive(true);
        if (_deactivateObject != null) _deactivateObject.SetActive(false);
    }
}
