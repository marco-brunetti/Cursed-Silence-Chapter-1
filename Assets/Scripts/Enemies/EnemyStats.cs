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
            
        
        
        public EnemyState RecievedAttack(EnemyState currentState)
        {
            
            EnemyState newState = EnemyState.Attack;

            return newState;
        }
    }
}
