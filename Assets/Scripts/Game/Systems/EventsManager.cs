using System;
using UnityEngine;

namespace Game.General
{
    public static class EventsManager
    {
        public static EventHandler<DamageEnemyEventArgs> DamageEnemy;

        public static void Damage(GameObject enemy, int damage, int poiseDecrement)
        {
            DamageEnemy?.Invoke(null, new DamageEnemyEventArgs(enemy, damage, poiseDecrement));
        }
    }

    public class DamageEnemyEventArgs : EventArgs
    {
        public readonly GameObject Enemy;
        public readonly int Damage;
        public readonly int PoiseDecrement;

        public DamageEnemyEventArgs(GameObject enemy, int damage, in int poiseDecrement)
        {
            Enemy = enemy;
            Damage = damage;
            PoiseDecrement = poiseDecrement;
        }
    }
}