using Enemies;
using System;
using UnityEngine;

public class EnemyFactory
{
    public static Enemy CreateEnemy(string type)
    {
        switch (type)
        {
            case "spider":
                return new EnemySpider();
            case "emily":
                return new EnemyEmily();
            default:
                throw new ArgumentException("Invalid enemy type");
        }
    }
}