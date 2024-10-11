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

        [field: SerializeField] public EnemyData Data { get; private set; }

        public virtual void Awake()
        {
            hasHeavyAttack = Data.HeavyAttackAnim != null;
            hasSpecialAttack = Data.SpecialAttackAnim != null;
            collider.enabled = true;
            random = new System.Random(Guid.NewGuid().GetHashCode());
            playerTracker = new EnemyPlayerTracker(this, attackZone, awareZone, visualCone);
        }
        
        public virtual void Start()
        {
            gameController = GameControllerV2.Instance;
            GameEvents.DamageEnemy += OnDamageEnemy;

            player = gameController.PlayerTransform;
            AnimationInit();
            StartPlayerTracking();
            stats = new EnemyStats(Data);
        }

        private void AnimationInit()
        {
            animation.Init(enemy: this, enemyData: Data, GetComponent<NavMeshAgent>());
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
                        animation.StartReact(args.FloatValue);
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
                        animation.Navigation.SetAgentSpeed(args.FloatValue);
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
                EnemyState.Idle => Data.IdleAnim != null,
                EnemyState.Walk => Data.MoveAnim != null,
                EnemyState.Attack => Data.AttackAnim != null,
                EnemyState.SpecialAttack => Data.SpecialAttackAnim != null,
                EnemyState.React => Data.ReactAnim != null,
                EnemyState.Block => Data.BlockAnim != null,
                EnemyState.Dead => Data.DeathAnim != null,
                _ => false
                //EnemyState.Wander => Data.WanderAnim,
                //EnemyState.Escape => Data.EscapeAnim
            };
        }

        protected virtual void Idle()
        {
            animation.SetState(Data.IdleAnim.name);
        }
        
        protected virtual void Die()
        {
            StopPlayerTracking();
            collider.enabled = false;
            StartCoroutine(EnemyDisappear());
            animation.SetState(Data.DeathAnim.name);
        }

        protected virtual void Attack()
        {
            attack ??= StartCoroutine(AttackingPlayer());
        }
        
        protected virtual void Move()
        {
            animation.SetState(Data.MoveAnim.name, moveTarget:player);
        }
        
        protected virtual void React()
        {
            isReacting = true;
            StopPlayerTracking();
            animation.SetState(Data.ReactAnim.name);
        }
        
        protected virtual void Block()
        {
            isReacting = true;
            animation.SetState(Data.BlockAnim.name);
        }

        private IEnumerator AttackingPlayer()
        {
            var attackKeysList = new List<string> { Data.AttackAnim.name };
            if(hasHeavyAttack) attackKeysList.Add(Data.HeavyAttackAnim.name);
            if(hasSpecialAttack) attackKeysList.Add(Data.SpecialAttackAnim.name);
        
            if (!attackKeysList.Contains(animation.CurrentKey))
            {
                animation.SetState(Data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
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
            if (animation.CurrentKey == Data.AttackAnim.name)
            {
                var p = 0;
                if(hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);
            
                if (hasHeavyAttack && hasSpecialAttack)
                {
                    if(p < Data.SpecialAttackProbability) animation.SetState(Data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                    else if (p < Data.HeavyAttackProbability + Data.SpecialAttackProbability) animation.SetState(Data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasHeavyAttack)
                {
                    if (p < Data.HeavyAttackProbability) animation.SetState(Data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasSpecialAttack)
                {
                    if(p < Data.SpecialAttackProbability) animation.SetState(Data.SpecialAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
            }
            else
            {
                animation.SetState(Data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
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
                        var disappearSpeed = Data.OnDieDisappearSpeed;//r is ParticleSystemRenderer ? 0.1f : 0.01f;
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