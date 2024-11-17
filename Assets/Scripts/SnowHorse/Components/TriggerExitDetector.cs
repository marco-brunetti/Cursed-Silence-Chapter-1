using System;
using System.Collections.Generic;
using UnityEngine;

namespace SnowHorse.Components
{
    public class TriggerExitDetector : MonoBehaviour
    {
        public EventHandler<DetectorEventArgs> TagExited;

        private List<string> tags = new();

        public void Init(List<string> detectionTags)
        {
            tags.Clear();
            detectionTags.ForEach(x => tags.Add(x.ToLower()));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!tags.Contains(other.tag.ToLower())) return;
            TagExited?.Invoke(this, new DetectorEventArgs(other.tag, other));
        }
    }
}