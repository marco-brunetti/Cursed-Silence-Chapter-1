using System.Threading.Tasks;
using UnityEngine;
using System;

namespace Enemies
{
    public class EnemyController : MonoBehaviour
    {
        private static readonly int AnimatorDie = Animator.StringToHash("die");
        private static readonly int AnimatorIdle = Animator.StringToHash("idle");
        private static readonly int AnimatorAttack = Animator.StringToHash("attack");
        private static readonly int AnimatorReactFront = Animator.StringToHash("react_front");
        [SerializeField] private EnemyData data;
        [SerializeField] private new Collider collider;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerDetector rightPlayerDetector;
        [SerializeField] private PlayerDetector leftPlayerDetector;

        private int currentHealth;
        private bool isEngaging;
        private bool canParry;
        private EnemyState currentState = EnemyState.Idle;

        public void ActivateParry() => canParry = true;
        public void DeactivateParry() => canParry = false;

        public void DeactivateReactAnimation() => animator.SetBool(AnimatorReactFront, false);

        private void OnEnable()
        {
            currentHealth = data.Health;

            rightPlayerDetector.Controller = this;
            leftPlayerDetector.Controller = this;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void DealDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            Debug.Log($"Dealing damage {damageAmount} remaining enemyhealth: {currentHealth}");

            if (currentHealth <= 0)
            {
                currentState = EnemyState.Dead;
            }
            else
            {
                animator.SetBool(AnimatorReactFront, true);
            }
        }

        public void PlayerDetected()
        {
            Debug.Log($"Player detected");
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
            }
        }

        private void Die()
        {
            collider.enabled = false;
            animator.SetBool(AnimatorDie, true);
            animator.SetBool(AnimatorIdle, false);
        }

        private void Idle()
        {
            collider.enabled = true;
            /*animator.SetBool(AnimatorDie, false);
            animator.SetBool(AnimatorIdle, true);*/
        }

        private void Attack()
        {

        }
    }

    public enum EnemyState
    {
        Idle,
        Attack,
        Escape,
        Block,
        Stunned,
        Dead
    }
}