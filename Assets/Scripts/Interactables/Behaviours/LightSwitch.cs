using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactables.Behaviours
{
    public class LightSwitch : MonoBehaviour, IBehaviour
    {
        public bool isOn;

        [FormerlySerializedAs("_lightsOn")] [SerializeField] private List<GameObject> lightsOn;
        [FormerlySerializedAs("_lightsOff")] [SerializeField] private List<GameObject> lightsOff;

        [FormerlySerializedAs("_offModel")] [SerializeField] private GameObject offModel;
        [FormerlySerializedAs("_onModel")] [SerializeField] private GameObject onModel;

        [FormerlySerializedAs("_onAudio")] [SerializeField] private PlayAudio onAudio;
        [FormerlySerializedAs("_offAudio")] [SerializeField] private PlayAudio offAudio;
    

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
                onAudio.Behaviour(false, false);
            }
            else
            {
                offAudio.Behaviour(false, false);
            }
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
    }
}
