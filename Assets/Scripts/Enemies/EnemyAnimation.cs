using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;


        private EnemyController controller;
        private EnemyData data;

        private new AnimationManager animation;

        private static readonly KeyValuePair<string, int> AnimDieForward = new("death_forward", Animator.StringToHash("death_forward"));
        private static readonly KeyValuePair<string, int> AnimIdle = new("idle", Animator.StringToHash("idle"));
        private static readonly KeyValuePair<string, int> AnimAttack = new("attack", Animator.StringToHash("attack"));
        private static readonly KeyValuePair<string, int> AnimHeavyAttack = new("heavy_attack", Animator.StringToHash("heavy_attack"));
        private static readonly KeyValuePair<string, int> AnimWalkForward = new("walk_forward", Animator.StringToHash("walk_forward"));
        private static readonly KeyValuePair<string, int> AnimReactFront = new("react_front", Animator.StringToHash("react_front"));

        private bool canChangeAttackAnimation = true;

        public void Init(EnemyController controller, EnemyData enemyData)
        {
            this.controller = controller;
            data = enemyData;

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

        public void Idle() => animation.EnableKey(AnimIdle, deactivateOtherKeys: true);
        public void Walk() => animation.EnableKey(AnimWalkForward, deactivateOtherKeys: true);
        public void Die() => animation.EnableKey(AnimDieForward, deactivateOtherKeys: true);
        public void React() => animation.EnableKey(AnimReactFront, deactivateOtherKeys: true);
        public void CanRecieveDamage() => controller.CanRecieveDamage(true);
        public void CantRecieveDamage() => controller.CanRecieveDamage(false);
        public void DeactivateReactAnimation() => animation.DisableKey(AnimReactFront);
        public void ChangeCurrentAttackClip() => canChangeAttackAnimation = true;


        public void Attack()
        {
            var changedState = (animation.CurrentKey != AnimAttack.Value && animation.CurrentKey != AnimHeavyAttack.Value);

            if (canChangeAttackAnimation || changedState)
            {
                if (animation.CurrentKey == AnimAttack.Value) animation.EnableKey(AnimHeavyAttack, deactivateOtherKeys: true);
                else animation.EnableKey(AnimAttack, deactivateOtherKeys: true);
                canChangeAttackAnimation = false;
            }
        }
    }
}