using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SnowHorse.Utils;
using UnityEngine;
using UnityEngine.AI;
using Game.General;

namespace Enemies
{
    //Attach to the game object that has the animator if the animation events are needed
    public class EnemyAnimation : MonoBehaviour
    {
        private readonly Dictionary<string, KeyValuePair<string,int>> animKeys = new();
        private float reactMoveSpeed;
        private float lookLerpTime;
        private Coroutine lookAtTarget;
        private Coroutine moveTowardsTarget;
        private Navigation navigation;
        private new AnimationManager animation;
        
        public string CurrentKey => animation.CurrentKeyString;
        public static EventHandler<AnimationEventArgs> AnimationEvent;
        
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

        public void SendAnimationEvent(string eventData)
        {
            AnimationEvent?.Invoke(this, JsonConvert.DeserializeObject<AnimationEventArgs>(eventData));
        }

        public void SetState(string animKey, Transform rootTransformForLook = null, Transform lookTarget = null, Transform moveTarget = null, bool moveRandomizedPath = false)
        {
            StopLooking();
            navigation.Stop();
            
            if (moveTarget) navigation.FollowPath(moveTarget, moveRandomizedPath);
            else if (lookTarget) LookAtTarget(rootTransformForLook, lookTarget);
            
            animation.Enable(animKeys[animKey]);
        }

        private void LookAtTarget(Transform rootTransform, Transform targetTransform, float duration = 50)
        {
            lookAtTarget = StartCoroutine(LookingAtTarget(rootTransform, targetTransform, duration));
        }

        private IEnumerator LookingAtTarget(Transform rootTransform, Transform targetTransform, float duration)
        {
            lookLerpTime = 0;
            var lookPos = rootTransform.position + rootTransform.forward;

            while (true)
            {
                var target = GetTargetDirOnYAxis(origin: rootTransform.position, target: targetTransform.position);

                if ((target - lookPos).magnitude < 0.01f) lookLerpTime = 0;
                else lookPos = Vector3.Slerp(lookPos, target, Interpolation.Linear(duration, ref lookLerpTime));

                rootTransform.LookAt(GetTargetDirOnYAxis(origin: rootTransform.position, target: lookPos));
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
    }

    public class AnimationEventArgs : EventArgs
    {
        [JsonProperty("name")] public string EventName;
        [JsonProperty("float")] public float FloatValue;
        [JsonProperty("int")] public int IntValue;
        [JsonProperty("bool")] public bool BoolValue;
        [JsonProperty("string")] public bool StringValue;
    }
}