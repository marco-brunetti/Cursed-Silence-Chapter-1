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

        protected override void Move()
        {
            animation.SetState(data.MoveAnim.name, lookTarget:player, rootTransformForLook: transform);
            animation.SetLookSpeed(1f);
        }

        protected override void OnWalkStarted(float speed)
        {
            animation.SetState(data.MoveAnim.name, moveTarget:player);
            animation.SetAgentSpeed(speed);
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
                    var p = 0;
                    if (hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);

                    if (hasHeavyAttack && hasSpecialAttack)
                    {
                        if (p < data.SpecialAttackProbability) SelectSpecialAttack();
                        else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);     
                    }
                    else if (hasHeavyAttack)
                    {
                        if (p < data.HeavyAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                    }
                    else if (hasSpecialAttack)
                    {
                        if (p < data.SpecialAttackProbability) SelectSpecialAttack();
                    }
                }
                else
                {
                    if (isTrackingStopped)
                    {
                        StartPlayerTracking(visualConeOnly: false);
                        isTrackingStopped = false;
                    }
                    
                    animation.SetState(data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                } 
            }
            else
            {
                if (isTrackingStopped)
                {
                    StartPlayerTracking(visualConeOnly: false);
                    isTrackingStopped = false;
                }
            }

            changeNextAttack = false;
        }

        //Added second special attack for Emily
        private void SelectSpecialAttack()
        {
            StopMovementFunctions();

            var sp = random.Next(0, 100);
            if (sp < 50) animation.SetState(data.SpecialAttackAnim.name);
            else animation.SetState(SpecialAttack2Clip.name);
        }

        private void StopMovementFunctions()
        {
            StopPlayerTracking();
            animation.StopNavigation();
            animation.SetLookSpeed(0);
            isTrackingStopped = true;
        }

        protected override void OnAnimationEvent(object sender, AnimationEventArgs args)
        {
            base.OnAnimationEvent(sender, args);
            if ((EnemyAnimation)sender != animation) return;

            switch (args.Event)
            {
                case "started_special_attack_movement":
                    StartCoroutine(SpecialAttackMovement());
                    break;
            }
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