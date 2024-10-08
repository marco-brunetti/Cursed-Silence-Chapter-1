using UnityEngine;
using UnityEngine.Events;

namespace Interactables.Behaviours
{
    public class Behaviour_GenericAction : MonoBehaviour, IBehaviour
    {
        public bool onInteraction;
        public bool onInspection;
        [SerializeField] private bool _deactivateWhenReady = true;
        private UnityAction action;

        private bool _deactivated;

        public void Setup(UnityAction action, bool onInteraction, bool onInspection)
        {
            this.action = action;
            this.onInteraction = onInteraction;
            this.onInspection = onInspection;
            _deactivated = false;
        }

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if (_deactivated) return;

            if (onInteraction && isInteracting) action?.Invoke();
            else if (onInspection && isInteracting) action?.Invoke();
            else if (!onInteraction && !onInspection) action?.Invoke();

            if (_deactivateWhenReady) _deactivated = true;
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
}