using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
	public ItemOrientation Orientation = ItemOrientation.Up;
	[SerializeField] private BlackboardItem _fullPage;
	[SerializeField] private GameObject _glow;

	[NonSerialized] public SpriteRenderer SpriteRenderer;
	[NonSerialized] public Sprite Sprite;
	[NonSerialized] public Collider[] Colliders;
	[SerializeField] private BlackboardItemSnap[] _snaps;
	[SerializeField] private List<BlackboardItemSnap> _snappedPoints = new();
	
	private bool _isFullySnapped;
	private BlackboardController _controller;

	private void Awake()
	{
		SpriteRenderer = GetComponent<SpriteRenderer>();
		Sprite = SpriteRenderer.sprite;
		Colliders = GetComponents<Collider>();

		_snaps = GetComponentsInChildren<BlackboardItemSnap>();
		Array.ForEach(_snaps, snap => snap.SetSnapAction(SnapDetected, this));
	}

	private void Start()
	{
		_controller = BlackboardController.Instance;
		_controller.SetColliderEnabled += OnSetColliderEnabled;
	}

	public void Behaviour(bool isInteracting, bool isInspecting)
	{
		if(isInteracting && _controller.BlackboardItems.Contains(gameObject))
		{
			var currentItem = PlayerController.Instance.Inventory.SelectedItem();
			if (currentItem && currentItem.TryGetComponent(out BlackboardItem item))
			{
				Debug.Log("Put subtitle for placing somewhere else");
			}
			else
			{
				_controller.CheckMouseHold(this);
			}
		}
	}

	public void EnableComponents(bool enable)
	{
		SpriteRenderer.enabled = enable;
		Array.ForEach(Colliders, x => x.enabled = enable);
	}

	public void Glow(bool enable)
	{
		_glow.SetActive(enable);
	}

	private void OnSetColliderEnabled(object sender, BlackboardEventArgs e)
	{
		if(!_isFullySnapped) Array.ForEach(Colliders, x => x.enabled = e.ColliderEnabled);
	}


	private void SnapDetected(bool isSnapping, BlackboardItemSnap thisSnap, BlackboardItemSnap otherSnap)
	{
		var validSnap = otherSnap.Id == thisSnap.Id && (thisSnap.isBaseSnapPoint || otherSnap.isBaseSnapPoint);
		var isOnBlackboard = _controller.BlackboardItems.Contains(otherSnap.BlackboardItem.gameObject);
		var validItem = this == _controller.CurrentItem && Orientation == otherSnap.BlackboardItem.Orientation;

		if (validSnap && validItem && isOnBlackboard)
		{
			RegisterSnap(isSnapping, thisSnap);
			otherSnap.BlackboardItem.RegisterSnap(isSnapping, otherSnap);

			if(isSnapping)
			{
				_controller.CancelHold();
				var snapOffset = transform.position - thisSnap.transform.position;
				transform.position = otherSnap.transform.position + snapOffset;
			}
		}
	}

	public void RegisterSnap(bool isSnapped, BlackboardItemSnap snap)
	{
		if(isSnapped)
		{
			if (_snappedPoints.Contains(snap)) return;
			else _snappedPoints.Add(snap);
		}
		else
		{
			_snappedPoints.Remove(snap);
		}    

		if(_snappedPoints.Count == _snaps.Length)
		{
			OnSetColliderEnabled(null, new() { ColliderEnabled = false });
			SpriteRenderer.enabled = false;
			//BlackboardController.Instance.BlackboardItems.Remove(gameObject);

			if (_fullPage)
			{
				_fullPage.Orientation = Orientation;
				_controller.BlackboardItems.Add(_fullPage.gameObject);
				_fullPage.gameObject.SetActive(true);
			}
			_isFullySnapped = true;
		}
	}

	public bool IsInspectable() { return false; }
	public bool IsInteractable() { return true; }
}

public enum ItemOrientation
{
	Up,
	Left,
	Down,
	Right
}