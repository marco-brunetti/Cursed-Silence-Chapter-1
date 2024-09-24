using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private HashSet<InventoryItem> _inventory = new();
        private PlayerData _playerData;
        
        private void Start()
        {
            _playerData = PlayerController.Instance.PlayerData;
        }

        public void Add(InventoryItem item)
        {
            PlayerController.Instance.InspectablesSource.pitch = 1;
            PlayerController.Instance.InspectablesSource.PlayOneShot(_playerData.InspectablePickupClip,
                0.2f * GameController.Instance.GlobalVolume);
     
            item.transform.SetParent(PlayerController.Instance.InventoryHolder);
            
            _inventory.Add(item);
        }

        public bool Contains(InventoryItem item, bool removeItem, bool destroyItem)
        {
            if(!item) return false;
            
            var isInInventory = _inventory.Contains(item);

            if (isInInventory)
            {
                if (removeItem)
                {
                    _inventory.Remove(item);
                    if(destroyItem) Destroy(item.gameObject);
                }
                //Check UI manager
            }
            
            return isInInventory;
        }

        public T Contains<T>(bool removeItem, bool destroyItem) where T : Component
        {
            T component = null;
            _inventory.FirstOrDefault(x=>x.TryGetComponent(out component));

            if (component)
            {
                if (removeItem)
                {
                    _inventory.Remove(component.GetComponent<InventoryItem>());
                    if(destroyItem) Destroy(component.gameObject);
                }
                
                //Check UI manager
            }

            return component;
        }
    }
}