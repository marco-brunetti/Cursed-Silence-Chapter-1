using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class BehaviourDeactivateColliders : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_colliders")] [SerializeField] private Collider[] colliders;
        [FormerlySerializedAs("_delay")] [SerializeField] private float delay;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            StartCoroutine(Deactivate());
        }

        public bool IsInspectable()
        {
            return false;
        }

        public bool IsInteractable()
        {
            return true;
        }

        private IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(delay);

            for(int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
        }
    }
}
