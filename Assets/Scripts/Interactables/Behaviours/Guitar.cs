using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class Guitar : MonoBehaviour, IBehaviour
    {
        [FormerlySerializedAs("_guitarStrumClips")] [SerializeField] private AudioClip[] guitarStrumClips;
        [FormerlySerializedAs("_guitarStrumSource")] [SerializeField] private AudioSource guitarStrumSource;
        [FormerlySerializedAs("_volume")] [SerializeField] private float volume;
        private int _currentStrumIndex;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(isInteracting)
            {
                if (_currentStrumIndex == 0 || _currentStrumIndex == 1)  //Place strum clips in grades I, VI, IV, and V
                {
                    _currentStrumIndex = Random.Range(2, 4); //minInclusive, maxExclusive
                }
                else
                {
                    _currentStrumIndex = Random.Range(0, 2); //minInclusive, maxExclusive
                }

                //guitarStrumSource.PlayOneShot(guitarStrumClips[_currentStrumIndex], volume * GameController.Instance.GlobalVolume);
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
