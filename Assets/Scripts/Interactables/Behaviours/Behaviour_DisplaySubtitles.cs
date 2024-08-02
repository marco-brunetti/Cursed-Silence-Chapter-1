using System.Collections;
using UnityEngine;
using SnowHorse.Utils;
using Unity.VisualScripting;

[RequireComponent(typeof(SubtitleHelper))]
public class Behaviour_DisplaySubtitles : MonoBehaviour, IBehaviour
{
    [field: SerializeField] public int[] SubtitleIndex {  get; private set; }

    [SerializeField] private float[] _subtitleDuration;

    [SerializeField] private float _delay = 1f;

    [Header("Trigger type")]
    [SerializeField] private bool _triggerOnInteraction;
    [SerializeField] private bool _triggerOnInspection;
    [SerializeField] private bool _triggerOnReturn;
    [SerializeField] private bool _deactivateAfterTrigger;
    [SerializeField] private bool _randomize;


    private bool isActive = true;

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isActive)
        {
            if (SubtitleIndex.Length != _subtitleDuration.Length)
            {
                WarningTool.Print("Check subtitle index and duration arrays!", gameObject);
            }
            else if (_triggerOnReturn == false && _triggerOnInspection == false && _triggerOnInteraction == false)
            {
                WarningTool.Print("Please set subtitle trigger type!", gameObject);
            }
            else
            {
                if (_triggerOnInteraction == true && isInteracting == true)
                {
                    StartCoroutine(Trigger());
                }
                if (_triggerOnInspection == true && isInspecting == true)
                {
                    StartCoroutine(Trigger());
                }
                if(_triggerOnReturn && !isInteracting && !isInspecting)
                {
                    StartCoroutine(Trigger());
                }
            }
        }

        if(_deactivateAfterTrigger)
        {
            isActive = false;
        }
    }

    private IEnumerator Trigger()
    {
        yield return new WaitForSecondsRealtime(_delay);

        if(SubtitleIndex.Length == 1)
        {
            UIManager.Instance.Subtitles.Display(SubtitleIndex[0], _subtitleDuration[0]);
            if (_subtitleDuration[0] == 0)
            {
                WarningTool.Print("Make sure subtitle durations are greater than zero!", gameObject);
            }
        }
        else
        {
            if (_randomize == true)
            {
                int i = Random.Range(0, SubtitleIndex.Length);
                UIManager.Instance.Subtitles.Display(SubtitleIndex[i], _subtitleDuration[i]);
                if (_subtitleDuration[i] == 0)
                {
                    WarningTool.Print("Make sure subtitle durations are greater than zero!", gameObject);
                }

                yield return null;
            }
            else
            {
                for (int i = 0; i < SubtitleIndex.Length; i++)
                {
                    UIManager.Instance.Subtitles.Display(SubtitleIndex[i], _subtitleDuration[i]);
                    if (_subtitleDuration[i] == 0)
                    {
                        WarningTool.Print("Make sure subtitle durations are greater than zero!", gameObject);
                    }

                    yield return new WaitForSecondsRealtime(_subtitleDuration[i] + 0.5f);
                }
            }
        }
    }

    public bool IsInteractable()
    {
        return _triggerOnInteraction;
    }

    public bool IsInspectable()
    {
        return _triggerOnInspection;
    }
}