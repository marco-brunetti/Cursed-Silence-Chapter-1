using UnityEngine;

namespace Interactables
{
    public class PlayerInspectModifier : MonoBehaviour
    {
        [field: SerializeField] public Vector3 Position { get; private set; }
        [field: SerializeField] public Vector3 Rotation { get; private set; }
        [field: SerializeField] public Vector3 Scale { get; private set; }
        [field: SerializeField] public bool RotateX { get; private set; }
        [field: SerializeField] public bool RotateY { get; private set; }
        [field: SerializeField] public bool InvertX { get; private set; }
        [field: SerializeField] public bool InvertY { get; private set; }

        [field: SerializeField, Range(0.5f, 2f)]
        public float SensitivityX { get; private set; } = 1f;

        [field: SerializeField, Range(0.5f, 2f)]
        public float SensitivityY { get; private set; } = 1f;
    }
}