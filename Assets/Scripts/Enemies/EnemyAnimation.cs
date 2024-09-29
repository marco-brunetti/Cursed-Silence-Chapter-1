using UnityEngine;

namespace Enemies
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;


        private EnemyController controller;
        private EnemyData data;

        private new AnimationManager animation;
        private static readonly string AnimatorDieForward = "death_forward";
        private static readonly string AnimatorIdle = "idle";
        private static readonly string AnimatorAttack = "attack";
        private static readonly string AnimatorHeavyAttack = "heavy_attack";
        private static readonly string AnimatorWalkForward = "walk_forward";
        private static readonly string AnimatorReactFront = "react_front";

        public void Init(EnemyController controller, EnemyData enemyData)
        {
            this.controller = controller;
            data = enemyData;

            string[] animationKeys = { AnimatorDieForward, AnimatorIdle, AnimatorAttack, AnimatorHeavyAttack, AnimatorWalkForward, AnimatorReactFront };
            animation = new(animationKeys, animator, animatorController: data.AnimatorController, data.AnimationClips);
        }

        public void Idle() => animation.EnableKey(AnimatorIdle, deactivateOtherKeys: true);
        public void Walk() => animation.EnableKey(AnimatorWalkForward, deactivateOtherKeys: true);
        public void Attack() => animation.EnableKey(AnimatorAttack, deactivateOtherKeys: true);
        public void Die() => animation.EnableKey(AnimatorDieForward, deactivateOtherKeys: true);
        public void React() => animation.EnableKey(AnimatorReactFront, deactivateOtherKeys: true);
        public void CanRecieveDamage() => controller.CanRecieveDamage(true);
        public void CantRecieveDamage() => controller.CanRecieveDamage(false);
        public void DeactivateReactAnimation() => animation.DisableKey(AnimatorReactFront);
        public void ChangeCurrentAttackClip() { if (controller.CurrentState == EnemyState.Attack) animation.ChangeNextStateClip(AnimatorAttack, AnimatorHeavyAttack); }
    }
}