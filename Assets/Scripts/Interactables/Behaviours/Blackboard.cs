using UnityEngine;

namespace Interactables.Behaviours
{
    public class BehaviourBlackboard : MonoBehaviour, IBehaviour
    {
        public BehaviourType Type { get; } = BehaviourType.Interactable;

        public void Behaviour()
        {
            
        }
    }
}