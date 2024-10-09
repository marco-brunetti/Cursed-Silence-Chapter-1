using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class AnimationManager
{
    private Animator animator;
    private Dictionary<int, List<AnimationClip>> dict = new();
    private System.Random random;
    private AnimatorOverrideController aoc;
    private AnimationClip[] originalControllerClips;
    public int CurrentKey { get; private set; }

    public AnimationManager(KeyValuePair<string, int>[] animationKeys, Animator animator, RuntimeAnimatorController animatorController, AnimationClip[] clips = null, string animationPath = "")
    {
        this.animator = animator;
        random = new System.Random(Guid.NewGuid().GetHashCode());
        aoc = new AnimatorOverrideController(animatorController);
        animator.runtimeAnimatorController = aoc;

        if (clips == null) clips = Resources.LoadAll<AnimationClip>(animationPath);
        originalControllerClips = animatorController.animationClips;

        foreach (var animKey in animationKeys)
        {
            //The animation already in the controller must be equals the key
            if (!Array.Exists(originalControllerClips, x => x.name == animKey.Key))
            {
                Debug.Log($"Warning: Animator controller does not contain state: {animKey.Key.ToUpper()}");
                continue;
            }

            var nameFilter = animKey.Key.Length + 3; //Makes sure only this key is present and not another with the same word included
            List<AnimationClip> animations = clips.Where(x => x.name.Contains(animKey.Key) && x.name.Length <= nameFilter).ToList();
            if (animations.Count > 0) dict.Add(animKey.Value, animations);
        }
    }

    public void Enable(KeyValuePair<string, int> key, bool deactivateOtherKeys = false)
    {
        if (animator.GetBool(key.Value) == true) return;
        if (deactivateOtherKeys) dict.Keys.ToList().ForEach(hash => { if (hash != key.Value) animator.SetBool(hash, false); });
        ChangeClip(key);
    }

    public void ChangeNextState(KeyValuePair<string, int> key1, KeyValuePair<string, int> key2, int key2Probability = 100)
    {
        if (CurrentKey == key1.Value && random.Next(100) < key2Probability)
        {
            Enable(key2);
        }
        else
        {
            Enable(key1);
        }
    }

    public void Disable(KeyValuePair<string, int> key)
    {
        if (animator.GetBool(key.Value) == true)
        {
            animator.SetBool(key.Value, false);
            CurrentKey = 0;
        }
    }

    private void ChangeClip(KeyValuePair<string, int> animKey)
    {
        if (!dict.ContainsKey(animKey.Value))
        {
            Debug.Log($"Animation key {animKey.Key.ToUpper()} not found. Check if the animator contains a state with {animKey.Key.ToUpper()} clip.");
            return;
        }

        var anim = dict[animKey.Value];
        var index = random.Next(anim.Count());
        aoc[animKey.Key] = anim[index];
        animator.SetBool(animKey.Value, true);
        CurrentKey = animKey.Value;
    }
}