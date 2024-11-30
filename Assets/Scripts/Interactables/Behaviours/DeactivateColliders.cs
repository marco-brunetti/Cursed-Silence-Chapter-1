using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class BehaviourDeactivateColliders : Behaviour
    {
        [FormerlySerializedAs("_colliders")] [SerializeField] private Collider[] colliders;
        [FormerlySerializedAs("_delay")] [SerializeField] private float delay;

        public override void Activate()
        {
            StartCoroutine(Deactivate());
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
