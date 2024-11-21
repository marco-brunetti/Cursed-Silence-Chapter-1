using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Player;

namespace Game.General
{
    public class TeleportMainLevelObjects : MonoBehaviour
    {
        [SerializeField] private Transform objectsContainer;
        [SerializeField] private List<Transform> targets;

        private int currentIndex = 0;
        private PlayerController controller;

        private void Start()
        {
            objectsContainer.parent = targets[currentIndex];
            objectsContainer.localPosition = Vector3.zero;
            controller = PlayerController.Instance;
        }
        
        public void UpdateObjectsPosition()
        {
            if (currentIndex < targets.Count)
            {
                currentIndex++;
                objectsContainer.parent = targets[currentIndex];
                objectsContainer.localPosition = Vector3.zero;
            }
            else
            {
                Debug.Log("No more main level instances.");
            }
        }

        public void UpdatePlayerAndObjectsPosition()
        {
            StartCoroutine(TeleportPlayer());
        }

        private IEnumerator TeleportPlayer()
        {
            controller.IsTeleporting = true;
            controller.transform.parent = objectsContainer;
            yield return new WaitForEndOfFrame();
            UpdateObjectsPosition();
            yield return new WaitForEndOfFrame();
            controller.transform.parent = null;
            controller.IsTeleporting = false;
        }
    }
}