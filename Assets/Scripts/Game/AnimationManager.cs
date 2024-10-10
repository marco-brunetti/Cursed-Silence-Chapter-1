using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class AnimationManager
{
    private readonly Animator animator;
    private readonly Dictionary<int, List<AnimationClip>> dict = new();
    private readonly System.Random random;
    private readonly AnimatorOverrideController aoc;
    public int CurrentKey { get; private set; }
    public string CurrentKeyString { get; private set; }

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

    //Original state clip and bool should have the same name as the key string
    public void Enable(KeyValuePair<string, int> key, bool deactivateOtherKeys = true)
    {
        if (animator.GetBool(key.Value)) return;
        if (deactivateOtherKeys) dict.Keys.ToList().ForEach(hash => { if (hash != key.Value) animator.SetBool(hash, false); });
        ChangeClip(key);
    }

    // ReSharper disable Unity.PerformanceAnalysis
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