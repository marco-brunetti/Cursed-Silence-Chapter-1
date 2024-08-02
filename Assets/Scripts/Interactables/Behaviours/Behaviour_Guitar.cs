using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_Guitar : MonoBehaviour, IBehaviour
{
    [SerializeField] private AudioClip[] _guitarStrumClips;
    [SerializeField] private AudioSource _guitarStrumSource;
    [SerializeField] private float _volume;
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

            _guitarStrumSource.PlayOneShot(_guitarStrumClips[_currentStrumIndex], _volume * GameController.Instance.GlobalVolume);
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
