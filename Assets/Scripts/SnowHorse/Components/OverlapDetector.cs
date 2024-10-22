using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnowHorse.Components
{
    public class OverlapDetector : MonoBehaviour
    {
        public static EventHandler<DetectorEventArgs> TagStaying;
        public static EventHandler<DetectorEventArgs> TagEntered;
        public static EventHandler<DetectorEventArgs> TagExited;

        private float radius;
        private float interval;
        private HashSet<string> tags = new();
        [SerializeField] private List<Collider> currentColliders;
        private LayerMask mask;
        private Coroutine sphereCheck;

        public void Init(List<string> detectionTags, float detectionRadius, LayerMask detectionMask, float detectionInterval)
        {
            Reset();
            detectionTags.ForEach(x => tags.Add(x.ToLower()));
            radius = detectionRadius;
            interval = detectionInterval;
            mask = detectionMask;

            sphereCheck = StartCoroutine(SphereCheck());
        }

        public void Reset()
        {
            tags.Clear();
            currentColliders.Clear();
            if (sphereCheck != null) StopCoroutine(sphereCheck);
        }

        private IEnumerator SphereCheck()
        {
            while (true)
            {
                currentColliders = Physics.OverlapSphere(transform.position, radius, mask).Where(x => tags.Contains(x.tag.ToLower())).ToList();
                if (currentColliders.Count > 0) TagStaying?.Invoke(this, new DetectorEventArgs("", null));
                yield return new WaitForSeconds(interval);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!tags.Contains(other.tag.ToLower())) return;
            TagEntered?.Invoke(this, new DetectorEventArgs(other.tag, other));
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