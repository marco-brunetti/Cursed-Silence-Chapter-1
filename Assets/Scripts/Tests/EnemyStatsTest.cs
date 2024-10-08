using UnityEngine;

namespace Enemies
{
    public class EnemyStatsTest : MonoBehaviour
    {
        [SerializeField] private EnemyData data;

        [Header("Parameters")]
        [SerializeField] private EnemyState enemyState;
        [SerializeField] private bool isVulnerable;
        [SerializeField] private int damage;
        [SerializeField] private int poiseDecrement;
        [SerializeField] private int blockProbability;


        [Header("Results")]
        public int CurrentHealth;
        public int CurrentPoise;
        private int Damage;
        private int PoiseDecrement;
        private int RandomBlockResult;


        //Block
        [SerializeField] private int blockSuccessfulEvents;
        [SerializeField] private int blockFailedEvents;
        [SerializeField] private float blockedPercent;

        private EnemyStats stats;

        private void Awake()
        {
            stats = new(data);
            blockProbability = data.BlockProbability;
            //stats.StatsChanged += OnStatsChanged;
        }

        private void Update()
        {
            stats.ReceivedAttack(new(enemyState, canDie: true, isVulnerable, damage, poiseDecrement));
        }

        private void OnStatsChanged(object sender, UpdatedStatsEventArgs e)
        {
            switch (e.Type)
            {
                case "block":

                    if (e.RandomBlockResult < blockProbability) blockSuccessfulEvents++;
                    else blockFailedEvents++;

                    if(blockFailedEvents > 0 && blockSuccessfulEvents > 0) blockedPercent = (float)blockSuccessfulEvents / (blockSuccessfulEvents + blockFailedEvents);
                    break;
                case "damage":
                    CurrentHealth = e.CurrentHealth;
                    Damage = e.HealthDecrement;
                    break;
                case "poise_decrement":
                    CurrentPoise = e.CurrentPoise; 
                    PoiseDecrement = e.PoiseDecrement;
                    break;
            }
        }
    }
}