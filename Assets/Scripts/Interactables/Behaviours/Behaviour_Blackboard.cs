using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_Blackboard : MonoBehaviour, IBehaviour
    {
        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(isInteracting)
            {

            }
        }

        public bool IsInspectable()
        {
            return false;
        }

        public bool IsInteractable()
        {
            return true;
        }
    }
}