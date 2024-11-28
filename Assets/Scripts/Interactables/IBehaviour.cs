using UnityEngine;

namespace Interactables.Behaviours
{
    public interface IBehaviour
    {
        BehaviourType Type { get; }
        void Behaviour();
        GameObject gameObject { get; }


    }
    
    public enum BehaviourType
    {
        Inspectable,
        Interactable
    }
}