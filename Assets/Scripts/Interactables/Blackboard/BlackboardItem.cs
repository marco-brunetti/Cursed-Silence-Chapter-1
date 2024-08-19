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
    private bool _isFullySnapped;
    [SerializeField] private List<BlackboardItemSnap> _snaps = new();
    [SerializeField] private List<BlackboardItemSnap> _snappedPoints = new();

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Sprite = SpriteRenderer.sprite;
        Colliders = GetComponents<Collider>();

        _snaps = GetComponentsInChildren<BlackboardItemSnap>().ToList();
    }

    private void Start()
    {
        BlackboardController.Instance.SetColliderEnabled += OnSetColliderEnabled;
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && BlackboardController.Instance.BlackboardItems.Contains(gameObject))
        {
            var currentItem = PlayerController.Instance.Inventory.SelectedItem();
            if (currentItem && currentItem.TryGetComponent(out BlackboardItem item))
            {
                Debug.Log("Put subtitle for placing somewhere else");
            }
            else
            {
                BlackboardController.Instance.CheckMouseHold(this);
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


        if(_snappedPoints.Count == _snaps.Count)
        {
            OnSetColliderEnabled(null, new() { ColliderEnabled = false });
            SpriteRenderer.enabled = false;
            //BlackboardController.Instance.BlackboardItems.Remove(gameObject);

            if (_fullPage)
            {
                _fullPage.Orientation = Orientation;
                BlackboardController.Instance.BlackboardItems.Add(_fullPage.gameObject);
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