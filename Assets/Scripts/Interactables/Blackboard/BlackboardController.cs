using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackboardController : MonoBehaviour, IBehaviour
{
    [field: SerializeField] public List<BlackboardItem> BlackboardItems { get; private set; } = new();

    public static BlackboardController Instance;

    private Collider _collider;
    private PlayerController _playerController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _collider = GetComponent<Collider>();

        if(BlackboardItems.Count > 0)
        {
            foreach(var item in BlackboardItems)
            {
                item.BlackboardCollider = _collider;
            }
        }
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if (isInteracting)
        {
            var inventorySelected = _playerController.Inventory.SelectedItem();

            if (inventorySelected && inventorySelected.TryGetComponent(out BlackboardItem item))
            {
                SetPos(item);

                BlackboardItems.Add(item);
                _playerController.Inventory.Remove(item.gameObject, deactivateObject: false);

                item.HoldItem(isFirstPlacement: true, blackboardCollider: _collider);
            }
        }
    }

    private void SetPos(BlackboardItem item)
    {
        Ray ray = new() { origin = _playerController.Camera.position, direction = _playerController.Camera.forward };

        if (Physics.Raycast(ray, out RaycastHit hit, _playerController.PlayerData.InteractDistance, _playerController.PlayerData.InteractLayer))
        {
            if(hit.collider.gameObject == gameObject)
            {
                item.transform.position = hit.point;
                item.transform.eulerAngles = new Vector3(hit.normal.x, hit.normal.y + 90, item.transform.eulerAngles.z);

                /*var previousPiece = BlackboardItems.Find(x => x.PageNumber == item.PageNumber);

                if(previousPiece) item.transform.parent = previousPiece.transform;
                else item.transform.parent = transform.parent;*/

                item.transform.parent = transform.parent;
                item.transform.localScale = Vector3.one;
            }
        }
    }

    public bool IsInspectable() { return false; }
    public bool IsInteractable() { return true; }
}
