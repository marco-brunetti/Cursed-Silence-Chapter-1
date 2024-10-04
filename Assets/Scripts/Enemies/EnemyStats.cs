using System;

namespace Enemies
{
    public class EnemyStats
    {
        private int normalizedBlockProbability;
        private int normalizedTakeDamageProbability;

        private int currentHealth;
        private int currentPoise;
        private EnemyData data;
        private Random random;

        public EnemyStats(EnemyData data)
        {
            this.data = data;
            currentHealth = data.Health;
            currentPoise = data.Poise;
            random = new Random(Guid.NewGuid().GetHashCode());

            NormalizeReactionProbability(data);

        }

        private void NormalizeReactionProbability(EnemyData data)
        {
            var normalizeFactor = 1 / (data.BlockProbability + data.TakeDamageProbability);
            normalizedBlockProbability = (int)(data.BlockProbability * normalizeFactor * 100);
            normalizedTakeDamageProbability = (int)(data.TakeDamageProbability * normalizeFactor);
        }
        
        public EnemyState RecievedAttack(EnemyState currentState, bool isVulnerable, int damage, int poiseDecrement)
        {
            EnemyState newState = currentState;

            if (currentState == EnemyState.Attack)
            {
                if(isVulnerable)
                {
                    currentHealth -= damage;
                    newState = EnemyState.React; //Add big react
                }
                else
                {
                    currentPoise -= poiseDecrement;

                    if(currentPoise <= 0)
                    {
                        currentHealth -= damage;
                        newState = EnemyState.React;

                        currentPoise = data.Poise;
                    }
                }
            }
            else if (currentState == EnemyState.Walk || currentState == EnemyState.Idle)
            {
                var i = random.Next(0, 100);
                
                if (i < normalizedBlockProbability) 
                {
                    newState = EnemyState.Block;
                }
                else
                {
                    currentHealth -= damage;
                    newState = EnemyState.React;
                }
            }

            if (currentHealth <= 0) newState = EnemyState.Dead;

            return newState;
        }

        public void ResetPoise()
        {
            currentPoise = data.Poise;
        }
    }
}
