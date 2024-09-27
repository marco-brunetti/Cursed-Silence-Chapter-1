using UnityEngine;
using System.Collections.Generic;
using Player;
using SnowHorse.Utils;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private new EnemyAnimation animation;
        [SerializeField] private EnemyData data;
        [SerializeField] private new Collider collider;

        [SerializeField] private Detector rightHandPlayerDetector;
        [SerializeField] private Detector leftHandPlayerDetector;
        [SerializeField] private Detector innerPlayerDetector;
        [SerializeField] private Detector outerPlayerDetector;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Renderer[] particleRenderers;

        private int currentHealth;
        private bool isEngaging;
        private bool canRecieveDamage;
        private EnemyState currentState = EnemyState.Idle;
        private Transform player;
        private Vector3 targetLookPosition;
        private float currentLerpTime;
        private List<Renderer> invisibleRenderers = new();

        #region AnimationEvents
        public void CanRecieveDamage() => canRecieveDamage = true;
        public void CantRecieveDamage() => canRecieveDamage = false;

        #endregion

        private static readonly int AnimatorDie = Animator.StringToHash("death");
        private static readonly int AnimatorIdle = Animator.StringToHash("idle");
        private static readonly int AnimatorAttack1 = Animator.StringToHash("attack");
        private static readonly int AnimatorWalkForward1 = Animator.StringToHash("walk_forward");
        private static readonly int AnimatorReactFront = Animator.StringToHash("react_front");


        private void OnEnable()
        {
            currentHealth = data.Health;

            rightHandPlayerDetector.DetectTag("Player");
            leftHandPlayerDetector.DetectTag("Player");
            innerPlayerDetector.DetectTag("Player");
            outerPlayerDetector.DetectTag("Player");

            Detector.ColliderEntered += PlayerDetected;
            Detector.ColliderExited += PlayerExitedDetector;
        }

        private void OnDisable()
        {
            Detector.ColliderEntered -= PlayerDetected;
            Detector.ColliderExited -= PlayerExitedDetector;
        }

        private void Start()
        {
            player = PlayerController.Instance.Player.transform;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void DealDamage(int damageAmount)
        {
            if (canRecieveDamage)
            {
                currentHealth -= damageAmount;
                Debug.Log($"Dealing damage {damageAmount} remaining enemyhealth: {currentHealth}");

                if (currentHealth <= 0) currentState = EnemyState.Dead;
                else animation.Set(AnimatorReactFront, true);
            }
        }

        private void PlayerDetected(object detector, Collider other)
        {
            var triggeredDetector = detector as Detector;
            if (currentState != EnemyState.Dead)
            {
                if (triggeredDetector == rightHandPlayerDetector || triggeredDetector == leftHandPlayerDetector)
                {
                    //Deal damage to player
                }
                else if (triggeredDetector == innerPlayerDetector)
                {
                    currentState = EnemyState.Attack;
                }
                else if (triggeredDetector == outerPlayerDetector)
                {
                    currentState = EnemyState.Walk;
                }
            }
        }

        private void PlayerExitedDetector(object detector, Collider other)
        {
            var triggeredDetector = detector as Detector;
            if (currentState != EnemyState.Dead)
            {
                if (triggeredDetector == rightHandPlayerDetector || triggeredDetector == leftHandPlayerDetector)
                {
                    //Deal damage to player
                }
                else if (triggeredDetector == innerPlayerDetector)
                {
                    currentState = EnemyState.Walk;
                }
            }
        }

        private void Update()
        {
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
            }
        }

        private void Die()
        {
            collider.enabled = false;
            innerPlayerDetector.gameObject.SetActive(false);
            outerPlayerDetector.gameObject.SetActive(false);
            leftHandPlayerDetector.gameObject.SetActive(false);
            rightHandPlayerDetector.gameObject.SetActive(false);
            animation.Set(AnimatorDie, true);
            animation.Set(AnimatorIdle, false);

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
            animation.Set(AnimatorIdle, true);
        }

        private void Walk()
        {
            LookAtPlayer();
            collider.enabled = true;
            animation.Set(AnimatorIdle, false);
            animation.Set(AnimatorAttack1, false);
            animation.Set(AnimatorWalkForward1, true);
        }

        private void Attack()
        {
            LookAtPlayer(0);
            collider.enabled = true;
            animation.Set(AnimatorAttack1, true);
            animation.Set(AnimatorIdle, false);
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