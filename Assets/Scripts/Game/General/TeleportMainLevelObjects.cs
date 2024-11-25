using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Player;

namespace Game.General
{
    public class TeleportMainLevelObjects : MonoBehaviour
    {
        [SerializeField] private Transform objectsContainer;
        [SerializeField] private List<Transform> mainLevelInstances;

        private int currentIndex = 0;
        private PlayerController controller;

        private void Start()
        {
            controller = PlayerController.Instance;
            objectsContainer.transform.position = mainLevelInstances[currentIndex].position;
        }
        
        public void UpdateObjectsPosition()
        {
            if (currentIndex < mainLevelInstances.Count)
            {
                currentIndex++;
                objectsContainer.transform.position = mainLevelInstances[currentIndex].position;
            }
            else
            {
                Debug.Log("No more main level instances.");
            }
        }

        public void UpdatePlayerAndObjectsPosition()
        {
            UpdateObjectsPosition();
            
            var offset = mainLevelInstances[currentIndex].position - mainLevelInstances[currentIndex - 1].position;

            controller.Character.enabled = false;
            controller.Player.transform.position += offset;
            controller.Character.enabled = true;
        }
    }
}