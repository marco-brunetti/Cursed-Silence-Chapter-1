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
        private bool isBlocking;
        private Transform player;
        private List<Renderer> invisibleRenderers = new();
        private EnemyState currentState;
        private System.Random random;


        public void CanRecieveDamage(bool enable) => canRecieveDamage = enable;

        private void Awake()
        {
            innerPlayerDetector.DetectTag("Player");
            outerPlayerDetector.DetectTag("Player");
            Detector.ColliderEntered += PlayerEnteredDetector;
            Detector.ColliderExited += PlayerExitedDetector;
            Detector.ColliderStaying += PlayerStayingInDetector;
            currentHealth = data.Health;
            
            collider.enabled = true;
            innerPlayerDetector.gameObject.SetActive(true);
            outerPlayerDetector.gameObject.SetActive(true);
        }

        private void Start()
        {
            random = new System.Random(Guid.NewGuid().GetHashCode());
            player = PlayerController.Instance.Player.transform;
            animation.Init(controller: this, enemyData: data, player);
            ChangeState(EnemyState.Idle);
        }

        private void OnDisable()
        {
            Detector.ColliderEntered -= PlayerEnteredDetector;
            Detector.ColliderExited -= PlayerExitedDetector;
            Detector.ColliderStaying -= PlayerStayingInDetector;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void DealDamage(int damageAmount)
        {
            if (currentState != EnemyState.Dead && canRecieveDamage && !isReacting && !isBlocking)
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

        public void ReactStop()
        {
            isReacting = false;
            var distance = Vector3.Distance(player.position, transform.position);
            if (Mathf.Abs(distance) > innerPlayerDetector.transform.localScale.x * 0.7f) //0.7 gives some room for error with player staying detector
            {
                ChangeState(EnemyState.Walk);
            }
            else
            {
                ChangeState(EnemyState.Attack);
            }
        }

        public void BlockStop()
        {
            isBlocking = false;
        }

        private void PlayerEnteredDetector(object detector, Collider other)
        {
            if (currentState != EnemyState.Dead && (Detector)detector == outerPlayerDetector) ChangeState(EnemyState.Walk);
        }

        private void PlayerExitedDetector(object detector, Collider other)
        {
            if (currentState != EnemyState.Dead && (Detector)detector == outerPlayerDetector) ChangeState(EnemyState.Idle);
        }

        private void PlayerStayingInDetector(object detector, Collider other)
        {
            if (!isReacting && (Detector)detector == innerPlayerDetector)
            {
                ChangeState(EnemyState.Attack);
            }
        }

        private void ChangeState(EnemyState newState)
        {
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
            isBlocking = true;
            animation.Block();
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
        React,
        Escape,
        Block,
        Stunned,
        Dead
    }
}