using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Enemies
{
    //Attach to the game object that has the animator if the animation events are needed
    public class EnemyAnimation : MonoBehaviour
    {
        private float reactMoveSpeed;
        private float lookLerpTime;
        private float moveSpeed;
        private Vector3 lookPos;
        private Coroutine lookAtTarget;
        private Coroutine moveTowardsTarget;
        private Animator animator;
        private Enemy enemy;
        private EnemyData data;
        private new AnimationManager animation;
        private readonly Dictionary<string, KeyValuePair<string,int>> animKeys = new();
        
        public string CurrentKey => animation.CurrentKeyString;
        public void Init(Enemy enemy, EnemyData enemyData)
        {
            animator = GetComponent<Animator>();
            this.enemy = enemy;
            data = enemyData;
            SetAnimationKeys(data);
        }

        private void SetAnimationKeys(EnemyData data)
        {
            var animList = new List<AnimationClip> { data.IdleAnim, data.MoveAnim, data.AttackAnim, data.DeathAnim, 
                data.HeavyAttackAnim, data.SpecialAttackAnim, data.ReactAnim, data.BlockAnim };
            
            animList.AddRange(data.AdditionalStateAnims);

            //adding original anim names to dictionary before the alternative ones are processed
            foreach (var x in animList.ToList())
            {
                if(x == null) animList.Remove(x);
                else animKeys.Add(x.name, new KeyValuePair<string, int>(x.name, Animator.StringToHash(x.name)));
            }

            animList.AddRange(data.AlternativeClips.Where(x=> x != null));
            
            animation = new AnimationManager(animKeys.Values.ToArray(), animator, animatorController: data.AnimatorController, animList.ToArray());
        }

        #region Animation Events
        public void SetVulnerable(string flag) => enemy.IsVulnerable(flag.ToLower() == "true");
        public void ChangeNextAttackClip() => enemy.ChangeNextAttack(true);

        public void ReactStart(float speed) { reactMoveSpeed = speed; StartCoroutine(ReactMoving()); }
        public void ReactStopMovement() => reactMoveSpeed = 0;
        public void ReactStop()
        {
            reactMoveSpeed = 0;
            enemy.ReactStop();
        }

        public void BlockStop() => enemy.ReactStop();

        public void WalkStarted(float speed) => moveSpeed = speed;

        #endregion

        public void SetState(string animKey, Transform lookTarget = null, Transform moveTarget = null, float moveSpeed = 0)
        {
            if (lookTarget) LookAtTarget(lookTarget);
            else StopLooking();
            
            this.moveSpeed = moveSpeed;
            if (moveTarget) MoveTowardsTarget(moveTarget);
            else StopMoving(); 
        
            animation.Enable(animKeys[animKey]);
        }

        private void MoveTowardsTarget(Transform targetTransform)
        {
            StopMoving();
            moveTowardsTarget = StartCoroutine(MovingTowardsTarget(targetTransform));
        }

        private IEnumerator MovingTowardsTarget(Transform targetTransform)
        {
            while (true)
            {
                var targetPosition = new Vector3(targetTransform.position.x, enemy.transform.position.y, targetTransform.position.z);
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private void StopMoving()
        {
            if (moveTowardsTarget != null)
            {
                StopCoroutine(moveTowardsTarget);
                moveTowardsTarget = null;
                moveSpeed = 0;
            }
        }

        private void LookAtTarget(Transform targetTransform, float duration = 50)
        {
            StopLooking();
            lookAtTarget = StartCoroutine(LookingAtTarget(targetTransform, duration));
        }

        private IEnumerator LookingAtTarget(Transform targetTransform, float duration)
        {
            lookLerpTime = 0;
            lookPos = enemy.transform.position + enemy.transform.forward;

            while (true)
            {
                var target = GetTargetDirOnYAxis(origin: enemy.transform.position, target: targetTransform.position);

                if ((target - lookPos).magnitude < 0.01f) lookLerpTime = 0;
                else lookPos = Vector3.Slerp(lookPos, target, Interpolation.Linear(duration, ref lookLerpTime));

                enemy.transform.LookAt(GetTargetDirOnYAxis(origin: enemy.transform.position, target: lookPos));
                yield return null;
            }
        }

        private void StopLooking()
        {
            if (lookAtTarget != null) StopCoroutine(lookAtTarget);
        }

        //Get direction with correct vector length
        private Vector3 GetTargetDirOnYAxis(Vector3 origin, Vector3 target, bool debug = false, Color? color = null)
        {
            var finalPos = origin + (new Vector3(target.x, origin.y, target.z) - origin).normalized;
            if (debug && color != null) Debug.DrawLine(origin, finalPos, (Color)color);
            return finalPos;
        }

        private IEnumerator ReactMoving()
        {
            while (reactMoveSpeed > 0)
            {
                enemy.transform.position -= transform.forward * (reactMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}