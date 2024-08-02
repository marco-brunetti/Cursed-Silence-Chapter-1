using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_RequireObject : MonoBehaviour, IBehaviour
{
    [SerializeField] private GameObject _requiredObject;
    [SerializeField] private bool _onInteraction = true;
    [SerializeField] private bool _onInspection;

    [SerializeField] private List<GameObject> _successBehaviours = new List<GameObject>();
    [SerializeField] private List<GameObject> _failedBehaviours = new List<GameObject>();

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(_requiredObject != null && _requiredObject == PlayerController.Instance.Inventory.SelectedItem())
        {
            if (_onInteraction && isInteracting)
            {
                ManageSucessBehaviours(isInteracting, false);
            }
            if (_onInspection && isInspecting)
            {
                ManageSucessBehaviours(false, isInspecting);
            }
        }
        else if(_requiredObject == null)
        {
            if (_onInteraction && isInteracting)
            {
                ManageSucessBehaviours(isInteracting, false);
            }
            if (_onInspection && isInspecting)
            {
                ManageSucessBehaviours(false, isInspecting);
            }
        }
        else
        {
            if (_onInteraction && isInteracting)
            {
                ManageFailedBehaviours(isInteracting, false);
            }
            if (_onInspection && isInspecting)
            {
                ManageFailedBehaviours(false, isInspecting);
            }
        }
    }

    private void ManageSucessBehaviours(bool isInteracting, bool isInspecting)
    {
        if (_successBehaviours.Count > 0)
        {
            for (int i = 0; i < _successBehaviours.Count; i++)
            {
                _successBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
            }
        }
    }

        private void ManageFailedBehaviours(bool isInteracting, bool isInspecting)
    {
        if (_failedBehaviours.Count > 0)
        {
            for (int i = 0; i < _failedBehaviours.Count; i++)
            {
                _failedBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
            }
        }
    }

    public bool IsInteractable()
    {
        return _onInteraction;
    }

    public bool IsInspectable()
    {
        return _onInspection;
    }
}
