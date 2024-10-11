using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public static EventHandler<DetectorEventArgs> TagDetectedStart;
    public static EventHandler<DetectorEventArgs> TagEntered;
    public static EventHandler<DetectorEventArgs> TagExited;
    
    private List<string> tags = new();
    private List<Collider> currentColliders = new();
    private Coroutine checkCollidersActive;
    private WaitForSeconds detectionWait;
    private bool getColliders;

    public void Init(List<string> detectionTags, DetectorShape shape, Vector3 scale, float detectionInterval = 0.1f, bool getColliders = false)
    {
        Stop();
        
        this.getColliders = getColliders;
        detectionTags.ForEach(x=>tags.Add(x.ToLower()));
        currentColliders = GetColliders(detectionTags, shape, scale);
        OnTriggerInit();
        detectionWait = new WaitForSeconds(detectionInterval);
        checkCollidersActive = StartCoroutine(CheckCollidersActive());
    }

    private List<Collider> GetColliders(List<string> detectionTags, DetectorShape shape, Vector3 scale)
    {
        return shape switch
        {
            DetectorShape.Box => Physics.OverlapBox(transform.position, scale / 2)
                .Where(x => detectionTags.Contains(x.tag)).ToList(),
            DetectorShape.Sphere => Physics.OverlapSphere(transform.position, scale.x / 2)
                .Where(x => detectionTags.Contains(x.tag)).ToList(),
            _ => Physics.OverlapSphere(transform.position, scale.x / 2).Where(x => detectionTags.Contains(x.tag))
                .ToList()
        };
    }

    public void Stop()
    {
        currentColliders.Clear();
        tags.Clear();
        detectionWait = null;
        if(checkCollidersActive != null) StopCoroutine(checkCollidersActive);
    }

    public void OnTriggerInit()
    {
        foreach (var col in currentColliders)
        {
            DetectorEventArgs args = getColliders ? new(col.tag.ToLower(), col) : new(col.tag.ToLower());
            TagDetectedStart?.Invoke(this, args);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(tags.Contains(other.tag.ToLower()))
        {
            if(!currentColliders.Contains(other)) currentColliders.Add(other);
            DetectorEventArgs args = getColliders ? new(other.tag, other) : new(other.tag);
            TagEntered?.Invoke(this, args);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(tags.Contains(other.tag.ToLower()))
        {
            if(currentColliders.Contains(other)) currentColliders.Remove(other);
            DetectorEventArgs args = getColliders ? new(other.tag, other) : new(other.tag);
            TagExited?.Invoke(this, args);
        }
    }

    private IEnumerator CheckCollidersActive()
    {
        while (true)
        {
            yield return detectionWait;
            foreach (var col in currentColliders.ToList())
            {
                if (!col || !col.gameObject.activeInHierarchy)
                {
                    currentColliders.Remove(col);
                    OnTriggerInit();
                }
            }
            yield return null;
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