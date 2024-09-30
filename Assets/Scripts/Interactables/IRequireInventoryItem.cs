using UnityEngine;
using System.Collections.Generic;

public interface IRequireInventoryItem
{
    public List<GameObject> RequiredObjects { get; }
}