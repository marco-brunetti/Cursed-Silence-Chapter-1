using System;
using UnityEngine;

public class BlackboardItem : MonoBehaviour, IBehaviour
{
    [field: SerializeField] public int PageNumber { get; private set; }
    public ItemOrientation Orientation = ItemOrientation.Up;

    private Collider _collider;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;
    private BlackboardController _blackboardController;

    [NonSerialized] public Sprite Sprite;
    [NonSerialized] public Vector3 MoveOffset;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite = _spriteRenderer.sprite;
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        _blackboardController = BlackboardController.Instance;
        _playerController = PlayerController.Instance;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _blackboardController.GetOrientationAngle(Orientation));
    }

    public void Behaviour(bool isInteracting, bool isInspecting)
    {
        if(isInteracting && _blackboardController.BlackboardItems.Contains(this))
        {
            StartCoroutine(_blackboardController.CheckMouseHold(this));
        }
    }

    public void EnableComponents(bool enable)
    {
        _spriteRenderer.enabled = enable;
        _collider.enabled = enable;
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