using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class AnimationManager : MonoBehaviour
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

        if(clips == null) clips = Resources.LoadAll<AnimationClip>(animationPath);
        originalControllerClips = animatorController.animationClips;

        foreach (var animKey in animationKeys)
        {
            //The animation already in the controller must be equals the key
            if (!Array.Exists(originalControllerClips, x => x.name == animKey.Key)) 
            {
                Debug.Log($"Warning: Animator controller does not contain state: {animKey.Key}");
                continue;        
            }

            var nameFilter = animKey.Key.Length + 3; //Makes sure only this key is present and not another with the same word included
            List<AnimationClip> animations = clips.Where(x => x.name.Contains(animKey.Key) && x.name.Length <= nameFilter).ToList();
            if (animations.Count > 0) dict.Add(animKey.Value, animations);
        }
    }

    public void EnableKey(KeyValuePair<string, int> animKey, bool deactivateOtherKeys = false)
    {
        if (animator.GetBool(animKey.Value) == true) return;
        if (deactivateOtherKeys) dict.Keys.ToList().ForEach(hash => { if (hash != animKey.Value) animator.SetBool(hash, false); });
        ChangeClip(animKey);
    }

    public void DisableKey(KeyValuePair<string, int> animKey)
    {
        if (animator.GetBool(animKey.Value) == true)
        {
            animator.SetBool(animKey.Value, false);
            CurrentKey = 0;
        }
    }

    public void ChangeClip(KeyValuePair<string, int> animKey)
    {
        if (!dict.ContainsKey(animKey.Value))
        {
            Debug.Log($"Animation key {animKey} not found.");
            return;
        }

        var anim = dict[animKey.Value];
        var index = random.Next(anim.Count());
        aoc[animKey.Key] = anim[index];
        animator.SetBool(animKey.Value, true);
        CurrentKey = animKey.Value;
    }
}