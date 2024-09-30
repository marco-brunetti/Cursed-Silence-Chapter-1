using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class BehaviourFlipFlopBehaviour : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_flipBehaviours")]
        [Header("Flip")]
        [SerializeField] private GameObject[] flipBehaviours;
        [FormerlySerializedAs("_flipDelay")] [SerializeField] private float flipDelay;

        [FormerlySerializedAs("_flopBehaviours")]
        [Header("Flop")]
        [SerializeField] private GameObject[] flopBehaviours;
        [FormerlySerializedAs("_flopDelay")] [SerializeField] private float flopDelay;

        private bool _flip = true;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            StartCoroutine(FlipFlop(isInteracting, isInspecting));
        }

        public bool IsInspectable()
        {
            return false;
        }

        public bool IsInteractable()
        {
            return true;
        }

        private IEnumerator FlipFlop(bool isInteracting, bool isInspecting)
        {
            if (_flip)
            {
                yield return new WaitForSeconds(flipDelay);

                for(int i = 0; i < flipBehaviours.Length; i++)
                {
                    flipBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
                }

                _flip = false;
            }
            else if(!_flip)
            {
                yield return new WaitForSeconds(flopDelay);

                for (int i = 0; i < flopBehaviours.Length; i++)
                {
                    flopBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
                }

                _flip = true;
            }
        }
    }
}
