using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class SwapGameObject : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_activateObject")] [SerializeField] private GameObject activateObject;
        [FormerlySerializedAs("_deactivateObject")] [SerializeField] private GameObject deactivateObject;

        [FormerlySerializedAs("_onInteraction")] [SerializeField] private bool onInteraction;
        [FormerlySerializedAs("_onInspection")] [SerializeField] private bool onInspection;
        [FormerlySerializedAs("_onReleasingInteractable")] [SerializeField] private bool onReleasingInteractable;
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(onInteraction && isInteracting) 
            {
                StartCoroutine(SwapObject());
            }
            else if(onInspection && isInspecting)
            {
                StartCoroutine(SwapObject());
            }
            else if(onReleasingInteractable && !isInteracting && !isInspecting)
            {
                StartCoroutine(SwapObject());
            }
            else if(!onInspection && !onInteraction && !onReleasingInteractable)
            {
                print("Please mark gameobject swap option in " + transform.parent.name + "!");
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

        private IEnumerator SwapObject()
        {
            yield return new WaitUntil(() => !PlayerController.Instance.IsTeleporting);

            if (activateObject != null) activateObject.SetActive(true);
            if (deactivateObject != null) deactivateObject.SetActive(false);
        }
    }
}
