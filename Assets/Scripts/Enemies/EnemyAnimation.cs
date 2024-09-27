using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Animations;

namespace Enemies
{
    public class EnemyAnimation : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AnimatorController controller;
        [SerializeField] private AnimationClip[] deathAnimations;
        [SerializeField] private AnimationClip[] idleAnimations;
        [SerializeField] private AnimationClip[] attackAnimations;
        [SerializeField] private AnimationClip[] walkForwardAnimations;
        [SerializeField] private AnimationClip[] reactFrontAnimations;

        private Dictionary<int, KeyValuePair<string, AnimationClip[]>> dict = new();
        private System.Random random;
        private AnimatorOverrideController aoc;

        public void DeactivateReactAnimation() => animator.SetBool(Animator.StringToHash("react_front"), false);

        private void Start()
        {
            random = new System.Random(Guid.NewGuid().GetHashCode());
            dict.Add(Animator.StringToHash("death"), new("death", deathAnimations));
            dict.Add(Animator.StringToHash("idle"), new("idle", idleAnimations));
            dict.Add(Animator.StringToHash("attack"), new("attack", attackAnimations));
            dict.Add(Animator.StringToHash("walk_forward"), new("walk_forward", walkForwardAnimations));
            dict.Add(Animator.StringToHash("react_front"), new("react_front", reactFrontAnimations));

            aoc = new AnimatorOverrideController(controller);
            animator.runtimeAnimatorController = aoc;
        }

        public void Set(int hash, bool enable)
        {
            /*if (enable)
            {
                var anim = dict[hash];
                var index = random.Next(anim.Value.Count());
                aoc[anim.Key] = anim.Value[index];

            }*/

            var anim = dict[hash];
            var index = random.Next(anim.Value.Count());
            aoc[anim.Key] = anim.Value[index];

            aoc = new AnimatorOverrideController(controller);
            animator.runtimeAnimatorController = aoc;

            Debug.Log($"animation key {anim.Key} value {anim.Value[index]}");

            animator.SetBool(hash, enable);
        }
    }
}