using System;
using System.Collections.Generic;
using Interactables;
using UnityEngine;
using Player;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIInventoryRequirement : MonoBehaviour
    {

        [SerializeField] private List<Image> requirementImages = new();
        [SerializeField] private Color availableItemColor = new();
        [SerializeField] private Color notAvailableItemColor = new();
        private void Start()
        {
            InventoryRequirement.showRequiredItems += ShowInventoryItem;
            InventoryRequirement.hideRequiredItems += HideInventoryItem;
        }

        private void ShowInventoryItem(object sender, List<IInventoryItem> requiredInventoryItems)
        {
            for (var i = 0; i < requiredInventoryItems.Count; i++)
            {
                var image = requirementImages[i];
                
                if (i > requirementImages.Count)
                {
                    image.gameObject.SetActive(false);
                    continue;
                }

                image.gameObject.SetActive(true);
                image.sprite = requiredInventoryItems[i].UiIcon;

                var inventory = PlayerController.Instance.Inventory;
                image.color = inventory.Contains(requiredInventoryItems[i]) ? availableItemColor : notAvailableItemColor;
            }
        }
        
        private void HideInventoryItem(object sender, EventArgs e)
        {
            foreach (var image in requirementImages)
            {
                image.gameObject.SetActive(false);
            }
        }
    }
}
