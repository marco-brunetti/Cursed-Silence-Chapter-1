using SnowHorse.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemyPlayerTracker : IDisposable
    {
        private bool inAttackZone;
        private bool inAwareZone;
        private bool outsideZones;
        private float visualConeCheckCounter;
        private CustomShapeDetector visualCone;
        private readonly Enemy enemy;
        private readonly EnemyData data;
        public static EventHandler<EnemyPlayerTrackerArgs> PlayerTrackerUpdated;
        private readonly Transform playerTransform;
        private Coroutine trackerTick;
        private WaitForSeconds baseTrackInterval;
        private WaitForSeconds onAttackTrackInterval = new(0.1f);

        public EnemyPlayerTracker(Enemy enemy, Transform player, CustomShapeDetector visualCone, EnemyData data)
        {
            this.enemy = enemy;
            this.visualCone = visualCone;
            this.data = data;
            playerTransform = player;
            baseTrackInterval = new WaitForSeconds(data.DetectionInterval);
        }

        public void Start(bool visualConeOnly = false)
        {
            Stop();
            
            if (visualCone)
            {
                if (visualConeOnly)
                {
                    CustomShapeDetector.TagStaying += OnPlayerInsideVisualCone;
                    visualCone.DetectTag("Player");
                }
                visualCone.gameObject.SetActive(visualConeOnly);
            }

            if (!visualConeOnly) trackerTick = enemy.StartCoroutine(TrackerTick());
        }

        public void Stop()
        {
            if(trackerTick != null) enemy.StopCoroutine(trackerTick);
            CustomShapeDetector.TagStaying -= OnPlayerInsideVisualCone;
            if(visualCone) visualCone.gameObject.SetActive(false);
            inAttackZone = false;
            inAwareZone = false;
            outsideZones = false;
        }

        private IEnumerator TrackerTick()
        {
            while (true)
            {
                if (playerTransform)
                {
                    var distance = Vector3.Distance(enemy.transform.position, playerTransform.position);

                    inAttackZone = distance <= data.MaxAttackDistance;
                    inAwareZone = distance <= data.MaxAwareDistance && distance > data.MaxAttackDistance;
                    outsideZones = distance > data.MaxAwareDistance;
                    
                    PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(inAttackZone, inAwareZone, outsideZones));

                    if (inAttackZone) yield return onAttackTrackInterval;
                    else yield return baseTrackInterval;
                }

                yield return null;
            }
        }

        private void OnPlayerInsideVisualCone(object sender, EventArgs e)
        {
            if ((CustomShapeDetector)sender == visualCone) CheckIfInVisualField();
        }

        private void CheckIfInVisualField()
        {
            visualConeCheckCounter -= Time.deltaTime;

            if (visualConeCheckCounter <= 0)
            {
                var tagData = new RaycastData
                {
                    FindTag = "Player",
                    Origin = enemy.transform.position,
                    Direction = Camera.main.transform.position - enemy.transform.position,
                    LayerMask = data.DetectionMask,
                    Debug = true
                };

                visualConeCheckCounter = 0.1f;
                if (Raycaster.FindWithTag<GameObject>(tagData) == null) return;

                Start(visualConeOnly: false);

                PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(playerEnteredVisualCone: true));
            }
        }

        public void Dispose()
        {
            Stop();
            visualCone = null;
        }
    }

    public class EnemyPlayerTrackerArgs : EventArgs
    {
        public bool IsPlayerInInnerZone { get; private set; }
        public bool IsPlayerInOuterZone { get; private set; }
        public bool IsPlayerOutsideDetectors { get; private set; }
        public bool PlayerEnteredVisualCone { get; private set; }

        public EnemyPlayerTrackerArgs(bool isPlayerInInnerZone = false, bool isPlayerInOuterZone = false, bool isPlayerOutsideDetectors = false, bool playerEnteredVisualCone = false)
        {
            IsPlayerInInnerZone = isPlayerInInnerZone;
            IsPlayerInOuterZone = isPlayerInOuterZone;
            IsPlayerOutsideDetectors = isPlayerOutsideDetectors;
            PlayerEnteredVisualCone = playerEnteredVisualCone;
        }
    }
}