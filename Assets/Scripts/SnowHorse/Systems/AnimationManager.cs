using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SnowHorse.Systems
{
    public class AnimationManager
    {
        private readonly Animator animator;
        private readonly Dictionary<int, List<AnimationClip>> dict = new();
        private readonly System.Random random;
        private readonly AnimatorOverrideController aoc;
        public int CurrentKey { get; private set; }
        public string CurrentKeyString { get; private set; }

        
        /// <summary>
        /// AnimationManager is a utility script that helps register animation states based on a set of predefined keys.
        /// The class is created with a set of key-value pairs where the key is the name of the animation state in the animator
        /// controller and the value is the hash key to access the animation clips.
        ///
        /// If no clips are provided, the AnimationManager class will then look for all AnimationClips
        /// with the provided key in the provided path. If the AnimationClip does not exist in the
        /// Resources folder, but does exist in the animator controller, a warning is logged.
        ///
        /// Finally, the AnimationManager will randomize the animation clips based on the provided keys.
        /// </summary>
        public AnimationManager(KeyValuePair<string, int>[] animationKeys, Animator animator, RuntimeAnimatorController animatorController, AnimationClip[] clips = null, string animationPath = "")
        {
            this.animator = animator;
            random = new System.Random(Guid.NewGuid().GetHashCode());
            aoc = new AnimatorOverrideController(animatorController);
            animator.runtimeAnimatorController = aoc;

            if (clips == null) clips = Resources.LoadAll<AnimationClip>(animationPath);
            var originalClips = animatorController.animationClips;

            foreach (var animKey in animationKeys)
            {
                //The animation already in the controller must be equals the key
                if (!Array.Exists(originalClips, x => x.name == animKey.Key))
                {
                    Debug.LogError($"Warning: Animator controller does not contain state: {animKey.Key.ToUpper()}");
                    continue;
                }

                var nameFilter = animKey.Key.Length + 3; //Makes sure only this key is present and not another with the same word included
                List<AnimationClip> animations = clips.Where(x => x != null && x.name.Contains(animKey.Key) && x.name.Length <= nameFilter).ToList();
                if(clips.Any(x=> x == null)) Debug.LogError($"Animation clip null. Check if all references in data and controller are set.");
                if (animations.Count > 0) dict.Add(animKey.Value, animations);
            }
        }

        /// <summary>
        /// Enables a key and plays a random animation clip.
        /// </summary>
        /// <param name="key">The key to enable. The key should be a name of a state clip and corresponding bool in the animator controller.</param>
        /// <param name="deactivateOtherKeys">If true, all other keys will be disabled</param>
        public void Enable(KeyValuePair<string, int> key, bool deactivateOtherKeys = true)
        {
            if (animator.GetBool(key.Value)) return;
            if (deactivateOtherKeys) dict.Keys.ToList().ForEach(hash => { if (hash != key.Value) animator.SetBool(hash, false); });
            ChangeClip(key);
        }

        private void ChangeClip(KeyValuePair<string, int> animKey)
        {
            if (!dict.TryGetValue(animKey.Value, out var anim))
            {
                Debug.LogError($"Animation key {animKey.Key.ToUpper()} not found. Check if the animator contains a state with {animKey.Key.ToUpper()} clip.");
                return;
            }

            var index = random.Next(anim.Count());
            aoc[animKey.Key] = anim[index];
            animator.SetBool(animKey.Value, true);
            CurrentKey = animKey.Value;
            CurrentKeyString = animKey.Key;
        }
    }
}