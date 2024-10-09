using Player;
using SnowHorse.Utils;
using System;
using UnityEngine;

namespace Enemies
{
    public class EnemyPlayerTracker : IDisposable
    {
        public bool InAttackZone { get; private set; }
        public bool InAwareZone { get; private set; }
        public bool OutsideZone { get; private set; }

        private bool inAttackZone;
        private bool inAwareZone;
        private float visualConeCheckCounter;
        private Detector attackZone;
        private Detector awareZone;
        private CustomShapeDetector visualCone;
        private readonly EnemyController controller;
        public static EventHandler<EnemyPlayerTrackerArgs> PlayerTrackerUpdated;

        public EnemyPlayerTracker(EnemyController controller, Detector attackZone, Detector awareZone, CustomShapeDetector visualCone)
        {
            this.controller = controller;
            this.attackZone = attackZone;
            this.awareZone = awareZone;
            this.visualCone = visualCone;
        }

        public void Start(bool visualConeOnly = false)
        {
            Stop();

            Detector.TagEntered += OnPlayerEnteredDetector;
            Detector.TagExited += OnPlayerExitedDetector;
            Detector.TagDetectedStart += OnDetectorStart;
            CustomShapeDetector.TagStaying += OnPlayerInsideVisualCone;
            ActivateZones(visualConeOnly);
            attackZone.Init(new() { "player" }, DetectorShape.Sphere, attackZone.transform.localScale);
            awareZone.Init(new() { "player" }, DetectorShape.Sphere, awareZone.transform.localScale);
            this.visualCone.DetectTag("Player");
        }

        public void Stop()
        {
            Detector.TagEntered -= OnPlayerEnteredDetector;
            Detector.TagExited -= OnPlayerExitedDetector;
            Detector.TagDetectedStart -= OnDetectorStart;
            CustomShapeDetector.TagStaying -= OnPlayerInsideVisualCone;
            visualCone.gameObject.SetActive(false);
            attackZone.gameObject.SetActive(false);
            awareZone.gameObject.SetActive(false);
        }


        private void ActivateZones(bool visualConeOnly = false)
        {
            visualCone.gameObject.SetActive(visualConeOnly);
            attackZone.gameObject.SetActive(!visualConeOnly);
            awareZone.gameObject.SetActive(!visualConeOnly);
        }



        private void CheckConditions(bool attackZoneDetected, bool awareZoneDetected)
        {
            if (attackZoneDetected != inAttackZone || awareZoneDetected != inAwareZone)
            {
                InAttackZone = attackZoneDetected;
                InAwareZone = awareZoneDetected && !attackZoneDetected;
                OutsideZone = !attackZoneDetected && !awareZoneDetected;

                PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(InAttackZone, InAwareZone, OutsideZone));
            }

            inAttackZone = attackZoneDetected;
            inAwareZone = awareZoneDetected;
        }

        private void OnPlayerEnteredDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if (triggeredDetector == attackZone) CheckConditions(attackZoneDetected: true, inAwareZone);
            else if (triggeredDetector == awareZone) CheckConditions(inAttackZone, awareZoneDetected: true);
        }

        private void OnDetectorStart(object sender, DetectorEventArgs args)
        {
            if ((Detector)sender == attackZone) CheckConditions(attackZoneDetected: true, inAwareZone);
        }

        private void OnPlayerInsideVisualCone(object sender, EventArgs e)
        {
            if ((CustomShapeDetector)sender == visualCone) CheckIfInVisualField();
        }

        private void OnPlayerExitedDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if (triggeredDetector == attackZone) CheckConditions(attackZoneDetected: false, inAwareZone);
            else if (triggeredDetector == awareZone) CheckConditions(inAttackZone, awareZoneDetected: false);
        }

        private void CheckIfInVisualField()
        {
            visualConeCheckCounter -= Time.deltaTime;

            if (visualConeCheckCounter <= 0)
            {
                var tagData = new RaycastData
                {
                    FindTag = "Player",
                    Origin = controller.transform.position,
                    Direction = PlayerController.Instance.Camera.transform.position - controller.transform.position,
                    LayerMask = controller.EnemyData.DetectionMask,
                    //Debug = true
                };

                visualConeCheckCounter = 0.1f;
                if (Raycaster.FindWithTag<GameObject>(tagData) == null) return;

                ActivateZones(visualConeOnly: false);

                PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(playerEnteredVisualCone: true));
            }
        }

        public void Dispose()
        {
            Stop();
            attackZone = null;
            awareZone = null;
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