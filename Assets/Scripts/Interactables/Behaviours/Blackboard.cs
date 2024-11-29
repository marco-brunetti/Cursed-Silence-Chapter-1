using UnityEngine;

namespace Interactables.Behaviours
{
    public class BehaviourBlackboard : Behaviour
    {
        public override BehaviourType Type { get; } = BehaviourType.Interactable;

        public override void Activate()
        {
            
        }
    }
}