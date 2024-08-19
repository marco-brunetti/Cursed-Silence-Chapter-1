using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnowHorse.Utils;

public class BlackboardController : MonoBehaviour, IBehaviour
{
	[field: SerializeField] public List<GameObject> BlackboardItems { get; private set; } = new();
	public static BlackboardController Instance;
	[NonSerialized] public BlackboardItem CurrentItem;
	public EventHandler<BlackboardEventArgs> SetColliderEnabled;

	private int _showRotateIconCount = 3;
	private int _defaultRendererOrder;
	private Collider _collider;
	private PlayerController _playerController;
	private BlackboardItem _blackboardItemInSight;
	private Vector3 _itemMoveOffset;
	private ItemOrientation _tempOrientation;
	private BlackboardState _currentState;
	private Dictionary<ItemOrientation, float> _orientationAngles;

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		_orientationAngles = new()
		{
			{ ItemOrientation.Up, 0 },
			{ ItemOrientation.UpLeft, 45 },
			{ ItemOrientation.Left, 90 },
			{ ItemOrientation.DownLeft, 135 },
			{ ItemOrientation.Down, 180 },
			{ ItemOrientation.DownRight, 225 },
			{ ItemOrientation.Right, 270 },
			{ ItemOrientation.UpRight, 315 },
		};
	}

	private void Start()
	{
		_playerController = PlayerController.Instance;
		_collider = GetComponent<Collider>();

		UIManager.Instance.SetBlackboardButtons(RotateItem, ApplyRotation, ResetBlackboard);
	}

	public void Behaviour(bool isInteracting, bool isInspecting)
	{
		if (isInteracting && _currentState == BlackboardState.None)
		{
			var currentItem = _playerController.Inventory.SelectedItem();

			if (currentItem && currentItem.TryGetComponent(out BlackboardItem item))
			{
				_playerController.Inventory.Remove(item.gameObject, deactivateObject: false);
				BlackboardItems.Add(item.gameObject);
				SetPos(item);
				HoldItem(item, isFirstPlacement: true);
			}
		}
	}

	private RaycastHit GetHitObject()
	{
		Physics.Raycast(_playerController.Camera.position, _playerController.Camera.forward, out RaycastHit hit, _playerController.PlayerData.InteractDistance, _playerController.PlayerData.InteractLayer);
		return hit;
	}

	private void Update()
	{
		if(_currentState == BlackboardState.Moving && Input.mousePosition != Vector3.zero)
		{
			var hit = GetHitObject();

			if(hit.collider == _collider)
			{
				if (_itemMoveOffset == Vector3.zero) _itemMoveOffset = CurrentItem.transform.position - hit.point;
				CurrentItem.transform.SetPositionAndRotation(hit.point + _itemMoveOffset, Quaternion.Euler(hit.normal.x, hit.normal.y + 90, _orientationAngles[CurrentItem.Orientation]));
			}
			else
			{
				//IF TIME PASSES, THE PLAYER SHOULD SAY TO LOOK AT THE CHALKBOARD
			}
		}

		if(_currentState == BlackboardState.None)
		{
			CheckGlowObject();
		}
	}

	private void CheckGlowObject()
	{
		var playerItemInSight = PlayerController.Instance.InteractableInSight;

		if(playerItemInSight)
		{
			if(_blackboardItemInSight)
			{
				if (_blackboardItemInSight.gameObject == playerItemInSight.gameObject)
				{
					_blackboardItemInSight.Glow(true);
					return;
				}
				else
				{
					_blackboardItemInSight.Glow(false);
					_blackboardItemInSight = null;
				}
			}

			if (BlackboardItems.Contains(playerItemInSight.gameObject))
			{
				_blackboardItemInSight = playerItemInSight.GetComponent<BlackboardItem>();
				_blackboardItemInSight.Glow(true);
			}
		}
		else if(_blackboardItemInSight != null)
		{
			_blackboardItemInSight.Glow(false);
			_blackboardItemInSight = null;
		}
	}

	private void SetPos(BlackboardItem item)
	{
		var hit = GetHitObject();
		item.transform.SetPositionAndRotation(hit.point, Quaternion.Euler(hit.normal.x, hit.normal.y + 90, _orientationAngles[item.Orientation]));
		item.transform.parent = transform.parent;
		item.transform.localScale = Vector3.one;
	}

	public void HoldItem(BlackboardItem item, bool isFirstPlacement = false)
	{
		SetupComponentsForHold(isHolding: true, item);
		StartCoroutine(WaitForMouseUp(item, isFirstPlacement));
	}
	private IEnumerator WaitForMouseUp(BlackboardItem item, bool isFirstPlacement)
	{
		if (isFirstPlacement) yield return new WaitForSecondsRealtime(0.1f);
		yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || _currentState == BlackboardState.None);
		SetupComponentsForHold(isHolding: false);
	}

	public void CheckMouseHold(BlackboardItem selectedItem)
	{
		if(_currentState == BlackboardState.None) StartCoroutine(CheckHold(selectedItem));
	}

	private IEnumerator CheckHold(BlackboardItem selectedItem)
	{
		yield return new WaitForSecondsRealtime(0.1f);

		if (Input.GetMouseButton(0))
		{
			HoldItem(selectedItem);
		}
		else
		{
			_currentState = BlackboardState.Looking;
			CurrentItem = selectedItem;
			_tempOrientation = CurrentItem.Orientation;
			UIManager.Instance.ShowBlackboardImage(sprite: CurrentItem.Sprite, zAngle: _orientationAngles[_tempOrientation]);
			if (_showRotateIconCount > 0)
			{
				UIManager.Instance.ShowRotateItemButton(true);
				_showRotateIconCount--;
			}

			SetupComponentsForLook(isLooking: true);
		}
	}

	private void SetupComponentsForHold(bool isHolding, BlackboardItem item = null)
	{
		if (isHolding)
		{
			_defaultRendererOrder = item.SpriteRenderer.sortingOrder;
			item.SpriteRenderer.sortingOrder = 10;
		}
		else
		{
			CurrentItem.SpriteRenderer.sortingOrder = _defaultRendererOrder;
			_itemMoveOffset = Vector3.zero;
		}

		SetColliderEnabled?.Invoke(this, new() { ColliderEnabled = !isHolding });

		_playerController.FreezePlayerMovement = isHolding;
		CurrentItem = item;
		_currentState = isHolding ? BlackboardState.Moving : BlackboardState.None;
	}

	private void SetupComponentsForLook(bool isLooking)
	{
		_playerController.FreezePlayerMovement = isLooking;
		_playerController.FreezePlayerRotation = isLooking;
		_playerController.ActivateDepthOfField(isLooking);
		CurrentItem.EnableComponents(!isLooking);
	}

	private void RotateItem()
	{
		_tempOrientation = _tempOrientation.Next();
		UIManager.Instance.ShowBlackboardImage(zAngle: _orientationAngles[_tempOrientation]);
		UIManager.Instance.ShowRotateItemButton(false);
	}

	private void ApplyRotation()
	{
		var itemRotation = CurrentItem.transform.eulerAngles;
		itemRotation.z = _orientationAngles[_tempOrientation];
		CurrentItem.transform.eulerAngles = itemRotation;
		CurrentItem.Orientation = _tempOrientation;

		ResetBlackboard();
	}

	private void ResetBlackboard()
	{
		UIManager.Instance.ShowBlackboardImage(false);
		SetupComponentsForLook(false);
		CurrentItem = null;
		_currentState = BlackboardState.None;
	}

	public void CancelHold()
	{
		_currentState = BlackboardState.None;
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

public class BlackboardEventArgs : EventArgs
{
	public bool ColliderEnabled;
}