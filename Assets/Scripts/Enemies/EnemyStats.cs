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
                    newState = TakeDamage(damage); //Add big react
                }
                else
                {
                    currentPoise -= poiseDecrement;

                    if(currentPoise <= 0)
                    {
                        newState = TakeDamage(damage);
                        currentPoise = data.Poise;
                    }
                }
            }
            else if (currentState == EnemyState.Walk || currentState == EnemyState.Idle)
            {
                if (random.Next(0, 100) < normalizedBlockProbability) newState = EnemyState.Block;
                else newState = TakeDamage(damage);
            }

            if (currentHealth <= 0) newState = EnemyState.Dead;

            return newState;
        }

        private EnemyState TakeDamage(int damage)
        {
            currentHealth -= damage;
            return EnemyState.React;
        }

        public void ResetPoise()
        {
            currentPoise = data.Poise;
        }
    }
}
