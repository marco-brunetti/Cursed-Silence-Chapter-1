using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Enemies
{
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
        
        public virtual void Start()
        {
            player = PlayerController.Instance.Player.transform;
            AnimationInit();
            StartPlayerTracking();
            stats = new EnemyStats(EnemyData);
        }
        
        private void AnimationInit()
        {
            animation.Init(controller: this, enemyData: EnemyData, player);
            ChangeState(EnemyState.Idle);
        }
        
        private void ChangeState(EnemyState newState)
        {
            isReacting = false;
            currentState = newState;
            
            switch (currentState)
            {
                case EnemyState.Idle:
                    animation.SetState(Data.IdleKey);
                    break;
                case EnemyState.Dead:
                    animation.SetState(Data.DeathKey);
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Walk:
                    animation.SetState(Data.WalkKey, lookTarget:player, moveTarget:player);
                    break;
                case EnemyState.React:
                    React();
                    break;
                case EnemyState.Block:
                    Block();
                    break;
                default:
                    ChangeState(EnemyState.Idle);
                    break;
            }
        }

        public virtual void Attack()
        {
            attack ??= StartCoroutine(AttackingPlayer());
        }
    
        private IEnumerator AttackingPlayer()
        {
            var attackKeysList = new List<string> { Data.AttackKey };
            if(hasHeavyAttack) attackKeysList.Add(Data.HeavyAttackKey);
            if(hasSpecialAttack) attackKeysList.Add(Data.SpecialAttackKey);
        
            if (!attackKeysList.Contains(animation.CurrentKey))
            {
                animation.SetState(Data.AttackKey, lookTarget: player);
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
                    if(p < Data.SpecialAttackProbability) animation.SetState(Data.SpecialAttackKey, lookTarget: player);
                    else if (p < Data.HeavyAttackProbability + Data.SpecialAttackProbability) animation.SetState(Data.HeavyAttackKey, lookTarget: player);
                }
                else if (hasHeavyAttack)
                {
                    if (p < Data.HeavyAttackProbability) animation.SetState(Data.HeavyAttackKey, lookTarget: player);
                }
                else if (hasSpecialAttack)
                {
                    if(p < Data.SpecialAttackProbability) animation.SetState(Data.SpecialAttackKey, lookTarget: player);
                }
            }
            else
            {
                animation.SetState(Data.AttackKey, lookTarget: player);
            }
                
            changeNextAttack = false;
        }
    }
    
    public enum EnemyState
    {
        Idle,
        Walk,
        Attack,
        SpecialAttack,
        React,
        Escape,
        Block,
        Wander,
        Dead
    }
}