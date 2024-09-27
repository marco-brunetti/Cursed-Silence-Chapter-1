using System;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public static EventHandler<Collider> ColliderEntered;
    public static EventHandler<Collider> ColliderExited;

    private string detectionTag;

    public void DetectTag(string detectionTag)
    {
        this.detectionTag = detectionTag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(detectionTag))
        {
            ColliderEntered?.Invoke(this, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(detectionTag))
        {
            ColliderExited?.Invoke(this, other);
        }
    }
}
