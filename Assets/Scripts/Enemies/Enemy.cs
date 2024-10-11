using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyData data;
        [SerializeField] protected new Collider collider;
        [SerializeField] protected Detector attackZone;
        [SerializeField] protected Detector awareZone;
        [SerializeField] protected CustomShapeDetector visualCone;
        [SerializeField] protected new EnemyAnimation animation;
        [SerializeField] private List<Renderer> renderers;

        private bool canDie = true;
        private bool isVulnerable = true;
        private bool isReacting;
        private bool hasHeavyAttack;
        private bool hasSpecialAttack;
        private bool changeNextAttack;
        
        private Transform player;
        private EnemyState currentState;
        private System.Random random;
        private EnemyStats stats;
        private Coroutine attack;
        private EnemyPlayerTracker playerTracker;
        private NavMeshAgent agent;
        private GameControllerV2 gameController;
        
        public virtual void Awake()
        {
            hasHeavyAttack = data.HeavyAttackAnim != null;
            hasSpecialAttack = data.SpecialAttackAnim != null;
            collider.enabled = true;
            random = new System.Random(Guid.NewGuid().GetHashCode());
            playerTracker = new EnemyPlayerTracker(this, attackZone, awareZone, visualCone, data.DetectionMask);
        }
        
        public virtual void Start()
        {
            gameController = GameControllerV2.Instance;
            GameEvents.DamageEnemy += OnDamageEnemy;

            player = gameController.PlayerTransform;
            AnimationInit();
            StartPlayerTracking();
            stats = new EnemyStats(data);
        }

        private void AnimationInit()
        {
            animation.Init(enemy: this, enemyData: data, GetComponent<NavMeshAgent>());
            EnemyAnimation.AnimationEvent += OnAnimationEvent;
            ChangeState(EnemyState.Idle);
        }
        
        private void StartPlayerTracking(bool visualConeOnly = true)
        {
            EnemyPlayerTracker.PlayerTrackerUpdated += OnPlayerTrackerUpdated;
            playerTracker.Start(visualConeOnly);
        }
        
        private void StopPlayerTracking()
        {
            EnemyPlayerTracker.PlayerTrackerUpdated -= OnPlayerTrackerUpdated;
            playerTracker.Stop();
        }
        
        public void OnDamageEnemy(object sender, DamageEnemyEventArgs args)
        {
            if(args.Enemy == this.gameObject)
            {
                if (currentState == EnemyState.Idle) playerTracker.Start(visualConeOnly: false);

                var isValidState = currentState != EnemyState.Dead && currentState != EnemyState.Escape;

                if (isValidState && !isReacting)
                {
                    ChangeState(stats.ReceivedAttack(new EnemyAttackedStateData(currentState, canDie, isVulnerable, args.Damage, args.PoiseDecrement)));
                }
            }
        }

        public void OnAnimationEvent(object sender, AnimationEventArgs args)
        {
            if((EnemyAnimation)sender == animation)
            {
                switch (args.EventName)
                {
                    case "set_vulnerable_true":
                        isVulnerable = true;
                        break;
                    case "set_vulnerable_false":
                        isVulnerable = false;
                        break;
                    case "change_next_attack":
                        changeNextAttack = true;
                        break;
                    case "react_start":
                        animation.StartReact(transform, args.FloatValue);
                        break;
                    case "react_stop_movement":
                        animation.StopReact(); //Stops movement only, still does not change state
                        break;
                    case "react_stop":
                        StopReact();
                        break;
                    case "block_stop":
                        StopReact();
                        break;
                    case "walk_started":
                        animation.SetAgentSpeed(args.FloatValue);
                        break;
                }
            }
        }
        
        private void ChangeState(EnemyState newState)
        {
            if(!IsValidState(newState))
            {
                Debug.LogError($"There is no anim for state {newState.ToString().ToUpper()} on {gameObject.name.ToUpper()}! Maintaining current state.");
                return;
            }

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

        private bool IsValidState(EnemyState newState)
        {
            return newState switch
            {
                EnemyState.Idle => data.IdleAnim != null,
                EnemyState.Walk => data.MoveAnim != null,
                EnemyState.Attack => data.AttackAnim != null,
                EnemyState.SpecialAttack => data.SpecialAttackAnim != null,
                EnemyState.React => data.ReactAnim != null,
                EnemyState.Block => data.BlockAnim != null,
                EnemyState.Dead => data.DeathAnim != null,
                _ => false
                //EnemyState.Wander => Data.WanderAnim,
                //EnemyState.Escape => Data.EscapeAnim
            };
        }

        protected virtual void Idle()
        {
            animation.SetState(data.IdleAnim.name);
        }
        
        protected virtual void Die()
        {
            StopPlayerTracking();
            collider.enabled = false;
            StartCoroutine(EnemyDisappear());
            animation.SetState(data.DeathAnim.name);
        }

        protected virtual void Attack()
        {
            attack ??= StartCoroutine(AttackingPlayer());
        }
        
        protected virtual void Move()
        {
            animation.SetState(data.MoveAnim.name, moveTarget:player);
        }
        
        protected virtual void React()
        {
            isReacting = true;
            StopPlayerTracking();
            animation.SetState(data.ReactAnim.name);
        }
        
        protected virtual void Block()
        {
            isReacting = true;
            animation.SetState(data.BlockAnim.name);
        }

        private IEnumerator AttackingPlayer()
        {
            var attackKeysList = new List<string> { data.AttackAnim.name };
            if(hasHeavyAttack) attackKeysList.Add(data.HeavyAttackAnim.name);
            if(hasSpecialAttack) attackKeysList.Add(data.SpecialAttackAnim.name);
        
            if (!attackKeysList.Contains(animation.CurrentKey))
            {
                animation.SetState(data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
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
            if (animation.CurrentKey == data.AttackAnim.name)
            {
                var p = 0;
                if(hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);
            
                if (hasHeavyAttack && hasSpecialAttack)
                {
                    if(p < data.SpecialAttackProbability) animation.SetState(data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                    else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasHeavyAttack)
                {
                    if (p < data.HeavyAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasSpecialAttack)
                {
                    if(p < data.SpecialAttackProbability) animation.SetState(data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
            }
            else
            {
                animation.SetState(data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
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
        
        //Set materials to fade or transparent
        private IEnumerator EnemyDisappear()
        {
            if (renderers.Count > 0)
            {
                while (renderers.Count > 0)
                {
                    foreach (var r in renderers.ToList())
                    {
                        var disappearSpeed = data.OnDieDisappearSpeed;//r is ParticleSystemRenderer ? 0.1f : 0.01f;
                        var c = r.material.color;
                        var alpha = Mathf.Clamp(c.a - disappearSpeed * Time.deltaTime, 0, 1);
                        r.material.color = new Color(c.r, c.g, c.b, alpha);
                        if (alpha <= 0) renderers.Remove(r);
                    }
                    yield return null;
                }
            
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"No renderers to disappear in {gameObject.name}");
            }
        }

        private void StopReact()
        {
            isReacting = false;
            animation.StopReact();
            StartPlayerTracking(visualConeOnly: false);
        }

        private void OnDestroy()
        {
            GameEvents.DamageEnemy -= OnDamageEnemy;
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