using Enemies;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private new Collider collider;
    [SerializeField] private Detector attackZone;
    [SerializeField] private Detector awareZone;
    [SerializeField] private CustomShapeDetector visualCone;
    [SerializeField] private new EnemyAnimation animation;

    private Transform player;
    private EnemyState currentState;
    private System.Random random;
    private EnemyStats stats;
    private EnemyPlayerTracker playerTracker;

    [field: SerializeField] public EnemyData Data { get; private set; }

    public virtual void Awake()
    {
        collider.enabled = true;
        random = new System.Random(Guid.NewGuid().GetHashCode());
        playerTracker = new EnemyPlayerTracker(this, attackZone, awareZone, visualCone);
    }

    public virtual void Move()
    {

    }
    public virtual void Attack()
    {

    }
    public virtual void Idle()
    {

    }
    public virtual void SpecialAttack()
    {

    }
    public virtual void React()
    {

    }
    public virtual void Block()
    {

    }
    public virtual void Die()
    {

    }
}