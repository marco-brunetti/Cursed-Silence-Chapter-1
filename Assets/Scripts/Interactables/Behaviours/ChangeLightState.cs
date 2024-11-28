using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class ChangeLightState : MonoBehaviour, IBehaviour
    {
        [SerializeField] private bool turnOn;
        [SerializeField] private LightSwitch[] switches;

        private void DeactivateLights()
        {
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i].isOn = !turnOn; //set to the opposite of desired behaviour for now
                switches[i].Behaviour();
            }
        }

        public BehaviourType Type { get; }
        public void Behaviour()
        {
            DeactivateLights();
        }
    }
}
