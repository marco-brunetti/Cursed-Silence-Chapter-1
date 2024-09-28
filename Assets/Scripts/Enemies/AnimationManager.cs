using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Animations;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private Dictionary<string, KeyValuePair<int, List<AnimationClip>>> dict = new();
    private System.Random random;
    private AnimatorOverrideController aoc;

    public AnimationManager(string[] animationKeys, Animator animator, AnimatorController animatorController, AnimationClip[] clips = null, string animationPath = "")
    {
        this.animator = animator;
        random = new System.Random(Guid.NewGuid().GetHashCode());
        aoc = new AnimatorOverrideController(animatorController);
        animator.runtimeAnimatorController = aoc;

        if(clips == null) clips = Resources.LoadAll<AnimationClip>(animationPath);

        foreach (var key in animationKeys)
        {
            if (!Array.Exists(animatorController.animationClips, x => x.name == key)) 
            {
                Debug.Log($"Warning: Animator controller does not contain state: {key}");
                continue;        
            }

            List<AnimationClip> animations = clips.Where(x => x.name.Contains(key)).ToList();
            if (animations.Count > 0) dict.Add(key, new(Animator.StringToHash(key), animations));
        }
    }

    public void Set(string key, bool enable)
    {
        var hash = Animator.StringToHash(key);
        if (animator.GetBool(hash) == enable) return;

        if (!dict.ContainsKey(key))
        {
            Debug.Log($"Animation key {key} not found.");
            return;
        }

        if (enable) ChangeCurrentClip(key);
        animator.SetBool(hash, enable);
    }

    public void ChangeCurrentClip(string key)
    {
        var anim = dict[key];
        var index = random.Next(anim.Value.Count());
        aoc[key] = anim.Value[index];
    }
}