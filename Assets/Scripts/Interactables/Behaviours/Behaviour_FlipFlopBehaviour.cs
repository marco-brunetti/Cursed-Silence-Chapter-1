using System.Collections;
using UnityEngine;

public class Behaviour_FlipFlopBehaviour : MonoBehaviour, IBehaviour
{
    [Header("Flip")]
    [SerializeField] private GameObject[] _flipBehaviours;
    [SerializeField] private float _flipDelay;

    [Header("Flop")]
    [SerializeField] private GameObject[] _flopBehaviours;
    [SerializeField] private float _flopDelay;

    private bool flip = true;

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
        if (flip)
        {
            yield return new WaitForSeconds(_flipDelay);

            for(int i = 0; i < _flipBehaviours.Length; i++)
            {
                _flipBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
            }

            flip = false;
        }
        else if(!flip)
        {
            yield return new WaitForSeconds(_flopDelay);

            for (int i = 0; i < _flopBehaviours.Length; i++)
            {
                _flopBehaviours[i].GetComponent<IBehaviour>().Behaviour(isInteracting, isInspecting);
            }

            flip = true;
        }
    }
}
