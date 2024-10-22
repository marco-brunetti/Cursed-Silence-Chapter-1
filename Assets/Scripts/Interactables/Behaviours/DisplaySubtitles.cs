using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    [RequireComponent(typeof(SubtitleHelper))]
    public class BehaviourDisplaySubtitles : MonoBehaviour, IBehaviour
    {
        [field: SerializeField] public int[] SubtitleIndex {  get; private set; }

        [FormerlySerializedAs("_subtitleDuration")] [SerializeField] private float[] subtitleDuration;

        [FormerlySerializedAs("_delay")] [SerializeField] private float delay = 1f;

        [FormerlySerializedAs("_triggerOnInteraction")]
        [Header("Trigger type")]
        [SerializeField] private bool triggerOnInteraction;
        [FormerlySerializedAs("_triggerOnInspection")] [SerializeField] private bool triggerOnInspection;
        [FormerlySerializedAs("_triggerOnReturn")] [SerializeField] private bool triggerOnReturn;
        [FormerlySerializedAs("_deactivateAfterTrigger")] [SerializeField] private bool deactivateAfterTrigger;
        [FormerlySerializedAs("_randomize")] [SerializeField] private bool randomize;

        private bool _isActive = true;

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(_isActive)
            {
                if (SubtitleIndex.Length != subtitleDuration.Length)
                {
                    Debug.Log($"Check subtitle index and duration arrays! gameObject:{gameObject}");
                }
                else if (triggerOnReturn == false && triggerOnInspection == false && triggerOnInteraction == false)
                {
                    Debug.Log($"Please set subtitle trigger type! gameObject:{gameObject}");
                }
                else
                {
                    if (triggerOnInteraction == true && isInteracting == true)
                    {
                        StartCoroutine(Trigger());
                    }
                    if (triggerOnInspection == true && isInspecting == true)
                    {
                        StartCoroutine(Trigger());
                    }
                    if(triggerOnReturn && !isInteracting && !isInspecting)
                    {
                        StartCoroutine(Trigger());
                    }
                }
            }

            if(deactivateAfterTrigger)
            {
                _isActive = false;
            }
        }

        private IEnumerator Trigger()
        {
            yield return new WaitForSecondsRealtime(delay);

            if(SubtitleIndex.Length == 1)
            {
                UIManager.Instance.Subtitles.Display(SubtitleIndex[0], subtitleDuration[0]);
                if (subtitleDuration[0] == 0)
                {
                    Debug.Log($"Make sure subtitle durations are greater than zero! gameObject: {gameObject}");
                }
            }
            else
            {
                if (randomize == true)
                {
                    int i = Random.Range(0, SubtitleIndex.Length);
                    UIManager.Instance.Subtitles.Display(SubtitleIndex[i], subtitleDuration[i]);
                    if (subtitleDuration[i] == 0)
                    {
                        Debug.Log($"Make sure subtitle durations are greater than zero! gameObject: {gameObject}");
                    }

                    yield return null;
                }
                else
                {
                    for (int i = 0; i < SubtitleIndex.Length; i++)
                    {
                        UIManager.Instance.Subtitles.Display(SubtitleIndex[i], subtitleDuration[i]);
                        if (subtitleDuration[i] == 0)
                        {
                            Debug.Log($"Make sure subtitle durations are greater than zero! gameObject: {gameObject}");
                        }

                        yield return new WaitForSecondsRealtime(subtitleDuration[i] + 0.5f);
                    }
                }
            }
        }

        public bool IsInteractable()
        {
            return triggerOnInteraction;
        }

        public bool IsInspectable()
        {
            return triggerOnInspection;
        }
    }
}