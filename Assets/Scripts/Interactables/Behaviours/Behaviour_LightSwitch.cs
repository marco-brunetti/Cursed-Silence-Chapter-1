using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour_LightSwitch : MonoBehaviour, IBehaviour
{
    public bool isOn;

    [SerializeField] private List<GameObject> _lightsOn;
    [SerializeField] private List<GameObject> _lightsOff;

    [SerializeField] private GameObject _offModel;
    [SerializeField] private GameObject _onModel;

    [SerializeField] private Behaviour_PlayAudio _onAudio;
    [SerializeField] private Behaviour_PlayAudio _offAudio;
    

    private void Start()
    {
        ManageLights(isOn);
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        isOn = !isOn;

        ManageLights(isOn);

        if(isOn)
        {
            _onAudio.Behaviour(false, false);
        }
        else
        {
            _offAudio.Behaviour(false, false);
        }
    }

    private void ManageLights(bool enable)
    {
        _onModel.SetActive(enable);
        _offModel.SetActive(!enable);

        for (int i = 0; i < _lightsOn.Count; i++)
        {
            _lightsOn[i].SetActive(enable);
            _lightsOff[i].SetActive(!enable);
        }
    }

    public bool IsInteractable()
    {
        return true;
    }

    public bool IsInspectable()
    {
        return false;
    }
}
