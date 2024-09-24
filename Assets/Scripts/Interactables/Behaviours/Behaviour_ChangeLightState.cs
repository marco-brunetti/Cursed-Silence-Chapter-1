using SnowHorse.Utils;
using UnityEngine;

namespace Interactables.Behaviours
{
    public class Behaviour_ChangeLightState : MonoBehaviour, IBehaviour
    {
        [SerializeField] private bool _turnOn;
        [SerializeField] private Behaviour_LightSwitch[] _switches;
        [SerializeField] private bool _onInteraction;
        [SerializeField] private bool _onInspection;


        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(_onInteraction && isInteracting)
            {
                DeactivateLights(isInteracting, isInspecting);
            }
            else if(_onInspection && isInspecting)
            {
                DeactivateLights(isInteracting, isInspecting);
            }
            else if(_onInteraction == false && _onInspection == false)
            {
                WarningTool.Print("Make sure to set activation type!", gameObject);
            }
        }

        public bool IsInspectable()
        {
            return _onInspection;
        }

        public bool IsInteractable()
        {
            return _onInteraction;
        }

        private void DeactivateLights(bool isInteracting, bool isInspecting)
        {
            for (int i = 0; i < _switches.Length; i++)
            {
                _switches[i].isOn = !_turnOn; //set to the opposite of desired behaviour for now
                _switches[i].Behaviour(isInteracting, isInspecting);
            }
        }
    }
}
