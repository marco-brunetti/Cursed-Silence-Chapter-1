using UnityEngine;

namespace Player
{
    public interface IInventoryItem
    {
        public Sprite UiIcon { get; }
        public GameObject gameObject { get; }
    }
}