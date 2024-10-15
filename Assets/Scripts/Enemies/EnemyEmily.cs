using SnowHorse.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyEmily : Enemy
    {
        [SerializeField] private AnimationClip SpecialAttack2Clip;
        private float specialAttackLerpTime;
        private float specialAttackLerpTime2;
        private bool isTrackingStopped;
        private float defaultLookSpeed = 5;

        protected override void Start()
        {
            base.Start();
            animation.AddStateAnimations(EnemyState.Attack, SpecialAttack2Clip.name);
        }
        
        protected override void Move()
        {
            animation.SetState(data.MoveAnim.name, currentState, lookTarget:player, rootTransformForLook: transform);
            animation.SetLookSpeed(defaultLookSpeed);
        }

        protected override void OnWalkStarted(float speed)
        {
            if (currentState == EnemyState.Walk) //Add any other state that contains walk clip
            {
                animation.SetState(data.MoveAnim.name, currentState, moveTarget: player);
                animation.SetAgentSpeed(speed);
            }
                
        }

        protected override void Attack()
        {
            var attackKeysList = new List<string> { data.AttackAnim.name };
            if (hasHeavyAttack) attackKeysList.Add(data.HeavyAttackAnim.name);
            if (hasSpecialAttack) attackKeysList.AddRange(new List<string> { data.SpecialAttackAnim.name, SpecialAttack2Clip.name });

            attack ??= StartCoroutine(AttackingPlayer(attackKeysList));
        }

        protected override void SetRandomAttack()
        {
            if (currentState == EnemyState.Attack)
            {
                if (animation.CurrentKey == data.AttackAnim.name)
                {
                    var p = random.Next(0, 100);

                    if (p < data.SpecialAttackProbability) SelectSpecialAttack();
                    else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability) animation.SetState(data.HeavyAttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                }
                else
                {
                    RestartTracking();
                    if(currentState == EnemyState.Attack) animation.SetState(data.AttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                } 
            }
            else
            {
                RestartTracking();
            }

            changeNextAttack = false;
        }

        //Added second special attack for Emily
        private void SelectSpecialAttack()
        {
            StopMovementFunctions();

            var sp = random.Next(0, 100);
            if (sp < 50) animation.SetState(data.SpecialAttackAnim.name, currentState);
            else animation.SetState(SpecialAttack2Clip.name, currentState);
        }

        private void StopMovementFunctions()
        {
            StopPlayerTracking();
            animation.StopNavigation();
            animation.SetLookSpeed(0);
            isTrackingStopped = true;
        }

        private void RestartTracking()
        {
            if (isTrackingStopped)
            {
                StartPlayerTracking(visualConeOnly: false);
                isTrackingStopped = false;
            }
        }

        protected override void OnAnimationEvent(object sender, AnimationEventArgs args)
        {
            if ((EnemyAnimation)sender != animation) return;
            base.OnAnimationEvent(sender, args);
            if (args.Event == "started_special_attack_movement") StartCoroutine(SpecialAttackMovement());
        }

        //Add custom movement for special attack animation
        private IEnumerator SpecialAttackMovement()
        {
            var initialPosition = transform.position;
            var target = transform.position + transform.forward * 7f;

            //Getting this numbers manually from the animation clip
            var runAttackClipSec = 0.67f;
            var AttackWaitSec = 1.5f;
            var returnClipSec = 2.33f;

            //Total attack time
            var currentTime = runAttackClipSec + AttackWaitSec + returnClipSec;

            while(currentTime > 0)
            { 
                //Go and come back
                if(currentTime > AttackWaitSec + returnClipSec) transform.position = Vector3.Lerp(initialPosition, target, Interpolation.Linear(runAttackClipSec, ref specialAttackLerpTime));
                else if(currentTime < returnClipSec) transform.position = Vector3.Lerp(target, initialPosition, Interpolation.Smooth(returnClipSec, ref specialAttackLerpTime2));

                currentTime -= Time.deltaTime;
                yield return null;
            }

            //When the model sets foot on the ground, there is a half second delay before clip ends
            yield return new WaitForSecondsRealtime(0.5f); 

            specialAttackLerpTime = 0;
            specialAttackLerpTime2 = 0;
        }
    }
}