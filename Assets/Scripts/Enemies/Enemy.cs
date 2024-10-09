using Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private new Collider collider;
    [SerializeField] private Detector attackZone;
    [SerializeField] private Detector awareZone;
    [SerializeField] private CustomShapeDetector visualCone;
    [SerializeField] private new EnemyAnimationGeneric animation;

    private bool hasHeavyAttack;
    private bool hasSpecialAttack;
    private bool changeNextAttack;
    private Transform player;
    private EnemyState currentState;
    private System.Random random;
    private EnemyStats stats;
    private EnemyPlayerTracker playerTracker;
    private Coroutine attack;

    [field: SerializeField] public EnemyData Data { get; private set; }

    public virtual void Awake()
    {
        hasHeavyAttack = !string.IsNullOrEmpty(Data.HeavyAttackKey);
        hasSpecialAttack = !string.IsNullOrEmpty(Data.SpecialAttackKey);
        collider.enabled = true;
        random = new System.Random(Guid.NewGuid().GetHashCode());
        playerTracker = new EnemyPlayerTracker(this, attackZone, awareZone, visualCone);
    }

    public virtual void Move()
    {

    }
    public virtual void Attack()
    {
        attack ??= StartCoroutine(AttackingPlayer());
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
    
    private IEnumerator AttackingPlayer()
    {
        var attackKeysList = new List<string> { Data.AttackKey };
        if(hasHeavyAttack) attackKeysList.Add(Data.HeavyAttackKey);
        if(hasSpecialAttack) attackKeysList.Add(Data.SpecialAttackKey);
        
        if (!attackKeysList.Contains(animation.CurrentKey))
        {
            animation.Attack();
            yield return null;
        }

        while (attackKeysList.Contains(animation.CurrentKey))
        {
            if (changeNextAttack) SetRandomAttack();
            yield return null;
        }

        attack = null;
    }

    private void SetRandomAttack()
    {
        if (animation.CurrentKey == Data.AttackKey)
        {
            var p = 0;
            if(hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);
            
            if (hasHeavyAttack && hasSpecialAttack)
            {
                if(p < Data.SpecialAttackProbability) animation.SpecialAttack();
                else if (p < Data.HeavyAttackProbability + Data.SpecialAttackProbability) animation.HeavyAttack();
            }
            else if (hasHeavyAttack)
            {
                if (p < Data.HeavyAttackProbability) animation.HeavyAttack();
            }
            else if (hasSpecialAttack)
            {
                if(p < Data.SpecialAttackProbability) animation.SpecialAttack();
            }
        }
        else
        {
            animation.Attack();
        }
                
        changeNextAttack = false;
    }
}