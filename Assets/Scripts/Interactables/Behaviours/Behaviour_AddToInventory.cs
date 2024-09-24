using Player;
using UnityEngine;

public class Behaviour_AddToInventory : MonoBehaviour, IBehaviour
{
    [SerializeField] public Transform GameObjectToAdd;

    private bool _addedToInventory;
    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (!_addedToInventory && isInteracting)
        {
            GetInventoryObject();

            PlayerController.Instance.Inventory.Add(GameObjectToAdd);
            _addedToInventory = true;
        }
    }

    private void GetInventoryObject()
    {
        if (GameObjectToAdd == null)
        {
            if(gameObject.GetComponent<Interactable>() == null)
            {
                if(transform.GetComponentInParent<Interactable>() == null)
                {
                    print("Didn't find inventory object! object: " + gameObject.name + "; parent: " + transform.parent.name);
                }
                else
                {
                    GameObjectToAdd = transform.parent.transform;
                }
            }
            else
            {
                GameObjectToAdd = gameObject.transform;
            }
        }
    }

    public bool IsInteractable()
    {
        return true;
    }

    public bool IsInspectable()
    {
        return false;
    }
}
