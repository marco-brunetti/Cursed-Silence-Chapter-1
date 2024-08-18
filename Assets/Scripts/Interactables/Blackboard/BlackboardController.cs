using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardController : MonoBehaviour, IBehaviour
{
    [field: SerializeField] public List<BlackboardItem> BlackboardItems { get; private set; } = new();
    public static BlackboardController Instance;

    private Collider _collider;
    private PlayerController _playerController;
    private BlackboardItem _currentBlackboardItem;
    private ItemOrientation _tempOrientation;
    private BlackboardState _currentState;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        _playerController = PlayerController.Instance;
        _collider = GetComponent<Collider>();

        UIManager.Instance.SetBlackboardButtons(RotateItem, ApplyRotation, ResetBlackboard);
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

                HoldItem(item, isFirstPlacement: true);
            }
        }
    }

    private void Update()
    {
        switch (_currentState)
        {
            case BlackboardState.Moving:
                Ray ray = new() { origin = _playerController.Camera.position, direction = _playerController.Camera.forward };
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, _playerController.PlayerData.InteractDistance, _playerController.PlayerData.InteractLayer))
                {
                    if (hit.collider == _collider)
                    {
                        if (_currentBlackboardItem.MoveOffset == Vector3.zero) _currentBlackboardItem.MoveOffset = _currentBlackboardItem.transform.position - hit.point;

                        _currentBlackboardItem.transform.position = hit.point + _currentBlackboardItem.MoveOffset;
                        _currentBlackboardItem.transform.eulerAngles = new Vector3(hit.normal.x, hit.normal.y + 90, GetOrientationAngle(_currentBlackboardItem.Orientation));
                    }
                }
                break;
        }
    }

    public IEnumerator CheckMouseHold(BlackboardItem selectedItem)
    {
        yield return new WaitForSecondsRealtime(0.1f);

        if (Input.GetMouseButton(0))
        {
            HoldItem(selectedItem);
        }
        else
        {
            _currentState = BlackboardState.Looking;
            _currentBlackboardItem = selectedItem;
            _tempOrientation = _currentBlackboardItem.Orientation;
            UIManager.Instance.ShowBlackboardImage(sprite: _currentBlackboardItem.Sprite, zAngle: GetOrientationAngle(_tempOrientation));
            SetupComponentsForLook(isLooking: true);
        }
    }

    public void HoldItem(BlackboardItem item, bool isFirstPlacement = false)
    {
        _currentBlackboardItem = item;
        _currentState = BlackboardState.Moving;
        _playerController.FreezePlayerMovement = true;
        item.GetComponent<Collider>().enabled = false;
        StartCoroutine(WaitForMouseUp(item, isFirstPlacement));
    }
    private IEnumerator WaitForMouseUp(BlackboardItem item, bool isFirstPlacement)
    {
        if (isFirstPlacement) yield return new WaitForSecondsRealtime(0.1f);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        item.GetComponent<Collider>().enabled = true;
        _playerController.FreezePlayerMovement = false;
        item.MoveOffset = Vector3.zero;
        _currentState = BlackboardState.None;
    }

    private void SetupComponentsForLook(bool isLooking)
    {
        _playerController.FreezePlayerMovement = isLooking;
        _playerController.FreezePlayerRotation = isLooking;
        _playerController.ActivateDepthOfField(isLooking);
        _currentBlackboardItem.EnableComponents(!isLooking);
    }

    private void SetPos(BlackboardItem item)
    {
        Ray ray = new() { origin = _playerController.Camera.position, direction = _playerController.Camera.forward };

        if (Physics.Raycast(ray, out RaycastHit hit, _playerController.PlayerData.InteractDistance, _playerController.PlayerData.InteractLayer))
        {
            if(hit.collider.gameObject == gameObject)
            {
                item.transform.position = hit.point;
                item.transform.eulerAngles = new Vector3(hit.normal.x, hit.normal.y + 90, GetOrientationAngle(item.Orientation));

                /*var previousPiece = BlackboardItems.Find(x => x.PageNumber == item.PageNumber);

                if(previousPiece) item.transform.parent = previousPiece.transform;
                else item.transform.parent = transform.parent;*/

                item.transform.parent = transform.parent;
                item.transform.localScale = Vector3.one;
            }
        }
    }

    private void RotateItem()
    {
        if (_tempOrientation == ItemOrientation.Up) _tempOrientation = ItemOrientation.Right;
        else if (_tempOrientation == ItemOrientation.Right) _tempOrientation = ItemOrientation.Down;
        else if (_tempOrientation == ItemOrientation.Down) _tempOrientation = ItemOrientation.Left;
        else if (_tempOrientation == ItemOrientation.Left) _tempOrientation = ItemOrientation.Up;

        UIManager.Instance.ShowBlackboardImage(zAngle: GetOrientationAngle(_tempOrientation));
    }

    private void ApplyRotation()
    {
        var itemRotation = _currentBlackboardItem.transform.eulerAngles;
        itemRotation.z = GetOrientationAngle(_tempOrientation);
        _currentBlackboardItem.transform.eulerAngles = itemRotation;
        _currentBlackboardItem.Orientation = _tempOrientation;

        ResetBlackboard();
    }

    private void ResetBlackboard()
    {
        UIManager.Instance.ShowBlackboardImage(false);
        SetupComponentsForLook(false);
        _currentBlackboardItem = null;
        _currentState = BlackboardState.None;
    }

    public float GetOrientationAngle(ItemOrientation orientation)
    {
        switch (orientation)
        {
            default:
            case ItemOrientation.Up:
                return 0;
            case ItemOrientation.Left:
                return 90;
            case ItemOrientation.Down:
                return 180;
            case ItemOrientation.Right:
                return 270;
        }
    }

    public bool IsInspectable() { return false; }
    public bool IsInteractable() { return true; }
}

public enum BlackboardState
{
    None,
    Looking,
    Moving,
    Joined
}