using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Components
{
    public class Detector : MonoBehaviour
    {
        public static EventHandler<DetectorEventArgs> TagStaying;
        public static EventHandler<DetectorEventArgs> TagEntered;
        public static EventHandler<DetectorEventArgs> TagExited;
    
        private List<string> tags = new();
        private float detectionInterval;
        private float currentInterval;

        public void Init(List<string> detectionTags, float detectionInterval = 0.1f)
        {
            tags.Clear();
            this.detectionInterval = detectionInterval;
            detectionTags.ForEach(x=>tags.Add(x.ToLower()));
        }
    
        private void OnTriggerEnter(Collider other)
        {
            if (!tags.Contains(other.tag.ToLower())) return;
            TagEntered?.Invoke(this, new DetectorEventArgs(other.tag, other));
        }
    
        private void OnTriggerStay(Collider other)
        {
            if (currentInterval <= 0)
            {
                if (!tags.Contains(other.tag.ToLower())) return;
                TagStaying?.Invoke(this, new DetectorEventArgs(other.tag, other));
                currentInterval = detectionInterval;
            }
        }

        private void Update()
        {
            if(currentInterval > 0)
            {
                currentInterval -= Time.deltaTime;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!tags.Contains(other.tag.ToLower())) return;
            TagExited?.Invoke(this, new DetectorEventArgs(other.tag, other));
        }
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
}