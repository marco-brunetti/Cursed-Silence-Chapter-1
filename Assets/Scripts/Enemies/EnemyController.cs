using System;
using UnityEngine;
using System.Collections.Generic;
using Player;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    { 
        [SerializeField] private EnemyData data;
        [SerializeField] private Detector innerPlayerDetector;
        [SerializeField] private Detector outerPlayerDetector;
        [SerializeField] private new Collider collider;
        [SerializeField] private new EnemyAnimation animation;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Renderer[] particleRenderers;

        private int currentHealth;
        private bool isEngaging;
        private bool canRecieveDamage;
        private bool isReacting;
        private Transform player;
        private List<Renderer> invisibleRenderers = new();
        private EnemyState currentState;
        private System.Random random;
        private EnemyPlayerTracker playerTracker;


        public void CanRecieveDamage(bool enable) => canRecieveDamage = enable;

        private void Awake()
        {
            currentHealth = data.Health;
            collider.enabled = true;
            random = new System.Random(Guid.NewGuid().GetHashCode());
        }

        private void Start()
        {
            player = PlayerController.Instance.Player.transform;
            AnimationInit();
            StartPlayerTracking();
        }


        // ReSharper disable Unity.PerformanceAnalysis
        public void DealDamage(int damageAmount)
        {
            if (currentState != EnemyState.Dead && canRecieveDamage && !isReacting)
            {
                EnemyState nextState;
                if (currentState == EnemyState.Attack)
                {
                    //Test for blocking attack
                    var i = random.Next(0,2);
                    if (i == 0) nextState = EnemyState.React;
                    else nextState = EnemyState.Block;
                }
                else
                {
                    nextState = EnemyState.React;
                }

                if (nextState == EnemyState.React)
                {
                    currentHealth -= damageAmount;
                    Debug.Log($"Dealing damage {damageAmount} remaining enemyhealth: {currentHealth}");
                    if (currentHealth <= 0) nextState = EnemyState.Dead;
                }
                else
                {
                    Debug.Log($"Enemy blocked attack.");
                }
                
                ChangeState(nextState);
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
                    Walk();
                    break;
                case EnemyState.React:
                    React();
                    break;
                case EnemyState.Block:
                    Block();
                    break;
            }

            if (currentState != EnemyState.Walk) animation.WalkSpeed = 0;
        }

        private void Idle()
        {
            animation.Idle();
        }

        private void Walk()
        {
            animation.Walk();
        }

        private void Attack()
        {
            animation.Attack();
        }

        private void React()
        {
            isReacting = true;
            animation.React();
        }

        private void Block()
        {
            isReacting = true;
            animation.Block();
        }

        private void Die()
        {
            StopPlayerTracking();
            collider.enabled = false;
            animation.Die();
            EnemyDisappear();
        }

        private void EnemyDisappear()
        {
            foreach (var r in renderers)
            {
                var c = r.material.color;

                if (c.a > 0)
                {
                    float disappearSpeed;
                    if (r is ParticleSystemRenderer) disappearSpeed = 0.1f;
                    else disappearSpeed = 0.01f;

                    var alpha = c.a - disappearSpeed * Time.deltaTime;
                    alpha = Mathf.Clamp(alpha, 0, 1);
                    r.material.color = new Color(c.r, c.g, c.b, alpha);
                }

                if (c.a <= 0 && !invisibleRenderers.Contains(r)) invisibleRenderers.Add(r);
            }

            if (invisibleRenderers.Count == renderers.Length) Destroy(gameObject);
        }

        private void OnPlayerTrackerUpdated(object sender, EnemyPlayerTrackerArgs e)
        {
            if (!isReacting && (EnemyPlayerTracker)sender == playerTracker)
            {
                if (e.IsPlayerInInnerZone) ChangeState(EnemyState.Attack);
                else if (e.IsPlayerInOuterZone) ChangeState(EnemyState.Walk);
                else if (e.IsPlayerOutsideDetectors) ChangeState(EnemyState.Idle);
            }
        }

        public void ReactStop()
        {
            isReacting = false;
            OnPlayerTrackerUpdated(playerTracker, new(playerTracker.IsPlayerInInnerZone, playerTracker.IsPlayerInOuterZone, playerTracker.IsPlayerOutsideDetectors));
        }

        private void AnimationInit()
        {
            animation.Init(controller: this, enemyData: data, player);
            ChangeState(EnemyState.Idle);
        }

        private void StartPlayerTracking()
        {
            playerTracker = new EnemyPlayerTracker(innerPlayerDetector, outerPlayerDetector);
            EnemyPlayerTracker.PlayerTrackerUpdated += OnPlayerTrackerUpdated;
        }

        private void StopPlayerTracking()
        {
            EnemyPlayerTracker.PlayerTrackerUpdated -= OnPlayerTrackerUpdated;
            playerTracker.Dispose();
        }

        private void OnDisable()
        {
            StopPlayerTracking();
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
        Stunned,
        Dead
    }
}