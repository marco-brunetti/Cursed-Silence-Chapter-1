using System;
using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemySpider : Enemy
    {
        private bool alertedOthers;
        private static EventHandler spiderAlerted;
        
        protected override void Awake()
        {
            base.Awake();
            spiderAlerted += OnSpiderAlerted;
        }
        
        protected override void OnPlayerTrackerUpdated(object sender, EnemyPlayerTrackerArgs e)
        {
            base.OnPlayerTrackerUpdated(sender, e);
            if (!alertedOthers && currentState != EnemyState.Dead && !isReacting && (EnemyPlayerTracker)sender == playerTracker)
            {
                if (e.PlayerEnteredVisualCone) spiderAlerted?.Invoke(this, null);
                alertedOthers = true;
            }
        }

        private void OnSpiderAlerted(object sender, EventArgs e)
        {
            if ((EnemySpider)sender != this)
            {
                spiderAlerted -= OnSpiderAlerted;
                alertedOthers = true;
                StartCoroutine(RandomAlertStart(random.Next(3, 10) / 10f));
            }
        }

        private IEnumerator RandomAlertStart(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerTracker.Start(visualConeOnly: false);
        }
    }
}
