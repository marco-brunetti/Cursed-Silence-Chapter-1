using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private List<GameObject> _inventory;
        private PlayerData _playerData;
        
        private void Start()
        {
            _inventory = new List<GameObject>() { null };
            _playerData = PlayerController.Instance.PlayerData;
        }

        public void Add(Transform interactable)
        {
            PlayerController.Instance.InspectablesSource.pitch = 1;
            PlayerController.Instance.InspectablesSource.PlayOneShot(_playerData.InspectablePickupClip,
                0.2f * GameController.Instance.GlobalVolume);
     
            interactable.SetParent(PlayerController.Instance.InventoryHolder);
            
            _inventory.Add(interactable.gameObject);
        }

        public bool Contains(GameObject item, bool removeItem, bool destroyItem)
        {
            if(!item) return false;
            
            var isInInventory = _inventory.Exists(x => x == item);

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
                    _inventory.Remove(component.gameObject);
                    if(destroyItem) Destroy(component.gameObject);
                }
                
                //Check UI manager
            }

            return component;
        }
    }
}