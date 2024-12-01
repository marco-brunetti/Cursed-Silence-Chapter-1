using System;
using Game.General;
using UnityEngine;
using Interactables.Behaviours;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIBlackboard : MonoBehaviour
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Image blackboardImage;
        [SerializeField] private Button rotateButton;
        [SerializeField] private Button applyRotationButton;
        [SerializeField] private Button cancelRotationButton;
        
        
        private void Awake()
        {
            BlackboardController.blackboardUIAction += SetBlackboardButtons;
            BlackboardController.showBlackboardImage += ShowBlackboardImage;
        }

        private void SetBlackboardButtons(object sender, BlackboardController.BlackboardUIActionArgs args)
        {
            rotateButton.onClick.AddListener(args.RotateItem);
            applyRotationButton.onClick.AddListener(args.ApplyRotation);
            cancelRotationButton.onClick.AddListener(args.CancelRotation);
        }

        private void ShowBlackboardImage(object sender, BlackboardController.ShowBlackboardImageArgs args)
        {
            if(args.Sprite) blackboardImage.sprite = args.Sprite;

            blackboardImage.transform.localRotation = Quaternion.Euler(0, 0, args.ZAngle);
            container.SetActive(args.Show);
            GameControllerV2.ActiveCursor(args.Show);
        }
    }
}
