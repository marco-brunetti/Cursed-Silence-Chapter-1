using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class ChangeLightState : Behaviour
    {
        [SerializeField] private bool turnOn;
        [SerializeField] private LightSwitch[] switches;

        private void DeactivateLights()
        {
            for (int i = 0; i < switches.Length; i++)
            {
                switches[i].isOn = !turnOn; //set to the opposite of desired behaviour for now
                switches[i].Activate();
            }
        }

        public override BehaviourType Type { get; }
        public override void Activate()
        {
            DeactivateLights();
        }
    }
}
