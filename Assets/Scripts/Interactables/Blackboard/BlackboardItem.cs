using System;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Interactables.Behaviours
{
    public class BlackboardItem : MonoBehaviour, IBehaviour
    {
        public ItemOrientation Orientation = ItemOrientation.Up;
        [SerializeField] private GameObject _glow;
        [field: SerializeField] public BlackboardItem FullPage { get; private set; }
        public BlackboardItemSnap[] Snaps { get; private set; }
        public List<BlackboardItemSnap> SnappedPoints = new();

        [NonSerialized] public bool IsFullySnapped;
        [NonSerialized] public SpriteRenderer SpriteRenderer;
        [NonSerialized] public Sprite Sprite;
        [NonSerialized] public Collider[] Colliders;

        private BlackboardController _controller;

        private void Awake()
        {
            Colliders = GetComponents<Collider>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Sprite = SpriteRenderer.sprite;
            Snaps = GetComponentsInChildren<BlackboardItemSnap>();
        }

        private void Start()
        {
            _controller = BlackboardController.Instance;
            _controller.SetColliderEnabled += OnSetColliderEnabled;
            Array.ForEach(Snaps, snap => snap.SetSnapAction(_controller.SnapDetected, this));
        }

        public void Behaviour(bool isInteracting, bool isInspecting)
        {
            if (isInteracting && _controller.BlackboardItems.Contains(gameObject))
            {
                var item = PlayerController.Instance.Inventory.Contains<BlackboardItem>(removeItem: false,
                    destroyItem: false);

                if (item)
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

        public void OnSetColliderEnabled(object sender, BlackboardEventArgs e)
        {
            if (!IsFullySnapped) Array.ForEach(Colliders, x => x.enabled = e.ColliderEnabled);
        }

        public bool IsInspectable()
        {
            return false;
        }

        public bool IsInteractable()
        {
            return true;
        }
    }
}

public enum ItemOrientation
{
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
}