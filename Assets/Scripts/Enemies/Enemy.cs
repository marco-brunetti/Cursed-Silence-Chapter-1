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

        private bool canDie;
        private bool isVulnerable;
        private bool isReacting;
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
        public void IsVulnerable(bool enable) => isVulnerable = enable;

        public void ChangeNextAttack(bool enable) => changeNextAttack = enable;

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
            stats = new EnemyStats(Data);
        }
        
        private void AnimationInit()
        {
            animation.Init(enemy: this, enemyData: Data);
            ChangeState(EnemyState.Idle);
        }
        
        private void StartPlayerTracking()
        {
            EnemyPlayerTracker.PlayerTrackerUpdated += OnPlayerTrackerUpdated;
            playerTracker.Start(visualConeOnly: true);
        }
        
        private void StopPlayerTracking()
        {
            EnemyPlayerTracker.PlayerTrackerUpdated -= OnPlayerTrackerUpdated;
            playerTracker.Stop();
        }
        
        public void DealDamage(int damageAmount, int poiseDecrement)
        {
            if (currentState == EnemyState.Idle) playerTracker.Start(visualConeOnly: false);

            var isValidState = currentState != EnemyState.Dead && currentState != EnemyState.Escape;

            if (isValidState && !isReacting)
            {
                ChangeState(stats.ReceivedAttack(new EnemyAttackedStateData(currentState, canDie, isVulnerable, damageAmount, poiseDecrement)));
            }
        }
        
        private void ChangeState(EnemyState newState)
        {
            isReacting = false;
            currentState = newState;
            
            switch (currentState)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Dead:
                    Die();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Walk:
                    Move();
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

        protected virtual void Idle()
        {
            animation.SetState(Data.IdleKey);
        }
        
        protected virtual void Die()
        {
            StopPlayerTracking();
            collider.enabled = false;
            animation.SetState(Data.DeathKey);
        }

        protected virtual void Attack()
        {
            attack ??= StartCoroutine(AttackingPlayer());
        }
        
        protected virtual void Move()
        {
            animation.SetState(Data.MoveKey, lookTarget:player, moveTarget:player);
        }
        
        protected virtual void React()
        {
            throw new System.NotImplementedException();
        }
        
        protected virtual void Block()
        {
            throw new System.NotImplementedException();
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
        
        protected virtual void OnPlayerTrackerUpdated(object sender, EnemyPlayerTrackerArgs e)
        {
            if (currentState != EnemyState.Dead && !isReacting && (EnemyPlayerTracker)sender == playerTracker)
            {
                if (e.PlayerEnteredVisualCone)
                {
                    ChangeState(EnemyState.Walk);
                    return;
                }

                if (e.IsPlayerInInnerZone && currentState != EnemyState.Attack)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (e.IsPlayerInOuterZone && currentState != EnemyState.Walk)
                {
                    ChangeState(EnemyState.Walk);
                }
                else if (e.IsPlayerOutsideDetectors && currentState != EnemyState.Idle)
                {
                    playerTracker.Start(visualConeOnly: true);
                    ChangeState(EnemyState.Idle);
                }


                if (!e.IsPlayerInInnerZone && !e.IsPlayerInOuterZone && !e.PlayerEnteredVisualCone && !e.IsPlayerOutsideDetectors)
                {
                    ChangeState(EnemyState.Idle); //Set in case there is a problem with the tracker
                }
            }
        }
        
        public void ReactStop()
        {
            isReacting = false;
            OnPlayerTrackerUpdated(playerTracker, new(playerTracker.InAttackZone, playerTracker.InAwareZone, playerTracker.OutsideZone));
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