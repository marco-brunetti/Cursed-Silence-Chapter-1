using System;

namespace Enemies
{
    public class EnemyPlayerTracker : IDisposable
    {
        public bool IsPlayerInInnerZone { get; private set; }
        public bool IsPlayerInOuterZone { get; private set; }
        public bool IsPlayerOutsideDetectors { get; private set; }

        private bool isPlayerInner;
        private bool isPlayerOuter;
        private Detector innerPlayerDetector;
        private Detector outerPlayerDetector;
        private Detector visualConePlayerDetector;

        public static EventHandler<EnemyPlayerTrackerArgs> PlayerTrackerUpdated;

        public EnemyPlayerTracker(Detector innerDetector, Detector outerDetector, Detector visualConeDetector)
        {
            innerPlayerDetector = innerDetector;
            outerPlayerDetector = outerDetector;
            visualConePlayerDetector = visualConeDetector;

            innerPlayerDetector.DetectTag("Player");
            outerPlayerDetector.DetectTag("Player");
            visualConePlayerDetector.DetectTag("Player");
            Detector.TagEntered += PlayerEnteredDetector;
            Detector.TagExited += PlayerExitedDetector;
            Detector.TagStaying += PlayerStayingInDetector;
            innerPlayerDetector.gameObject.SetActive(false);
            outerPlayerDetector.gameObject.SetActive(false);
            visualConePlayerDetector.gameObject.SetActive(true);
        }
        
        private void CheckConditions(bool isPlayerInner, bool isPlayerOuter)
        {
            if(isPlayerInner) Detector.TagStaying -= PlayerStayingInDetector;
            
            if(isPlayerInner != this.isPlayerInner || isPlayerOuter != this.isPlayerOuter)
            {
                IsPlayerInInnerZone = isPlayerInner;
                IsPlayerInOuterZone = isPlayerOuter && !isPlayerInner;
                IsPlayerOutsideDetectors = !isPlayerInner && !isPlayerOuter;

                PlayerTrackerUpdated?.Invoke(this, new(IsPlayerInInnerZone, IsPlayerInOuterZone, IsPlayerOutsideDetectors));
            }

            this.isPlayerInner = isPlayerInner;
            this.isPlayerOuter = isPlayerOuter;
        }

        public void PlayerEnteredDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if(triggeredDetector == innerPlayerDetector)
            {
                CheckConditions(isPlayerInner:true, isPlayerOuter);
            }
            else if (triggeredDetector == outerPlayerDetector)
            {
                CheckConditions(isPlayerInner, isPlayerOuter: true);
            }
            else if(triggeredDetector == visualConePlayerDetector)
            {
                visualConePlayerDetector.gameObject.SetActive(false);
                innerPlayerDetector.gameObject.SetActive(true);
                outerPlayerDetector.gameObject.SetActive(true);

                PlayerTrackerUpdated?.Invoke(this, new(IsPlayerInInnerZone, IsPlayerInOuterZone, IsPlayerOutsideDetectors, playerEnteredVisualCone: true));
            }
        }

        public void PlayerStayingInDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if (triggeredDetector == innerPlayerDetector) CheckConditions(isPlayerInner: true, isPlayerOuter);
            else if (triggeredDetector == outerPlayerDetector) CheckConditions(isPlayerInner, isPlayerOuter: true);
        }

        public void PlayerExitedDetector(object sender, EventArgs e)
        {
            var triggeredDetector = (Detector)sender;
            if (triggeredDetector == innerPlayerDetector) CheckConditions(isPlayerInner: false, isPlayerOuter);
            else if (triggeredDetector == outerPlayerDetector) CheckConditions(isPlayerInner, isPlayerOuter: false);
        }
        
        public void Dispose()
        {
            innerPlayerDetector = null;
            outerPlayerDetector = null;
            Detector.TagEntered -= PlayerEnteredDetector;
            Detector.TagExited -= PlayerExitedDetector;
            Detector.TagStaying -= PlayerStayingInDetector;
        }
    }

    public class EnemyPlayerTrackerArgs : EventArgs
    {
        public bool IsPlayerInInnerZone { get; private set; }
        public bool IsPlayerInOuterZone { get; private set; }
        public bool IsPlayerOutsideDetectors { get; private set; }
        public bool PlayerEnteredVisualCone { get; private set; }

        public EnemyPlayerTrackerArgs(bool isPlayerInInnerZone, bool isPlayerInOuterZone, bool isPlayerOutsideDetectors, bool playerEnteredVisualCone = false)
        {
            IsPlayerInInnerZone = isPlayerInInnerZone;
            IsPlayerInOuterZone = isPlayerInOuterZone;
            IsPlayerOutsideDetectors = isPlayerOutsideDetectors;
            PlayerEnteredVisualCone = playerEnteredVisualCone;
        }
    }
}