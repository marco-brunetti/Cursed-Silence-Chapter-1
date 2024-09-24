using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class BehaviourAddStress : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_onInteraction")] [SerializeField] private bool onInteraction;
        [FormerlySerializedAs("_onInspection")] [SerializeField] private bool onInspection;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if (onInteraction && isInteracting)
            {
                AddStress();
            }
            else if (onInspection && isInspecting)
            {
                AddStress();
            }
            else if(!isInteracting && !isInspecting)
            {
                AddStress();
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

        private void AddStress()
        {
            PlayerController.Instance.PlayerStress.AddStress();
        }
    }
}
