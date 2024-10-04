using System;
using System.Diagnostics;

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
            random = new Random(Guid.NewGuid().GetHashCode());
        }
        
        public EnemyState RecievedAttack(AttackedStateData stateData)
        {
            EnemyState newState;
            switch(stateData.CurrentState)
            {
                case EnemyState.Attack:
                    if(stateData.IsVulnerable) return BlockState(stateData); //TakeDamageState(stateData); //Add big react
                    else return DecrementPoiseState(stateData);
                case EnemyState.Walk:
                case EnemyState.Idle:
                    return BlockState(stateData);
                default:
                    return stateData.CurrentState;
            }
        }

        private EnemyState BlockState(AttackedStateData stateData)
        {
            int i = random.Next(0, 100);
            if (i < data.BlockProbability) 
            {
                UnityEngine.Debug.Log($"Enemy blocked. Block probability: {data.BlockProbability}%. Random result: {i}");
                return EnemyState.Block;
            }
            
            UnityEngine.Debug.Log($"Enemy didn't block. Block probability: {data.BlockProbability}%. Random result: {i}");
            return TakeDamageState(stateData);
        }

        private EnemyState DecrementPoiseState(AttackedStateData stateData)
        {
            currentPoise -= stateData.PoiseDecrement;
            if(currentPoise <= 0)
            {
                currentPoise = data.Poise;
                UnityEngine.Debug.Log($"Enemy poise reduced. Amount: {stateData.PoiseDecrement}. Current poise: {currentPoise}");
                return TakeDamageState(stateData);
            }

            return stateData.CurrentState;
        }

        private EnemyState TakeDamageState(AttackedStateData stateData)
        {
            currentHealth -= stateData.Damage;
            UnityEngine.Debug.Log($"Enemy took damage. Amount: {stateData.Damage}. Current health: {currentHealth}");
            if (currentHealth <= 0) return EnemyState.Dead;
            return EnemyState.React;
        }
    }

    public class AttackedStateData
    {
        public readonly EnemyState CurrentState;
        public readonly bool IsVulnerable;
        public readonly int Damage;
        public readonly int PoiseDecrement;

        public AttackedStateData(EnemyState currentState, bool isVulnerable, int damage, int poiseDecrement)
        {
            CurrentState = currentState;
            IsVulnerable = isVulnerable;
            Damage = damage;
            PoiseDecrement = poiseDecrement;
        }
    }
}
