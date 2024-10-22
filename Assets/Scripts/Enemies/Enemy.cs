using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using SnowHorse.Systems;
using SnowHorse.Components;

namespace Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] protected List<Renderer> renderers;
        [SerializeField] protected EnemyData data;
        [SerializeField] protected new Collider collider;
        [SerializeField] protected new EnemyAnimation animation;
        [SerializeField] protected CustomShapeDetector visualCone;

        private bool canDie = true;
        private EnemyStats stats;
        private NavMeshAgent agent;

        protected bool isVulnerable;
        protected bool isReacting;
        protected bool hasHeavyAttack;
        protected bool hasSpecialAttack;
        protected bool changeNextAttack;
        protected Transform player;
        protected EnemyState currentState;
        protected System.Random random;
        protected Coroutine attack;
        protected EnemyPlayerTracker playerTracker;

        public static EventHandler<Enemy> EnemyAwake;
        public static EventHandler<GameObject> AddActiveEnemy;
        public static EventHandler<GameObject> RemoveActiveEnemy;

        public void SetPlayerTransform(Transform playerTransform) => player = playerTransform;
        
        protected virtual void Awake()
        {
            EnemyAwake?.Invoke(this, this);
            hasHeavyAttack = data.HeavyAttackAnim != null;
            hasSpecialAttack = data.SpecialAttackAnim != null;
            collider.enabled = true;
            random = new System.Random(Guid.NewGuid().GetHashCode());
        }
        
        protected virtual void Start()
        {
            EventsManager.DamageEnemy += OnDamageEnemy;
            playerTracker = new EnemyPlayerTracker(this, player, visualCone, data);
            AnimationInit();
            StartPlayerTracking();
            stats = new EnemyStats(data);
        }

        private void AnimationInit()
        {
            animation.Init(data, GetComponent<NavMeshAgent>());
            EnemyAnimation.AnimationClipEvent += OnAnimationEvent;
            ChangeState(data.InitialEnemyState);
        }
        
        protected void StartPlayerTracking(bool visualConeOnly = true)
        {
            EnemyPlayerTracker.PlayerTrackerUpdated += OnPlayerTrackerUpdated;
            playerTracker.Start(visualConeOnly);
        }
        
        protected void StopPlayerTracking()
        {
            EnemyPlayerTracker.PlayerTrackerUpdated -= OnPlayerTrackerUpdated;
            playerTracker.Stop();
        }

        private void OnDamageEnemy(object sender, DamageEnemyEventArgs args)
        {
            if(isVulnerable && args.Enemy == gameObject)
            {
                if (currentState == EnemyState.Idle) playerTracker.Start(visualConeOnly: false);

                var isValidState = currentState != EnemyState.Dead && currentState != EnemyState.Escape;

                if (isValidState && !isReacting)
                {
                    ChangeState(stats.ReceivedAttack(new EnemyAttackedStateData(currentState, canDie, isVulnerable, args.Damage, args.PoiseDecrement)));
                }
            }
        }

        protected virtual void OnAnimationEvent(object sender, AnimationEventArgs args)
        {
            if ((EnemyAnimation)sender != animation) return;
            
            switch (args.Event)
            {
                case "set_vulnerable":
                    isVulnerable = args.Bool;
                    break;
                case "change_next_attack":
                    changeNextAttack = true;
                    break;
                case "react_start":
                    isVulnerable = false;
                    animation.StartReact(transform, args.Float);
                    break;
                case "react_stop_movement":
                    animation.StopReact(); //Stops movement only, still does not change state
                    break;
                case "react_or_block_stop":
                    StopReact();
                    break;
                case "walk_started":
                    OnWalkStarted(args.Float);
                    break;
                case "set_look_speed":
                    animation.SetLookSpeed(args.Float);
                    break;
            }
        }
        
        protected virtual void ChangeState(EnemyState newState)
        {
            if(!IsValidState(newState))
            {
                Debug.LogError($"There is no anim for state {newState.ToString().ToUpper()} on {gameObject.name.ToUpper()}! Maintaining current state.");
                return;
            }

            isReacting = false;
            currentState = newState;

            List<EnemyState> activeStates = new() { EnemyState.Attack, EnemyState.React, EnemyState.Block, EnemyState.Walk };

            if(activeStates.Contains(currentState)) AddActiveEnemy?.Invoke(this, gameObject);
            else RemoveActiveEnemy?.Invoke(this, gameObject);
            
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
                case EnemyState.Escape:
                case EnemyState.Wander:
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
                EnemyState.React => data.ReactAnim != null,
                EnemyState.Block => data.BlockAnim != null,
                EnemyState.Dead => data.DeathAnim != null,
                _ => false
                //EnemyState.Wander => Data.WanderAnim,
                //EnemyState.Escape => Data.EscapeAnim
            };
        }

        private void Idle()
        {
            animation.SetState(data.IdleAnim.name, currentState);
        }
        
        private void Die()
        {
            animation.StopNavigation();
            StopPlayerTracking();
            collider.enabled = false;
            StartCoroutine(EnemyDisappear());
            animation.SetState(data.DeathAnim.name, currentState);
        }

        protected virtual void Attack()
        {
            var attackKeysList = new List<string> { data.AttackAnim.name };
            if (hasHeavyAttack) attackKeysList.Add(data.HeavyAttackAnim.name);
            if (hasSpecialAttack) attackKeysList.Add(data.SpecialAttackAnim.name);
            attack ??= StartCoroutine(AttackingPlayer(attackKeysList));
        }
        
        protected virtual void Move()
        {
            animation.SetState(data.MoveAnim.name, currentState, moveTarget:player, randomizePath: data.RandomizePath);
        }
        
        private void React()
        {
            isReacting = true;
            StopPlayerTracking();
            animation.SetState(data.ReactAnim.name, currentState);
        }
        
        private void Block()
        {
            isReacting = true;
            animation.SetState(data.BlockAnim.name, currentState);
        }

        protected IEnumerator AttackingPlayer(List<string> attackKeysList)
        {
            if (!attackKeysList.Contains(animation.CurrentKey))
            {
                animation.SetState(data.AttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                yield return null;
            }

            while (currentState == EnemyState.Attack)
            {
                if (changeNextAttack) SetRandomAttack();
                yield return null;
            }

            attack = null;
        }

        protected virtual void SetRandomAttack()
        {
            if(currentState == EnemyState.Attack)
            {
                if (animation.CurrentKey == data.AttackAnim.name)
                {
                    var p = 0;
                    if (hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);

                    if (hasHeavyAttack && hasSpecialAttack)
                    {
                        if (p < data.SpecialAttackProbability) animation.SetState(data.SpecialAttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                        else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability) animation.SetState(data.HeavyAttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                    }
                    else if (hasHeavyAttack)
                    {
                        if (p < data.HeavyAttackProbability) animation.SetState(data.HeavyAttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                    }
                    else if (hasSpecialAttack)
                    {
                        if (p < data.SpecialAttackProbability) animation.SetState(data.SpecialAttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                    }
                }
                else
                {
                    animation.SetState(data.AttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                }
            }
            changeNextAttack = false;
        }
        
        protected virtual void OnPlayerTrackerUpdated(object sender, EnemyPlayerTrackerArgs e)
        {
            if (currentState == EnemyState.Dead || isReacting || (EnemyPlayerTracker)sender != playerTracker) return;
            
            if (e.PlayerEnteredVisualCone && currentState != EnemyState.Walk)
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
        
        //Set materials to fade or transparent
        private IEnumerator EnemyDisappear()
        {
            if (data.OnDieDisappearSpeed > 0 && renderers.Count > 0)
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
        }

        private void StopReact()
        {
            isReacting = false;
            animation.StopReact();
            StartPlayerTracking(visualConeOnly: false);
        }

        protected virtual void OnWalkStarted(float speed)
        {
            if(currentState == EnemyState.Walk)
            {
                animation.SetAgentSpeed(speed); //Add any other state that contains walk clip
            }
        }

        private void OnDestroy()
        {
            EventsManager.DamageEnemy -= OnDamageEnemy;
        }
    }
    
    public enum EnemyState
    {
        Idle,
        Walk,
        Attack,
        React,
        Escape,
        Block,
        Wander,
        Dead
    }
}