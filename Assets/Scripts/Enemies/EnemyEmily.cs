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

        protected override void Attack()
        {
            var attackKeysList = new List<string> { data.AttackAnim.name };
            if (hasHeavyAttack) attackKeysList.Add(data.HeavyAttackAnim.name);
            if (hasSpecialAttack)
            {
                attackKeysList.Add(data.SpecialAttackAnim.name);
                attackKeysList.Add(SpecialAttack2Clip.name);
            }
            attack ??= StartCoroutine(AttackingPlayer(attackKeysList));
        }


        protected override void SetRandomAttack()
        {
            if (animation.CurrentKey == data.AttackAnim.name)
            {
                var p = 0;
                if (hasHeavyAttack || hasSpecialAttack) p = random.Next(0, 100);

                if (hasHeavyAttack && hasSpecialAttack)
                {
                    //Added second special attack for Emily
                    if (p < data.SpecialAttackProbability)
                    {
                        SelectSpecialAttack();
                    }
                    else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability)
                    {
                        animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                    }
                        
                }
                else if (hasHeavyAttack)
                {
                    if (p < data.HeavyAttackProbability) animation.SetState(data.HeavyAttackAnim.name, rootTransformForLook: transform, lookTarget: player);
                }
                else if (hasSpecialAttack)
                {
                    if (p < data.SpecialAttackProbability)
                    {
                        SelectSpecialAttack();
                    }
                }
            }
            else
            {
                if (isTrackingStopped)
                {
                    StartPlayerTracking();
                    isTrackingStopped = false;
                }
                    
                animation.SetState(data.AttackAnim.name, rootTransformForLook: transform, lookTarget: player);
            }

            changeNextAttack = false;
        }

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

            var currentTime = runAttackClipSec + AttackWaitSec + returnClipSec;

            while(currentTime > 0)
            {
                if(currentTime > AttackWaitSec + returnClipSec)
                {
                    transform.position = Vector3.Lerp(initialPosition, target, Interpolation.Linear(runAttackClipSec, ref specialAttackLerpTime));
                }
                else if(currentTime < returnClipSec)
                {
                    transform.position = Vector3.Lerp(target, initialPosition, Interpolation.Smooth(returnClipSec, ref specialAttackLerpTime2));
                }

                Debug.DrawRay(initialPosition, target, Color.blue);


                currentTime -= Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSecondsRealtime(0.5f); //When the model sets foot on the ground, there is a half second delay

            specialAttackLerpTime = 0;
            specialAttackLerpTime2 = 0;
        }
    }
}
