using UnityEngine;

namespace Interactables
{
    public class InspectableModifier : MonoBehaviour
    {
        [field: SerializeField] public Vector3 Position { get; private set; }
        [field: SerializeField] public Vector3 Rotation { get; private set; }
        [field: SerializeField] public Vector3 Scale { get; private set; }
        [field: SerializeField] public bool RotateX { get; private set; }
        [field: SerializeField] public bool RotateY { get; private set; }
    }
}