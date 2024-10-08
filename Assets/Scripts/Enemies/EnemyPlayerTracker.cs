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

        private bool isPlayerInAttackZone;
        private bool isPlayerInAwareZone;
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
            Detector.TagEntered += OnPlayerEnteredDetector;
            Detector.TagExited += OnPlayerExitedDetector;
            Detector.TagDetectedStart += OnDetectorStart;
            CustomShapeDetector.TagStaying += OnPlayerStayingInDetector;
            this.attackZone.gameObject.SetActive(false);
            this.awareZone.gameObject.SetActive(false);
            this.visualCone.gameObject.SetActive(true);
            this.visualCone.DetectTag("Player");
        }

        public void ActivateDetectors()
        {
            visualCone.gameObject.SetActive(false);
            attackZone.gameObject.SetActive(true);
            awareZone.gameObject.SetActive(true);
            attackZone.Init(new() {"player"}, DetectorShape.Sphere, attackZone.transform.localScale);
            awareZone.Init(new (){"player"}, DetectorShape.Sphere, awareZone.transform.localScale);
        }

        public void DeactivateDetectors()
        {
            visualCone.gameObject.SetActive(false);
            attackZone.gameObject.SetActive(false);
            awareZone.gameObject.SetActive(false);
        }

        public void ActivateVisualCone()
        {
            visualCone.gameObject.SetActive(true);
            attackZone.gameObject.SetActive(false);
            awareZone.gameObject.SetActive(false);
        }
        
        private void CheckConditions(bool isPlayerInAttackZone, bool isPlayerInAwareZone)
        {
            if(isPlayerInAttackZone) Detector.TagDetectedStart -= OnPlayerStayingInDetector;
            
            if(isPlayerInAttackZone != this.isPlayerInAttackZone || isPlayerInAwareZone != this.isPlayerInAwareZone)
            {
                InAttackZone = isPlayerInAttackZone;
                InAwareZone = isPlayerInAwareZone && !isPlayerInAttackZone;
                OutsideZone = !isPlayerInAttackZone && !isPlayerInAwareZone;

                PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(InAttackZone, InAwareZone, OutsideZone));
            }

            this.isPlayerInAttackZone = isPlayerInAttackZone;
            this.isPlayerInAwareZone = isPlayerInAwareZone;
        }

        private void OnPlayerEnteredDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if(triggeredDetector == attackZone) CheckConditions(isPlayerInAttackZone: true, isPlayerInAwareZone);
            else if (triggeredDetector == awareZone) CheckConditions(isPlayerInAttackZone, isPlayerInAwareZone: true);
        }

        private void OnDetectorStart(object sender, DetectorEventArgs args)
        {
            var triggeredDetector = (Detector)sender;
            if(triggeredDetector == attackZone) CheckConditions(isPlayerInAttackZone: true, isPlayerInAwareZone);
        }

        private void OnPlayerStayingInDetector(object sender, EventArgs e)
        {
            if((CustomShapeDetector)sender == visualCone) CheckIfInVisualField();
        }

        private void OnPlayerExitedDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if (triggeredDetector == attackZone) CheckConditions(isPlayerInAttackZone: false, isPlayerInAwareZone);
            else if (triggeredDetector == awareZone) CheckConditions(isPlayerInAttackZone, isPlayerInAwareZone: false);
        }

        private void CheckIfInVisualField()
        {
            visualConeCheckCounter -= Time.deltaTime;

            if(visualConeCheckCounter <= 0)
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

                ActivateDetectors();

                PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(playerEnteredVisualCone: true));
            }
        }
        
        public void Dispose()
        {
            attackZone = null;
            awareZone = null;
            visualCone = null;
            Detector.TagEntered -= OnPlayerEnteredDetector;
            Detector.TagExited -= OnPlayerExitedDetector;
            Detector.TagDetectedStart -= OnPlayerStayingInDetector;
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