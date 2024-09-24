using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnowHorse.Utils;
using System.Linq;
using Player;

public class BlackboardController : MonoBehaviour, IBehaviour
{
	[field: SerializeField] public List<GameObject> BlackboardItems { get; private set; } = new();

	public BlackboardItem CurrentItem { get; private set; }
	public EventHandler<BlackboardEventArgs> SetColliderEnabled;
	public static BlackboardController Instance;

	private int _showRotateIconCount = 3;
	private int _defaultRendererOrder;
	private Collider _collider;
	private PlayerController _playerController;
	private BlackboardItem _currentItemInSight;
	private Vector3 _itemMoveOffset;
	private ItemOrientation _uiOrientation;
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
			var item = _playerController.Inventory.Contains<BlackboardItem>(removeItem:true, destroyItem:false);
			if(!item) return;

			BlackboardItems.Add(item.gameObject);
			SetPos(item);
			HoldItem(item, isFirstPlacement: true);
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
		var newItemInSight = _playerController.InteractableInSight;

		if(newItemInSight)
		{
			if(_currentItemInSight)
			{
				if (_currentItemInSight.gameObject == newItemInSight.gameObject)
				{
					_currentItemInSight.Glow(true);
					return;
				}
				else
				{
					_currentItemInSight.Glow(false);
					_currentItemInSight = null;
				}
			}

			if (BlackboardItems.Contains(newItemInSight.gameObject))
			{
				_currentItemInSight = newItemInSight.GetComponent<BlackboardItem>();
				_currentItemInSight.Glow(true);
			}
		}
		else if(_currentItemInSight != null)
		{
			_currentItemInSight.Glow(false);
			_currentItemInSight = null;
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
		if (isFirstPlacement) yield return new WaitForSecondsRealtime(0.5f);
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
			_uiOrientation = CurrentItem.Orientation;
			UIManager.Instance.ShowBlackboardImage(sprite: CurrentItem.Sprite, zAngle: _orientationAngles[_uiOrientation]);

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
		_uiOrientation = _uiOrientation.Next();
		UIManager.Instance.ShowBlackboardImage(zAngle: _orientationAngles[_uiOrientation]);
		UIManager.Instance.ShowRotateItemButton(false);
	}

	private void ApplyRotation()
	{
		var itemRotation = CurrentItem.transform.eulerAngles;
		itemRotation.z = _orientationAngles[_uiOrientation];
		CurrentItem.transform.eulerAngles = itemRotation;
		CurrentItem.Orientation = _uiOrientation;

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

	public void SnapDetected(bool isSnapping, BlackboardItemSnap thisSnap, BlackboardItemSnap otherSnap)
	{
		var validSnap = otherSnap.Id == thisSnap.Id && (thisSnap.isBaseSnapPoint || otherSnap.isBaseSnapPoint);
		if (!validSnap) return;

		var isOnBlackboard = BlackboardItems.Contains(otherSnap.BlackboardItem.gameObject);
		var validItem = thisSnap.BlackboardItem == CurrentItem && thisSnap.BlackboardItem.Orientation == otherSnap.BlackboardItem.Orientation;

		if (validItem && isOnBlackboard)
		{
			RegisterSnap(isSnapping, thisSnap);
			RegisterSnap(isSnapping, otherSnap);

			if (isSnapping)
			{
				CancelHold();
				var snapOffset = thisSnap.BlackboardItem.transform.position - thisSnap.transform.position;
				thisSnap.BlackboardItem.transform.position = otherSnap.transform.position + snapOffset;
			}
		}
	}

	private void RegisterSnap(bool isSnapped, BlackboardItemSnap snap)
	{
		var item = snap.BlackboardItem;

		if (!item.Snaps.Contains(snap))
		{
			Debug.Log($"Incorrect snap registered in {gameObject}!");
			return;
		}

		if (isSnapped)
		{
			if (item.SnappedPoints.Contains(snap)) return;
			else item.SnappedPoints.Add(snap);
		}
		else
		{
			item.SnappedPoints.Remove(snap);
		}

		if (item.SnappedPoints.Count == item.Snaps.Length)
		{
			item.OnSetColliderEnabled(null, new() { ColliderEnabled = false });
			item.SpriteRenderer.enabled = false;

			if (item.FullPage)
			{
				item.FullPage.Orientation = item.Orientation;
				BlackboardItems.Add(item.FullPage.gameObject);
				item.FullPage.gameObject.SetActive(true);
			}
			item.IsFullySnapped = true;
		}
	}

	public bool IsInspectable() { return false; }
	public bool IsInteractable() { return true; }
}

public enum BlackboardState
{
	None,
	Looking,
	Moving
}

public class BlackboardEventArgs : EventArgs
{
	public bool ColliderEnabled;
}