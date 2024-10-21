using System.Collections.Generic;
using System.Linq;
using Game.General;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private HashSet<InventoryItem> _inventory = new();
        private PlayerData _playerData;

        public void Add(InventoryItem item)
        {
            if(!_playerData) _playerData = PlayerController.Instance.PlayerData;

            PlayerController.Instance.InspectablesSource.pitch = 1;
            PlayerController.Instance.InspectablesSource.PlayOneShot(_playerData.InspectablePickupClip,
                0.2f * GameController.Instance.GlobalVolume);
     
            item.transform.SetParent(PlayerController.Instance.InventoryHolder);
            item.transform.localPosition = Vector3.zero;
            item.gameObject.SetActive(false);
            
            _inventory.Add(item);
        }

        public bool Contains(InventoryItem item, bool removeItem, bool destroyItem)
        {
            if(!item) return false;
            
            var isInInventory = _inventory.Contains(item);

            if (isInInventory && removeItem)
            {
                _inventory.Remove(item);
                if(destroyItem) Destroy(item.gameObject);
            }
            
            return isInInventory;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public T Find<T>(bool removeItem, bool destroyItem) where T : Component
        {
            T component = null;
            _inventory.FirstOrDefault(x=>x.TryGetComponent(out component));

            if (component && removeItem)
            {
                _inventory.Remove(component.GetComponent<InventoryItem>());
                if(destroyItem) Destroy(component.gameObject);
            }

            return component;
        }

        public void ShowInUI(List<InventoryItem> requiredItems)
        {
            
        }
    }
}