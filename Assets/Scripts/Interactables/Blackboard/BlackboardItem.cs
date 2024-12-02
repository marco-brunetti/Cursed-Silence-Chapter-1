using System;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Interactables.Behaviours
{
    public class BlackboardItem : Behaviour
    {
        public ItemOrientation Orientation = ItemOrientation.Up;
        [field: SerializeField] public BlackboardItem FullPage { get; private set; }
        public BlackboardItemSnap[] Snaps { get; private set; }
        public List<BlackboardItemSnap> SnappedPoints = new();

        [NonSerialized] public bool IsFullySnapped;
        [NonSerialized] public SpriteRenderer SpriteRenderer;
        [NonSerialized] public Sprite Sprite;
        [NonSerialized] private Collider[] Colliders;

        private BlackboardController controller;
        private readonly int emissionMap = Shader.PropertyToID("_EmissionMap");

        private void Awake()
        {
            Colliders = GetComponents<Collider>();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Sprite = SpriteRenderer.sprite;
            Snaps = GetComponentsInChildren<BlackboardItemSnap>();
        }

        private void Start()
        {
            controller = BlackboardController.Instance;
            controller.SetColliderEnabled += OnSetColliderEnabled;
            Array.ForEach(Snaps, snap => snap.SetSnapAction(controller.SnapDetected, this));
        }

        public override void Activate()
        {
            if (!controller.BlackboardItems.Contains(gameObject)) return;
            
            if (PlayerController.Instance.Inventory.Find<BlackboardItem>(removeFromInventory: false, out var item))
            {
                Debug.Log("Put subtitle for placing somewhere else");
            }
            else
            {
                controller.CheckMouseHold(this);
            }
        }

        public void EnableComponents(bool enable)
        {
            SpriteRenderer.enabled = enable;
            Array.ForEach(Colliders, x => x.enabled = enable);
        }

        public void Glow(bool enable)
        {
            SpriteRenderer.material = enable ? controller.GlowPageMaterial : controller.DefaultPageMaterial;
            if(enable) SpriteRenderer.material.SetTexture(emissionMap, SpriteRenderer.sprite.texture);
        }

        public void OnSetColliderEnabled(object sender, BlackboardEventArgs e)
        {
            if (!IsFullySnapped) Array.ForEach(Colliders, x => x.enabled = e.ColliderEnabled);
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