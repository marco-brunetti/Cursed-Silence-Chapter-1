using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Detector : MonoBehaviour
{
    public static EventHandler<DetectorEventArgs> TagDetectedTick;
    public static EventHandler<DetectorEventArgs> TagEntered;
    public static EventHandler<DetectorEventArgs> TagExited;
    
    private List<string> tags = new();
    private List<Collider> currentColliders = new();
    private Coroutine checkCollidersActive;
    private WaitForSeconds interval;
    private DetectorShape shape;
    private Vector3 scale;
    bool returnColliders;

    public void Init(List<string> detectionTags, DetectorShape shape, Vector3 scale, float detectionInterval = 0.1f, bool returnColliders = false)
    {
        Stop();
        this.returnColliders = returnColliders;
        this.shape = shape;
        this.scale = scale;
        detectionTags.ForEach(x=>tags.Add(x.ToLower()));
        interval = new WaitForSeconds(detectionInterval);
        checkCollidersActive = StartCoroutine(CheckCollidersActive());
    }

    private List<Collider> GetColliders(List<string> detectionTags, DetectorShape shape, Vector3 scale)
    {
        return shape switch
        {
            DetectorShape.Box => Physics.OverlapBox(transform.position, scale / 2)
                .Where(x => detectionTags.Contains(x.tag.ToLower())).ToList(),
            DetectorShape.Sphere => Physics.OverlapSphere(transform.position, scale.x / 2)
                .Where(x => detectionTags.Contains(x.tag.ToLower())).ToList(),
            _ => Physics.OverlapSphere(transform.position, scale.x / 2).Where(x => detectionTags.Contains(x.tag.ToLower()))
                .ToList()
        };
    }

    public void Stop()
    {
        currentColliders.Clear();
        tags.Clear();
        interval = null;
        if(checkCollidersActive != null) StopCoroutine(checkCollidersActive);
    }

    public void OnTriggerTick()
    {
        currentColliders = GetColliders(tags, shape, scale);

        foreach (var col in currentColliders)
        {
            DetectorEventArgs args = returnColliders ? new(col.tag.ToLower(), col) : new(col.tag.ToLower());
            TagDetectedTick?.Invoke(this, args);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(tags.Contains(other.tag.ToLower()))
        {
            if(!currentColliders.Contains(other)) currentColliders.Add(other);
            DetectorEventArgs args = returnColliders ? new(other.tag, other) : new(other.tag);
            TagEntered?.Invoke(this, args);
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if(tags.Contains(other.tag.ToLower()))
        {
            if(currentColliders.Contains(other)) currentColliders.Remove(other);
            DetectorEventArgs args = returnColliders ? new(other.tag, other) : new(other.tag);
            TagExited?.Invoke(this, args);
        }
    }

    private IEnumerator CheckCollidersActive()
    {
        while (true)
        {
            OnTriggerTick();
            yield return interval;
        }
    }
}

public enum DetectorShape
{
    Box,
    Sphere,
    Capsule
}

public class DetectorEventArgs : EventArgs
{
    public string Tag { get; private set; }
    public Collider Collider { get; private set; }

    public DetectorEventArgs(string tag = "", Collider collider = null)
    {
        Tag = tag;
        Collider = collider;
    }
}