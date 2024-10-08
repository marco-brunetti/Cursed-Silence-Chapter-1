using UnityEngine;

namespace Interactables.Behaviours
{
    public interface IBehaviour
    {
        void Behaviour(bool isInteracting, bool isInspecting);
        bool IsInteractable();
        bool IsInspectable();
        GameObject gameObject { get; }
    }
}