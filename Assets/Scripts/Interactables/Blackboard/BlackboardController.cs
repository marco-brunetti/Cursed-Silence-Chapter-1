using System.Collections.Generic;
using UnityEngine;

public class BlackboardController : MonoBehaviour, IBehaviour
{

    private List<BlackboardItem> blackboardItems = new();

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (isInteracting)
        {
            var inventorySelected = PlayerController.Instance.Inventory.SelectedItem();

            if (inventorySelected && inventorySelected.TryGetComponent(out BlackboardItem item))
            {
                blackboardItems.Add(item);
                PlayerController.Instance.Inventory.Remove(item.gameObject, deactivateObject: false);
                item.GetComponent<Collider>().enabled = true;
                SendRay(item);
                item.IsOnBlackboard = true;
            }
        }
    }

    public bool IsInspectable()
    {
        return false;
    }

    public bool IsInteractable()
    {
        return true;
    }

    private void SendRay(BlackboardItem item)
    {
        var playerController = PlayerController.Instance;

        Ray ray = new()
        {
            origin = playerController.Camera.position,
            direction = playerController.Camera.forward
        };

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, playerController.PlayerData.InteractDistance, playerController.PlayerData.InteractLayer))
        {
            if(hit.collider.gameObject == gameObject)
            {
                item.transform.position = hit.point;
                item.transform.rotation = Quaternion.Euler(new Vector3(hit.normal.x, hit.normal.y + 90, hit.normal.z));
                item.transform.parent = transform;
            }
        }
    }
}
