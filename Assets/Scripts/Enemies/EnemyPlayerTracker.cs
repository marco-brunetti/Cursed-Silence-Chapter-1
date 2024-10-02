using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyPlayerTracker : MonoBehaviour
    {
        public bool IsPlayerInInnerZone { get; private set; }
        public bool IsPlayerInOuterZone { get; private set; }
        public bool IsPlayerOutsideDetectors { get; private set; }

        private bool isPlayerInner;
        private bool isPlayerOuter;
        private Detector innerPlayerDetector;
        private Detector outerPlayerDetector;

        public static EventHandler<EnemyPlayerTrackerArgs> PlayerTrackerUpdated;

        public void Init(Detector innerDetector, Detector outerDetector)
        {
            innerPlayerDetector = innerDetector;
            outerPlayerDetector = outerDetector;

            innerPlayerDetector.DetectTag("Player");
            outerPlayerDetector.DetectTag("Player");
            Detector.ColliderEntered += PlayerEnteredDetector;
            Detector.ColliderExited += PlayerExitedDetector;
            Detector.ColliderStaying += PlayerStayingInDetector;
            innerPlayerDetector.gameObject.SetActive(true);
            outerPlayerDetector.gameObject.SetActive(true);
        }

        public void Terminate()
        {

        }

        private void CheckConditions(bool isPlayerInner, bool isPlayerOuter)
        {
            if(isPlayerInner != this.isPlayerInner || isPlayerOuter != this.isPlayerOuter)
            {
                IsPlayerInInnerZone = isPlayerInner;
                IsPlayerInOuterZone = isPlayerOuter && !isPlayerInner;
                IsPlayerOutsideDetectors = !isPlayerInner && !isPlayerOuter;

                PlayerTrackerUpdated?.Invoke(null, new(IsPlayerInInnerZone, IsPlayerInOuterZone, IsPlayerOutsideDetectors));
            }

            this.isPlayerInner = isPlayerInner;
            this.isPlayerOuter = isPlayerOuter;
        }

        public void PlayerEnteredDetector(object sender, Collider other)
        {
            var triggeredDetector = (Detector)sender;

            if(triggeredDetector == innerPlayerDetector) CheckConditions(isPlayerInner:true, isPlayerOuter);
            else if (triggeredDetector == outerPlayerDetector) CheckConditions(isPlayerInner, isPlayerOuter: true);
        }

        public void PlayerStayingInDetector(object sender, Collider other)
        {
            var triggeredDetector = (Detector)sender;

            if (triggeredDetector == innerPlayerDetector) CheckConditions(isPlayerInner: true, isPlayerOuter);
            else if (triggeredDetector == outerPlayerDetector) CheckConditions(isPlayerInner, isPlayerOuter: true);
        }

        public void PlayerExitedDetector(object sender, Collider other)
        {
            var triggeredDetector = (Detector)sender;

            if (triggeredDetector == innerPlayerDetector) CheckConditions(isPlayerInner: false, isPlayerOuter);
            else if (triggeredDetector == outerPlayerDetector) CheckConditions(isPlayerInner, isPlayerOuter: false);
        }


        private void OnDisable()
        {
            Detector.ColliderEntered -= PlayerEnteredDetector;
            Detector.ColliderExited -= PlayerExitedDetector;
            Detector.ColliderStaying -= PlayerStayingInDetector;
        }
    }

    public class EnemyPlayerTrackerArgs : EventArgs
    {
        public bool IsPlayerInInnerZone { get; private set; }
        public bool IsPlayerInOuterZone { get; private set; }
        public bool IsPlayerOutsideDetectors { get; private set; }

        public EnemyPlayerTrackerArgs(bool isPlayerInInnerZone, bool isPlayerInOuterZone, bool isPlayerOutsideDetectors)
        {
            IsPlayerInInnerZone = isPlayerInInnerZone;
            IsPlayerInOuterZone = isPlayerInOuterZone;
            IsPlayerOutsideDetectors = isPlayerOutsideDetectors;
        }
    }

}