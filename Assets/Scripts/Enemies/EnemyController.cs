using UnityEngine;
using System.Collections.Generic;
using Player;
using SnowHorse.Utils;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    { 
        [SerializeField] private EnemyData data;
        [SerializeField] private new Collider collider;
        [SerializeField] private new EnemyAnimation animation;

        [SerializeField] private Detector rightHandPlayerDetector;
        [SerializeField] private Detector leftHandPlayerDetector;
        [SerializeField] private Detector innerPlayerDetector;
        [SerializeField] private Detector outerPlayerDetector;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Renderer[] particleRenderers;

        private int currentHealth;
        private bool isEngaging;
        private bool canRecieveDamage;
        public EnemyState CurrentState { get; private set; } = EnemyState.Idle;
        private Transform player;
        private Vector3 targetLookPosition;
        private float currentLerpTime;
        private List<Renderer> invisibleRenderers = new();

        public void CanRecieveDamage(bool enable)
        {
            if(true) //Check condition later
            {
                canRecieveDamage = enable;
            }
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

        private void Awake()
        {
            rightHandPlayerDetector.DetectTag("Player");
            leftHandPlayerDetector.DetectTag("Player");
            innerPlayerDetector.DetectTag("Player");
            outerPlayerDetector.DetectTag("Player");
            Detector.ColliderEntered += PlayerDetected;
            Detector.ColliderExited += PlayerExitedDetector;
            animation.Init(controller: this, enemyData: data);
        }

        private void Start()
        {
            player = PlayerController.Instance.Player.transform;
            targetLookPosition = player.position;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void DealDamage(int damageAmount)
        {
            if (CurrentState != EnemyState.Dead && canRecieveDamage)
            {
                currentHealth -= damageAmount;
                Debug.Log($"Dealing damage {damageAmount} remaining enemyhealth: {currentHealth}");

                if (currentHealth <= 0) CurrentState = EnemyState.Dead;
                else animation.React();
            }
        }

        private void PlayerDetected(object detector, Collider other)
        {
            if (CurrentState != EnemyState.Dead)
            {
                var triggeredDetector = detector as Detector;
                if (triggeredDetector == rightHandPlayerDetector || triggeredDetector == leftHandPlayerDetector)
                {
                    //Deal damage to player
                }
                else if (triggeredDetector == innerPlayerDetector)
                {
                    CurrentState = EnemyState.Attack;
                }
                else if (triggeredDetector == outerPlayerDetector)
                {
                    CurrentState = EnemyState.Walk;
                }
            }
        }

        private void PlayerExitedDetector(object detector, Collider other)
        {
            if (CurrentState != EnemyState.Dead)
            {
                var triggeredDetector = detector as Detector;

                if (triggeredDetector == rightHandPlayerDetector || triggeredDetector == leftHandPlayerDetector)
                {
                    //Deal damage to player
                }
                else if (triggeredDetector == innerPlayerDetector)
                {
                    CurrentState = EnemyState.Walk;
                }
            }
        }

        private void Update()
        {
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
        }

        private void Die()
        {
            collider.enabled = false;
            innerPlayerDetector.gameObject.SetActive(false);
            outerPlayerDetector.gameObject.SetActive(false);
            leftHandPlayerDetector.gameObject.SetActive(false);
            rightHandPlayerDetector.gameObject.SetActive(false);
            animation.Die();

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

        private void Idle()
        {
            LookAtPlayer();
            collider.enabled = true;
            animation.Idle();
        }

        private void Walk()
        {
            animation.Walk();
            LookAtPlayer();
            MoveTowardsPlayer();
            collider.enabled = true;
        }

        private void Attack()
        {
            LookAtPlayer();
            collider.enabled = true;
            animation.Attack();
        }

        private void LookAtPlayer(float correctionAngle = 0)
        {
            if ((targetLookPosition - player.position).magnitude < 0.01f)
            {
                currentLerpTime = 0;
            }
            else
            {
                var lerpDuration = (1 / (targetLookPosition - player.position).magnitude) * 50f;
                var percent = Interpolation.Smoother(lerpDuration, ref currentLerpTime);
                targetLookPosition = Vector3.Lerp(targetLookPosition, player.position, percent);
            }

            transform.LookAt(targetLookPosition);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + correctionAngle, 0);
        }

        private void MoveTowardsPlayer()
        {
            var speed = 3;
            var targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
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