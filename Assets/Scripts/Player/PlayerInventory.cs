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

        public void Add(IInventoryItem item)
        {
            if(!playerData) playerData = PlayerController.Instance.PlayerData;
     
            item.gameObject.transform.SetParent(PlayerController.Instance.InventoryHolder);
            item.gameObject.transform.localPosition = Vector3.zero;
            item.gameObject.SetActive(false);
            
            inventory.Add(item);
        }

        public bool Contains(IInventoryItem item, bool removeItem, bool destroyItem)
        {
            if(item == null) return false;
            
            var isInInventory = inventory.Contains(item);

            if (isInInventory && removeItem)
            {
                inventory.Remove(item);
                if(destroyItem) Destroy(item.gameObject);
            }
            
            return isInInventory;
        }
        
        public T Find<T>(bool removeItem, bool destroyItem) where T : Component
        {
            T component = null;
            inventory.FirstOrDefault(x=>x.gameObject.TryGetComponent(out component));

            if (component && removeItem)
            {
                inventory.Remove(component.GetComponent<IInventoryItem>());
                if(destroyItem) Destroy(component.gameObject);
            }

            return component;
        }

        public void ShowInUI(List<IInventoryItem> requiredItems)
        {
            
        }
    }
}