using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Enemies
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private EnemyController controller;
        private EnemyData data;
        private Transform player;

        private new AnimationManager animation;

        private readonly KeyValuePair<string, int> AnimDieForward = new("death_forward", Animator.StringToHash("death_forward"));
        private readonly KeyValuePair<string, int> AnimIdle = new("idle", Animator.StringToHash("idle"));
        private readonly KeyValuePair<string, int> AnimAttack = new("attack", Animator.StringToHash("attack"));
        private readonly KeyValuePair<string, int> AnimHeavyAttack = new("heavy_attack", Animator.StringToHash("heavy_attack"));
        private readonly KeyValuePair<string, int> AnimWalkForward = new("walk_forward", Animator.StringToHash("walk_forward"));
        private readonly KeyValuePair<string, int> AnimReactFront = new("react_front", Animator.StringToHash("react_front"));

        private bool canChangeAttackAnimation = true;

        [NonSerialized] public float WalkSpeed;
        private float reactMoveSpeed;

        public void Init(EnemyController controller, EnemyData enemyData, Transform player)
        {
            this.controller = controller;
            data = enemyData;
            this.player = player;

            KeyValuePair<string, int>[] animationKeys =
            {
                AnimDieForward,
                AnimIdle,
                AnimAttack,
                AnimHeavyAttack,
                AnimWalkForward,
                AnimReactFront
            };

            animation = new(animationKeys, animator, animatorController: data.AnimatorController, data.AnimationClips);
        }

        #region Animation Events
        public void CanRecieveDamage() => controller.CanRecieveDamage(true);
        public void CantRecieveDamage() => controller.CanRecieveDamage(false);
        public void ChangeCurrentAttackClip() => canChangeAttackAnimation = true;
        public void WalkStarted(float walkSpeed) => WalkSpeed = walkSpeed;
        public void ReactStart(float speed) { reactMoveSpeed = speed; StartCoroutine(ReactMoveTimer()); }
        public void ReactStopAnimation() => animation.DisableKey(AnimReactFront);
        public void ReactStopMovement() => reactMoveSpeed = 0;
        #endregion

        public void Idle() => animation.EnableKey(AnimIdle, deactivateOtherKeys: true);
        public void Die() => animation.EnableKey(AnimDieForward, deactivateOtherKeys: true);
        public void React() => animation.EnableKey(AnimReactFront, deactivateOtherKeys: true);

        public void Attack()
        {
            if (animation.CurrentKey == AnimReactFront.Value) return;

            if(animation.CurrentKey != AnimAttack.Value && animation.CurrentKey != AnimHeavyAttack.Value)
            {
                animation.EnableKey(AnimAttack, deactivateOtherKeys: true);
            }
            else if (canChangeAttackAnimation)
            {
                if (animation.CurrentKey == AnimAttack.Value) animation.EnableKey(AnimHeavyAttack, deactivateOtherKeys: true);
                else animation.EnableKey(AnimAttack, deactivateOtherKeys: true);
                canChangeAttackAnimation = false;
            }
        }

        public void Walk()
        {
            animation.EnableKey(AnimWalkForward, deactivateOtherKeys: true);
            MoveTowards(player.position, WalkSpeed);
        }

        private IEnumerator ReactMoveTimer()
        {
            while (reactMoveSpeed > 0)
            {
                controller.transform.position -= transform.forward * reactMoveSpeed * Time.deltaTime;
                yield return null;
            }
        }
            
        private void MoveTowards(Vector3 target, float speed)
        {
            var targetPosition = new Vector3(target.x, controller.transform.position.y, target.z);
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, targetPosition, speed * Time.deltaTime);
        }
    }
}