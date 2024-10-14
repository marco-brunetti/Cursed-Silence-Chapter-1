using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.AI;
using Game.General;
using static UnityEngine.GraphicsBuffer;

namespace Enemies
{
    //Attach to the game object that has the animator if the animation events are needed
    public class EnemyAnimation : MonoBehaviour
    {
        private readonly Dictionary<string, KeyValuePair<string,int>> animKeys = new();
        private float reactMoveSpeed;
        private float lookLerpTime;
        private float lookSpeed;
        private Coroutine lookAtTarget;
        private Coroutine moveTowardsTarget;
        private Navigation navigation;
        private new AnimationManager animation;
        
        public string CurrentKey => animation.CurrentKeyString;
        public static EventHandler<AnimationEventArgs> AnimationClipEvent;
        
        public void Init(EnemyData enemyData, NavMeshAgent agent)
        {
            navigation = gameObject.AddComponent<Navigation>();
            navigation.Init(agent);
            SetAnimationKeys(enemyData);
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
            animation = new AnimationManager(animKeys.Values.ToArray(), GetComponent<Animator>(), animatorController: data.AnimatorController, animList.ToArray());
        }

        //Set in the animation clip with a string json
        public void AnimEvent(string eventData)
        {
            //Using JsonUtility as it is more performant than JsonConvert
            var args = JsonUtility.FromJson<AnimationEventArgs>(eventData);
            AnimationClipEvent?.Invoke(this, args);
        }
        
        

        public void SetState(string animKey, Transform rootTransformForLook = null, Transform lookTarget = null, Transform moveTarget = null, bool randomizePath = false, float randomPathRange = 2f)
        {
            StopLooking();
            navigation.Stop();
            
            if (moveTarget) navigation.FollowPath(moveTarget, randomizePath, randomPathRange);
            else if (lookTarget) LookAtTarget(rootTransformForLook, lookTarget);
            
            animation.Enable(animKeys[animKey]);
        }

        private void LookAtTarget(Transform rootTransform, Transform targetTransform)
        {
            StopLooking();
            lookAtTarget = StartCoroutine(LookingAtTarget(rootTransform, targetTransform));
        }

        private IEnumerator LookingAtTarget(Transform rootTransform, Transform targetTransform)
        {
            lookLerpTime = 0;
            var lookPos = rootTransform.position + rootTransform.forward;

            while (true)
            {
                if(lookSpeed > 0)
                {
                    Vector3 relativeTarget = (targetTransform.position - rootTransform.transform.position).normalized;
                    //Vector3.right if you have a sprite rotated in the right direction
                    Quaternion toQuaternion = Quaternion.FromToRotation(Vector3.forward, relativeTarget);
                    rootTransform.transform.rotation = Quaternion.Slerp(rootTransform.transform.rotation, toQuaternion, lookSpeed * Time.deltaTime);
                    /*var target = GetTargetDirOnYAxis(origin: rootTransform.position, target: targetTransform.position);

                    var optimalDuration = 50f;
                    var duration = optimalDuration / lookSpeed;

                    if ((target - lookPos).magnitude < 0.01f) lookLerpTime = 0;
                    else lookPos = Vector3.Slerp(lookPos, target, Interpolation.Linear(duration, ref lookLerpTime));

                    Vector3.Angle(lookPos, target);

                    rootTransform.LookAt(GetTargetDirOnYAxis(origin: rootTransform.position, target: lookPos));*/
                }

                yield return null;
            }
        }

        private void StopLooking()
        {
            lookSpeed = 0;
            if (lookAtTarget != null) StopCoroutine(lookAtTarget);
        }

        //Get direction with correct vector length
        private Vector3 GetTargetDirOnYAxis(Vector3 origin, Vector3 target, bool debug = false, Color? color = null)
        {
            var finalPos = origin + (new Vector3(target.x, origin.y, target.z) - origin).normalized;
            if (debug && color != null) Debug.DrawLine(origin, finalPos, (Color)color);
            return finalPos;
        }

        public void StartReact(Transform rootTransform, float moveSpeed)
        {
            reactMoveSpeed = moveSpeed;
            StartCoroutine(ReactMoving(rootTransform));
        }
        
        private IEnumerator ReactMoving(Transform rootTransform)
        {
            while (reactMoveSpeed > 0)
            {
                rootTransform.position -= rootTransform.forward * (reactMoveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        
        public void StopReact() => reactMoveSpeed = 0;
        public void SetAgentSpeed(float speed) => navigation.SetAgentSpeed(speed);
        public void DestroyAgent() => navigation.DestroyAgent();
        public void StopNavigation() => navigation.Stop();
        public void SetLookSpeed(float speed)
        {
            lookLerpTime = 0;
            lookSpeed = speed;
        }   
    }

    public record AnimationEventArgs
    {
        public string Clip;
        public string Event;
        public float Float;
        public bool Bool;
    }
}