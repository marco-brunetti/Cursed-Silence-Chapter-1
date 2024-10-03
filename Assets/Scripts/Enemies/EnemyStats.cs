using System;

namespace Enemies
{
    public class EnemyStats
    {
        private int currentHealth;
        private int currentPoise;
        private EnemyData data;
        private Random random;

        public EnemyStats(EnemyData data)
        {
            this.data = data;
            currentHealth = data.Health;
            currentPoise = data.Poise;
            random = new System.Random(Guid.NewGuid().GetHashCode());
        }
            
        
        
        public EnemyState RecievedAttackState(EnemyState currentState)
        {
            
            EnemyState newState = EnemyState.Attack;

            return newState;
        }
    }
}
