using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelDecorator : MonoBehaviour
{
    [field: SerializeField] public List<DecoratorCompatibility> Compatibility { get; private set; }
    [field: SerializeField] public Vector3 positionOffset { get; private set; }
    [field: SerializeField] public Vector3 rotationOffset { get; private set; }

    [NonSerialized] public bool IsUsed;
}

public enum DecoratorCompatibility
{
    Ceiling,
    Wall,
    Floor
}