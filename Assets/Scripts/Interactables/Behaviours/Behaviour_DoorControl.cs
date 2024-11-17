using UnityEngine;
using SnowHorse.Components;
using System;

namespace Interactables.Behaviours
{
    public class Behaviour_DoorControl : MonoBehaviour, IBehaviour
    {
        [SerializeField] private TriggerEnterDetector playerEnterDetector;
        [SerializeField] private TriggerExitDetector playerExitDetector;

        public static EventHandler OnPlayerEnterDoor;
        public static EventHandler OnPlayerExitDoor;

        private void Awake()
        {
            playerEnterDetector.TagEntered += (sender, e) => OnPlayerEnterDoor?.Invoke(sender, e);
            playerExitDetector.TagExited += (sender, e) => OnPlayerExitDoor?.Invoke(sender, e);
        }

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            
        }
        public bool IsInteractable() => true;
        public bool IsInspectable() => false;
    }
}