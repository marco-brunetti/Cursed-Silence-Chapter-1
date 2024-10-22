using System;
using UnityEngine;

namespace SnowHorse.Components
{
    public class CustomShapeDetector : MonoBehaviour
    {
        public static EventHandler TagStaying;
        public static EventHandler<Collider> ColliderStaying;

        private string detectionTag;

        public void DetectTag(string detectionTag) => this.detectionTag = detectionTag;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(detectionTag)) TagStaying?.Invoke(this, EventArgs.Empty);
        }
    }
}