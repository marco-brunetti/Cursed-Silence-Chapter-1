using System;

namespace Enemies
{
    public class EnemyStats
    {
        private int currentHealth;
        private int currentPoise;
        private EnemyData data;
        private Random random;

        public EventHandler<UpdatedStatsEventArgs> StatsChanged;

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
            switch (stateData.CurrentState)
            {
                case EnemyState.Attack:
                    if (stateData.IsVulnerable) return BlockState(stateData); //TakeDamageState(stateData); //Add big react
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
                StatsChanged?.Invoke(this, GetUpdatedStatsArgs("block", randomBlockResult: i));
                return EnemyState.Block;
            }

            StatsChanged?.Invoke(this, GetUpdatedStatsArgs("block", randomBlockResult: i));
            return TakeDamageState(stateData);
        }

        private EnemyState DecrementPoiseState(AttackedStateData stateData)
        {
            currentPoise -= stateData.PoiseDecrement;
            if (currentPoise <= 0)
            {
                currentPoise = data.Poise;
                StatsChanged?.Invoke(this, GetUpdatedStatsArgs("poise_decrement", poiseDecrement: stateData.PoiseDecrement));
                return TakeDamageState(stateData);
            }

            return stateData.CurrentState;
        }

        private EnemyState TakeDamageState(AttackedStateData stateData)
        {
            currentHealth -= stateData.Damage;
            StatsChanged?.Invoke(this, GetUpdatedStatsArgs("damage", damage: stateData.Damage));
            if (currentHealth <= 0) return EnemyState.Dead;
            return EnemyState.React;
        }

        
        private UpdatedStatsEventArgs GetUpdatedStatsArgs(string type, int damage = 0, int poiseDecrement = 0, int randomBlockResult = 0)
        {
            switch (type)
            {
                case "block":
                    return new UpdatedStatsEventArgs()
                    {
                        Type = type,
                        BlockProbability = data.BlockProbability,
                        RandomBlockResult = randomBlockResult
                    };
                case "damage":
                    return new UpdatedStatsEventArgs()
                    {
                        Type = type,
                        CurrentHealth = currentHealth,
                        HealthDecrement = damage
                    };
                case "poise_decrement":
                    return new UpdatedStatsEventArgs()
                    {
                        Type = type,
                        CurrentPoise = currentPoise,
                        PoiseDecrement = poiseDecrement
                    };
                default:
                    return null;
            }
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

public class UpdatedStatsEventArgs : EventArgs
{
    public string Type;
    public int CurrentHealth;
    public int CurrentPoise;
    public int HealthDecrement;
    public int PoiseDecrement;
    public int BlockProbability;
    public int RandomBlockResult;
}