using UnityEngine;

namespace Interactables.Behaviours
{
    public abstract class Behaviour : MonoBehaviour
    {
        public abstract BehaviourType Type { get; }

        public abstract void Activate();
    }
    
    public enum BehaviourType
    {
        Inspectable,
        Interactable
    }
}