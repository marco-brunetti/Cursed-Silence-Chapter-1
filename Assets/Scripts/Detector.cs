using System;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public static EventHandler TagEntered;
    public static EventHandler TagExited;
    public static EventHandler TagStaying;
    
    public static EventHandler<Collider> ColliderEntered;
    public static EventHandler<Collider> ColliderExited;
    public static EventHandler<Collider> ColliderStaying;

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
            TagEntered?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag(detectionTag))
        {
            ColliderExited?.Invoke(this, other);
            TagExited?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag(detectionTag))
        {
            ColliderStaying?.Invoke(this, other);
            TagStaying?.Invoke(this, EventArgs.Empty);
        }
    }
}
