using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class ChangeLightState : MonoBehaviour, IBehaviour
    {
        [SerializeField] private bool turnOn;
        [SerializeField] private LightSwitch[] switches;
        [SerializeField] private bool onInteraction;
        [SerializeField] private bool onInspection;


        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if(onInteraction && isInteracting)
            {
                DeactivateLights(isInteracting, isInspecting);
            }
            else if(onInspection && isInspecting)
            {
                DeactivateLights(isInteracting, isInspecting);
            }
            else if(onInteraction == false && onInspection == false)
            {
                Debug.Log($"{gameObject.name} onInteraction: {onInteraction} onInspection: {onInspection}");
            }
        }

        public bool IsInspectable()
        {
            return onInspection;
        }

        public bool IsInteractable()
        {
            return onInteraction;
        }

        private void DeactivateLights(bool isInteracting, bool isInspecting)
        {
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i].isOn = !turnOn; //set to the opposite of desired behaviour for now
                switches[i].Behaviour(isInteracting, isInspecting);
            }
        }
    }
}
