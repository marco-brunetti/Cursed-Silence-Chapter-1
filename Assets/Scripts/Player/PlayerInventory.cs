using System;
using System.Collections.Generic;
using System.Linq;
//using Game.General;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private HashSet<IInventoryItem> inventory = new();
        private PlayerData playerData;
        public static EventHandler<bool> ShowUIPoint;

        public void Add(IInventoryItem item)
        {
            if(!playerData) playerData = PlayerController.Instance.PlayerData;
     
            item.gameObject.transform.SetParent(PlayerController.Instance.InventoryHolder);
            item.gameObject.transform.localPosition = Vector3.zero;
            item.gameObject.SetActive(false);
            
            inventory.Add(item);
            ShowUIPoint.Invoke(this, true);
        }

        public bool Contains(IInventoryItem item) => item != null && inventory.Contains(item);
        
        public bool Find<T>(bool removeFromInventory, out T foundItem) where T : Component
        {
            T component = null;
            inventory.FirstOrDefault(x=>x.gameObject.TryGetComponent(out component));

            if (component)
            {
                foundItem = component;
                if(removeFromInventory) inventory.Remove(component.GetComponent<IInventoryItem>());
                return true;
            }

            foundItem = default;
            return false;
        }

        public void Remove(IInventoryItem item)
        {
            if(inventory.Contains(item)) inventory.Remove(item);
        }

        public void RemoveAndDestroy(IInventoryItem item)
        {
            Remove(item);
            if(item.gameObject) Destroy(item.gameObject);
        }

        public void ShowInUI(List<IInventoryItem> requiredItems)
        {
            
        }
    }
}