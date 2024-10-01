using UnityEngine;
using System.Collections.Generic;
using Player;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    { 
        [SerializeField] private EnemyData data;
        [SerializeField] private new Collider collider;
        [SerializeField] private new EnemyAnimation animation;

        [SerializeField] private Detector innerPlayerDetector;
        [SerializeField] private Detector outerPlayerDetector;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Renderer[] particleRenderers;

        private int currentHealth;
        private bool isEngaging;
        private bool canRecieveDamage = true;
        public EnemyState CurrentState { get; private set; }
        private Transform player;
        private List<Renderer> invisibleRenderers = new();

        public void CanRecieveDamage(bool enable)
        {
            if (true) //Check condition later
            {
                canRecieveDamage = enable;
            }
        }

        private void Awake()
        {
            innerPlayerDetector.DetectTag("Player");
            outerPlayerDetector.DetectTag("Player");
            Detector.ColliderEntered += PlayerDetected;
            Detector.ColliderExited += PlayerExitedDetector;
        }

        private void Start()
        {
            player = PlayerController.Instance.Player.transform;
            animation.Init(controller: this, enemyData: data, player);
            ChangeState(EnemyState.Idle);
        }

        private void OnEnable()
        {
            currentHealth = data.Health;
        }

        private void OnDisable()
        {
            Detector.ColliderEntered -= PlayerDetected;
            Detector.ColliderExited -= PlayerExitedDetector;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void DealDamage(int damageAmount)
        {
            if (CurrentState != EnemyState.Dead && canRecieveDamage)
            {
                currentHealth -= damageAmount;
                Debug.Log($"Dealing damage {damageAmount} remaining enemyhealth: {currentHealth}");

                if (currentHealth <= 0) ChangeState(EnemyState.Dead);
                else animation.React();
            }
        }

        private void PlayerDetected(object detector, Collider other)
        {
            if (CurrentState != EnemyState.Dead)
            {
                var triggeredDetector = detector as Detector;
                if (triggeredDetector == innerPlayerDetector)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (triggeredDetector == outerPlayerDetector)
                {
                    ChangeState(EnemyState.Walk);
                }
            }
        }

        private void PlayerExitedDetector(object detector, Collider other)
        {
            if (CurrentState != EnemyState.Dead)
            {
                if ((Detector)detector == innerPlayerDetector) ChangeState(EnemyState.Walk);
                else if ((Detector)detector == outerPlayerDetector) ChangeState(EnemyState.Idle);
            }
        }

        private void ChangeState(EnemyState newState)
        {
            CurrentState = newState;

            switch (CurrentState)
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
            }

            if (CurrentState != EnemyState.Walk) animation.WalkSpeed = 0;
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

        private void Die()
        {
            collider.enabled = false;
            innerPlayerDetector.gameObject.SetActive(false);
            outerPlayerDetector.gameObject.SetActive(false);
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
    }

    public enum EnemyState
    {
        Idle,
        Walk,
        Attack,
        Escape,
        Block,
        Stunned,
        Dead
    }
}