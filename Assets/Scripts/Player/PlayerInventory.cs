using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private GameObject _selectedItem;
    private List<GameObject> _inventory;
    private PlayerData _playerData;

    private int _selectedItemIndex;

    private void Start()
    {
        _inventory = new List<GameObject>() { null };
        _playerData = PlayerController.Instance.PlayerData;
    }

    public void Manage()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            ItemSelect();
        }

        //Prevents inventory errors
        if (_selectedItemIndex > _inventory.Count)
        {
            _selectedItemIndex = 0;
            _selectedItem = _inventory[0];
        }
        else if (_selectedItem != _inventory[_selectedItemIndex])
        {
            _selectedItem = _inventory[_selectedItemIndex];
        }
    }

    private void ItemSelect()
    {
        int inventoryCapacity = _inventory.Count - 1;

        if(inventoryCapacity > 0)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                _selectedItemIndex = (_selectedItemIndex < inventoryCapacity) ? ++_selectedItemIndex : 0;

            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                _selectedItemIndex = (_selectedItemIndex > 0) ? --_selectedItemIndex : inventoryCapacity;
            }
        }

        _selectedItem = _inventory[_selectedItemIndex];

        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i] != null)
            {
                if (_inventory[i] == _selectedItem)
                {
                    _selectedItem.SetActive(true);
                }
                else
                {
                    _inventory[i].SetActive(false);
                }
            }
        }
    }

    public void Add(Transform interactable, Vector3 positionInInventory, Vector3 rotationInInventory, Vector3 scaleInInventory)
    {
        //PlayerData playerData = PlayerController.Instance.PlayerData;
        _playerData.InspectablesSource.pitch = 1;
        _playerData.InspectablesSource.PlayOneShot(_playerData.InspectablePickupClip, 0.2f * GameController.Instance.GlobalVolume);

        if (_selectedItem != null)
        {
            _selectedItem.SetActive(false);
        }

        GameObject inventoryObject = PrepareItemForInventory(interactable, positionInInventory, rotationInInventory, scaleInInventory);

        _inventory.Add(inventoryObject);

        _selectedItem = inventoryObject;
        _selectedItemIndex = _inventory.Count - 1;
    }

    private GameObject PrepareItemForInventory(Transform interactable, Vector3 positionInInventory, Vector3 rotationInInventory, Vector3 scaleInInventory)
    {
        if (interactable.TryGetComponent(out Collider interactableCollider))
        {
            interactableCollider.enabled = false;
        }

        if (interactable.TryGetComponent(out Renderer interactableRenderer))
        {
            interactableRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            interactableRenderer.receiveShadows = false;
        }

        interactable.gameObject.layer = LayerMask.NameToLayer("Inventory");
        interactable.SetParent(_playerData.InventoryHolder);
        interactable.localPosition = positionInInventory;
        interactable.localRotation = Quaternion.Euler(rotationInInventory);

        if (scaleInInventory != Vector3.zero)
        {
            interactable.localScale = scaleInInventory;
        }

        ChangeItemLayer(interactable.gameObject, "Inventory");

        //Refreshes camera, solving a layer change bug
        _playerData.InventoryCamera.SetActive(false);
        _playerData.InventoryCamera.SetActive(true);

        return interactable.gameObject;
    }

    public void Remove(GameObject interactable, bool deactivateObject = true)
    {
        ChangeItemLayer(interactable.gameObject, "Default");

        if (deactivateObject)
        {
            _inventory[_selectedItemIndex].SetActive(false);
        }

        _inventory.Remove(interactable);
        _selectedItemIndex = _inventory.Count - 1;
        if(_selectedItemIndex > 0) _inventory[_inventory.Count - 1].SetActive(true);
    }

    private void ChangeItemLayer(GameObject item, string layer)
    {
        item.layer = LayerMask.NameToLayer(layer);

        foreach (Transform child in item.GetComponentInChildren<Transform>(true))
        {
            child.gameObject.layer = LayerMask.NameToLayer(layer);

            foreach (Transform nestedChild in child.GetComponentInChildren<Transform>(true))
            {
                nestedChild.gameObject.layer = LayerMask.NameToLayer(layer);
            }
        }
    }

    public GameObject SelectedItem()
    {
        return _selectedItem;
    }
}
