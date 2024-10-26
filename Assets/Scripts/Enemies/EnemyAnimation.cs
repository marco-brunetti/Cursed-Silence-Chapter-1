using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using SnowHorse.Systems;

namespace Enemies
{
    //Attach to the game object that has the animator if the animation events are needed
    public class EnemyAnimation : MonoBehaviour
    {
        private readonly Dictionary<string, KeyValuePair<string,int>> animKeys = new();
        private float reactMoveSpeed;
        private float lookSpeed;
        private Coroutine lookAtTarget;
        private Coroutine moveTowardsTarget;
        private NavigationManager navigation;
        private new AnimationManager animation;
        private Dictionary<EnemyState, List<string>> stateAnims = new();
        
        public string CurrentKey => animation.CurrentKeyString;
        public static EventHandler<AnimationEventArgs> AnimationClipEvent;
        
        public void Init(EnemyData enemyData, NavMeshAgent agent)
        {
            navigation = gameObject.AddComponent<NavigationManager>();
            navigation.Init(agent, enemyData.PathFindInterval);
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
            
            SetStateAnimations(data);
        }

        private void SetStateAnimations(EnemyData data)
        {
            var attackAnims = new List<AnimationClip> { data.AttackAnim, data.HeavyAttackAnim, data.SpecialAttackAnim };
            var moveAnims = new List<AnimationClip> { data.MoveAnim };
            var idleAnims = new List<AnimationClip> { data.IdleAnim };
            var deadAnims = new List<AnimationClip> { data.DeathAnim };
            var blockAnims = new List<AnimationClip> { data.BlockAnim };
            var reactAnims = new List<AnimationClip> { data.ReactAnim };

            stateAnims.TryAdd(EnemyState.Attack, attackAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.Walk, moveAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.Idle, idleAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.Dead, deadAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.Block, blockAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.React, reactAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.Wander, moveAnims.Where(x=> x != null).Select(x=> x.name).ToList());
            stateAnims.TryAdd(EnemyState.Escape, moveAnims.Where(x=> x != null).Select(x=> x.name).ToList());
        }
        
        public void AddStateAnimations(EnemyState state, string animKey)
        {
            if (!stateAnims[state].Contains(animKey)) stateAnims[state].Add(animKey);
        }

        //Set in the animation clip with a string json
        public void AnimEvent(string eventData)
        {
            //Using JsonUtility as it is more performant than JsonConvert
            var args = JsonUtility.FromJson<AnimationEventArgs>(eventData);
            AnimationClipEvent?.Invoke(this, args);
        }

        public void SetState(string animKey, EnemyState currentState, Transform rootTransformForLook = null, Transform lookTarget = null, float lookTargetSpeed = 5, Transform moveTarget = null, bool randomizePath = false, float randomPathRange = 2f)
        {
            //We check if the current state corresponds to the anim key, to prevent accidental changes
            if (stateAnims[currentState].Contains(animKey))
            {
                StopLooking();
                navigation.Stop();
            
                if (moveTarget) navigation.FollowPath(moveTarget, randomizePath, randomPathRange);
                else if (lookTarget) LookAtTarget(rootTransformForLook, lookTarget, lookTargetSpeed);
            
                animation.Enable(animKeys[animKey]);
            }
            else
            {
                Debug.LogError($"Invalid anim key {animKey} for state {currentState}.");
            }
        }

        private void LookAtTarget(Transform rootTransform, Transform targetTransform, float lookTargetSpeed)
        {
            StopLooking();
            lookSpeed = lookTargetSpeed;
            lookAtTarget = StartCoroutine(LookingAtTarget(rootTransform, targetTransform));
        }

        private IEnumerator LookingAtTarget(Transform rootTransform, Transform targetTransform)
        {
            while (true)
            {
                if(lookSpeed > 0)
                {
                    var direction = targetTransform.position - rootTransform.position;
                    var step = lookSpeed * Time.deltaTime;
                    var newRotation = Vector3.RotateTowards(rootTransform.forward, direction, step, 0.0f);
                    rootTransform.rotation = Quaternion.LookRotation(newRotation);
                }

                yield return null;
            }
        }

        private void StopLooking()
        {
            lookSpeed = 0;
            if (lookAtTarget != null) StopCoroutine(lookAtTarget);
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
        public void StopNavigation(bool stopAgentCompletely = false) => navigation.Stop(stopAgentCompletely);
        public void DestroyAgent() => navigation.DestroyAgent();
        public void SetLookSpeed(float speed) => lookSpeed = speed;
    }

    public record AnimationEventArgs
    {
        public string Clip;
        public string Event;
        public float Float;
        public bool Bool;
    }
}