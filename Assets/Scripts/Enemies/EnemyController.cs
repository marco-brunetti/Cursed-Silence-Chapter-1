using System;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.Serialization;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Detector attackZone;
        [SerializeField] private Detector awareZone;
        [SerializeField] private CustomShapeDetector visualCone;
        [SerializeField] private new Collider collider;
        [SerializeField] private new EnemyAnimation animation;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Renderer[] particleRenderers;

        private bool canDie;
        private bool isVulnerable;
        private bool isReacting;
        private Transform player;
        private List<Renderer> invisibleRenderers = new();
        private EnemyState currentState;
        private System.Random random;
        private EnemyPlayerTracker playerTracker;
        private EnemyStats stats;

        [field: SerializeField] public EnemyData EnemyData { get; private set; }
        public void IsVulnerable(bool enable) => isVulnerable = enable;

        private void Awake()
        {
            collider.enabled = true;
            random = new System.Random(Guid.NewGuid().GetHashCode());
            //playerTracker = new EnemyPlayerTracker(this, attackZone, awareZone, visualCone);
        }

        private void Start()
        {
            player = PlayerController.Instance.Player.transform;
            AnimationInit();
            StartPlayerTracking();
            stats = new EnemyStats(EnemyData);
        }

        // ReSharper disable Unity.PerformanceAnalysis
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
            // playerTracker.ActivateDetectors();

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
                case EnemyState.SpecialAttack:
                    SpecialAttack();
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
                default:
                    ChangeState(EnemyState.Idle);
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

        private void SpecialAttack()
        {
            playerTracker.Stop();
            animation.SpecialAttack();
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

        private void AnimationInit()
        {
            animation.Init(controller: this, enemyData: EnemyData, player);
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

        private void OnDisable()
        {
            StopPlayerTracking();
        }
    }
}