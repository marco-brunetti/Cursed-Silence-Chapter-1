using SnowHorse.Utils;
using System;
using System.Collections.Generic;
using Game.Components;
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
        private readonly Enemy enemy;
        private readonly EnemyData data;
        public static EventHandler<EnemyPlayerTrackerArgs> PlayerTrackerUpdated;

        public EnemyPlayerTracker(Enemy enemy, Detector attackZone, Detector awareZone, CustomShapeDetector visualCone, EnemyData data)
        {
            this.enemy = enemy;
            this.attackZone = attackZone;
            this.awareZone = awareZone;
            this.visualCone = visualCone;
            this.data = data;
        }

        public void Start(bool visualConeOnly = false)
        {
            Stop();

            //Detector.TagEntered += OnPlayerEnteredDetector;
            Detector.TagExited += OnPlayerExitedDetector;
            Detector.TagStaying += OnDetectorTick;
            CustomShapeDetector.TagStaying += OnPlayerInsideVisualCone;
            ActivateZones(visualConeOnly);
        }

        public void Stop()
        {
            //Detector.TagEntered -= OnPlayerEnteredDetector;
            Detector.TagExited -= OnPlayerExitedDetector;
            Detector.TagStaying -= OnDetectorTick;
            CustomShapeDetector.TagStaying -= OnPlayerInsideVisualCone;
            if(visualCone) visualCone.gameObject.SetActive(false);
            attackZone.gameObject.SetActive(false);
            awareZone.gameObject.SetActive(false);
            inAttackZone = false;
            inAwareZone = false;
            OutsideZone = false;
        }


        private void ActivateZones(bool visualConeOnly = false)
        {
            if (visualCone) visualCone.gameObject.SetActive(visualConeOnly);
            attackZone.gameObject.SetActive(!visualConeOnly || !visualCone);
            awareZone.gameObject.SetActive(!visualConeOnly || !visualCone);

            if (visualConeOnly && visualCone)
            {
                visualCone.DetectTag("Player");
            }
            else
            {
                attackZone.Init(new List<string> { "player" }, data.DetectionInterval);
                awareZone.Init(new List<string> { "player" }, data.DetectionInterval);
            }
        }



        private void CheckConditions(bool attackZoneDetected, bool awareZoneDetected)
        {
            InAttackZone = attackZoneDetected;
            InAwareZone = awareZoneDetected && !attackZoneDetected;
            OutsideZone = !attackZoneDetected && !awareZoneDetected;

            PlayerTrackerUpdated?.Invoke(this, new EnemyPlayerTrackerArgs(InAttackZone, InAwareZone, OutsideZone));

            inAttackZone = attackZoneDetected;
            inAwareZone = awareZoneDetected;
        }

        private void OnPlayerEnteredDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if (triggeredDetector == attackZone) CheckConditions(attackZoneDetected: true, inAwareZone);
            else if (triggeredDetector == awareZone) CheckConditions(inAttackZone, awareZoneDetected: true);
        }

        private void OnDetectorTick(object sender, DetectorEventArgs args)
        {
            Debug.Log("Detector Tick");
            if ((Detector)sender == attackZone) CheckConditions(attackZoneDetected: true, inAwareZone);
            else if ((Detector)sender == awareZone) CheckConditions(inAttackZone, awareZoneDetected: true);
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
                    Origin = enemy.transform.position,
                    Direction = Camera.main.transform.position - enemy.transform.position,
                    LayerMask = data.DetectionMask,
                    Debug = true
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