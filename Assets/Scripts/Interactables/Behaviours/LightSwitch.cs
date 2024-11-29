using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class LightSwitch : Behaviour
    {
        public bool isOn;

        [FormerlySerializedAs("_lightsOn")] [SerializeField] private List<GameObject> lightsOn;
        [FormerlySerializedAs("_lightsOff")] [SerializeField] private List<GameObject> lightsOff;

        [FormerlySerializedAs("_offModel")] [SerializeField] private GameObject offModel;
        [FormerlySerializedAs("_onModel")] [SerializeField] private GameObject onModel;
    

        private void Start()
        {
            ManageLights(isOn);
        }

        public override void Activate()
        {
            isOn = !isOn;

            ManageLights(isOn);
        }

        private void ManageLights(bool enable)
        {
            onModel.SetActive(enable);
            offModel.SetActive(!enable);

            for (int i = 0; i < lightsOn.Count; i++)
            {
                lightsOn[i].SetActive(enable);
                lightsOff[i].SetActive(!enable);
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

        public override BehaviourType Type { get; }
    }
}
