using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBehaviour
{
    void Behaviour(bool isInteracting, bool isInspecting);
    bool IsInteractable();
    bool IsInspectable();
    GameObject gameObject { get; }
}
