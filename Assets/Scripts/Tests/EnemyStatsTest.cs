using UnityEngine;

namespace Enemies
{
    public class EnemyStatsTest : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        private EnemyStats stats;

        void Awake()
        {
            stats = new(data);
            stats.UpdatedStats += OnStatsChanged;
        }

        // Update is called once per frame
        void Update()
        {
            stats.RecievedAttack(new(EnemyState.Attack, true, 1, 1));
        }

        private void OnStatsChanged(object sender, UpdatedStatsEventArgs e)
        {
            switch (e.Type)
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
}