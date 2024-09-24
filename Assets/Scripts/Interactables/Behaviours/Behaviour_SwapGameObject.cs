using System.Collections;
using Player;
using UnityEngine;

namespace Interactables.Behaviours
{
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
                StartCoroutine(SwapObject());
            }
            else if(_onInspection && isInspecting)
            {
                StartCoroutine(SwapObject());
            }
            else if(_onReleasingInteractable && !isInteracting && !isInspecting)
            {
                StartCoroutine(SwapObject());
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

        private IEnumerator SwapObject()
        {
            yield return new WaitUntil(() => !PlayerController.Instance.IsTeleporting);

            if (_activateObject != null) _activateObject.SetActive(true);
            if (_deactivateObject != null) _deactivateObject.SetActive(false);
        }
    }
}
