using SnowHorse.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimation : MonoBehaviour
    {
        private bool canChangeAttackAnimation = true;
        private float reactMoveSpeed;
        private float lookLerpTime;
        private Vector3 lookPos;
        private Coroutine lookAtPlayer;
        private Coroutine moveTowards;
        private Animator animator;
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
        private readonly KeyValuePair<string, int> AnimBlock = new("block", Animator.StringToHash("block"));

        [NonSerialized] public float WalkSpeed;

        public void Init(EnemyController controller, EnemyData enemyData, Transform player)
        {
            animator = GetComponent<Animator>();
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
                AnimReactFront,
                AnimBlock
            };

            animation = new(animationKeys, animator, animatorController: data.AnimatorController, data.AnimationClips);
        }

        #region Animation Events
        public void CanRecieveDamage() => controller.CanRecieveDamage(true);
        public void CantRecieveDamage() => controller.CanRecieveDamage(false);
        public void ChangeCurrentAttackClip() => canChangeAttackAnimation = true;

        public void ReactStart(float speed) { reactMoveSpeed = speed; StartCoroutine(ReactMoveTimer()); }
        public void ReactStop()
        {
            reactMoveSpeed = 0;
            controller.ReactStop();
        }
        
        public void BlockStop() => controller.ReactStop();

        public void WalkStarted(float walkSpeed)
        { 
            if (animation.CurrentKey == AnimWalkForward.Value && moveTowards == null)
            {
                WalkSpeed = walkSpeed;
                MoveTowardsPlayer(true, WalkSpeed);
            }
        }
        #endregion

        public void Idle()
        {
            animation.EnableKey(AnimIdle, deactivateOtherKeys: true);
            LookAtPlayer(false);
            MoveTowardsPlayer(false);
        }
            
        public void Die()
        {
            animation.EnableKey(AnimDieForward, deactivateOtherKeys: true);
            LookAtPlayer(false);
            MoveTowardsPlayer(false);
        }
            
        public void React()
        {
            animation.EnableKey(AnimReactFront, deactivateOtherKeys: true);
            LookAtPlayer(false);
            MoveTowardsPlayer(false);
        }
        
        public void Block()
        {
            animation.EnableKey(AnimBlock, deactivateOtherKeys: true);
            LookAtPlayer(true);
            MoveTowardsPlayer(false);
        }
            
        public void Attack()
        {
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

            LookAtPlayer(true);
            MoveTowardsPlayer(false);
        }

        public void Walk()
        {
            animation.EnableKey(AnimWalkForward, deactivateOtherKeys: true);
            LookAtPlayer(true, 50);
        }
 
        private void MoveTowardsPlayer(bool isMoving, float speed = 0)
        {
            if (moveTowards != null)
            {
                StopCoroutine(moveTowards);
                moveTowards = null;
            }
            if (isMoving)
            {
                moveTowards = StartCoroutine(MovingTowardsPlayer(speed));
            }
        }

        private IEnumerator MovingTowardsPlayer(float speed)
        {
            while(true)
            {
                var targetPosition = new Vector3(player.position.x, controller.transform.position.y, player.position.z);
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
        }

        private void LookAtPlayer(bool isLooking, float duration = 50)
        {
            if (lookAtPlayer != null) StopCoroutine(lookAtPlayer);
            if (isLooking) lookAtPlayer = StartCoroutine(LookingAtPlayer(duration));
        }

        private IEnumerator LookingAtPlayer(float duration, float correctionAngle = 0)
        {
            lookLerpTime = 0;
            lookPos = controller.transform.position + controller.transform.forward;
            
            while (true)
            {
                var target = GetTargetDirOnYAxis(origin:controller.transform.position, target:player.position);
                
                if ((target - lookPos).magnitude < 0.01f) lookLerpTime = 0;
                else lookPos = Vector3.Lerp(lookPos, target, Interpolation.Smooth(duration, ref lookLerpTime));
                
                controller.transform.LookAt(GetTargetDirOnYAxis(origin:controller.transform.position, target:lookPos));
                yield return null;
            }
        }

        //Get direction with correct vector length
        private Vector3 GetTargetDirOnYAxis(Vector3 origin, Vector3 target)
        {
            var dir = (new Vector3(target.x, origin.y , target.z) - origin).normalized;
            return origin + dir;
        }

        private IEnumerator ReactMoveTimer()
        {
            while (reactMoveSpeed > 0)
            {
                controller.transform.position -= transform.forward * (reactMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}