using System;
using UnityEngine;

public static class GameEvents
{
    public static EventHandler<DamageEnemyEventArgs> DamageEnemy;

    public static void Damage(GameObject enemy, int damage, int poiseDecrement)
    {
        DamageEnemy?.Invoke(null, new(enemy, damage, poiseDecrement));
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