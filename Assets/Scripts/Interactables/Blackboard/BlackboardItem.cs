using System;
using UnityEngine;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
    public ItemOrientation Orientation = ItemOrientation.Up;
    private SpriteRenderer _spriteRenderer;
    [NonSerialized] public Sprite Sprite;
    [NonSerialized] public Collider Collider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite = _spriteRenderer.sprite;
        Collider = GetComponent<Collider>();
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && BlackboardController.Instance.BlackboardItems.Contains(this))
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
        _spriteRenderer.enabled = enable;
        Collider.enabled = enable;
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