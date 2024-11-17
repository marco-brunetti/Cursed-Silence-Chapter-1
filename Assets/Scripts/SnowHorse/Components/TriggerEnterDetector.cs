using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnowHorse.Components
{
    public class TriggerEnterDetector : MonoBehaviour
    {
        public EventHandler<DetectorEventArgs> TagEntered;

        private List<string> tags = new();

        public void Init(List<string> detectionTags)
        {
            tags.Clear();
            detectionTags.ForEach(x => tags.Add(x.ToLower()));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!tags.Contains(other.tag.ToLower())) return;
            TagEntered?.Invoke(this, new DetectorEventArgs(other.tag, other));
        }
    }
}