using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelDecorator : MonoBehaviour
{
    [field: SerializeField] public List<DecoratorCompatibility> Compatibility { get; private set; }
    [field: SerializeField] public Vector3 Position { get; private set; }
    [field: SerializeField] public Vector3 Rotation { get; private set; }

    [field: SerializeField] private Vector3 scale;

    [NonSerialized] public bool IsUsed;
    public Vector3 Scale { get { return scale == Vector3.zero ? transform.localScale : scale; } }
}

public enum DecoratorCompatibility
{
    Ceiling,
    Wall,
    Floor
}