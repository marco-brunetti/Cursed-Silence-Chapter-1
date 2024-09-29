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
    public string CurrentKey { get; private set; }

    public AnimationManager(string[] animationKeys, Animator animator, AnimatorController animatorController, AnimationClip[] clips = null, string animationPath = "")
    {
        this.animator = animator;
        random = new System.Random(Guid.NewGuid().GetHashCode());
        aoc = new AnimatorOverrideController(animatorController);
        animator.runtimeAnimatorController = aoc;

        if(clips == null) clips = Resources.LoadAll<AnimationClip>(animationPath);

        foreach (var key in animationKeys)
        {
            if (!Array.Exists(animatorController.animationClips, x => x.name == key)) //The animation already in the controller must be equals the key
            {
                Debug.Log($"Warning: Animator controller does not contain state: {key}");
                continue;        
            }

            var nameFilter = key.Length + 3; //Makes sure only this key is present and not another with the same word included
            List<AnimationClip> animations = clips.Where(x => x.name.Contains(key) && x.name.Length <= nameFilter).ToList();
            if (animations.Count > 0) dict.Add(key, new(Animator.StringToHash(key), animations));
        }
    }

    public void EnableKey(string key, bool deactivateOtherKeys = false)
    {
        if(deactivateOtherKeys)
        {
            foreach(var kvp in dict)
            {
                if (kvp.Key != key) Set(key, false);
            }
        }

        Set(key, true);
    }

    public void DisableKey(string key) => Set(key, false);

    private void Set(string key, bool enable)
    {
        var hash = Animator.StringToHash(key);
        if (animator.GetBool(hash) == enable) return;
        animator.SetBool(hash, enable);
        if (enable) ChangeClip(key);
    }

    public void ChangeClip(string key)
    {
        if (!dict.ContainsKey(key))
        {
            Debug.Log($"Animation key {key} not found.");
            return;
        }

        var anim = dict[key];
        var index = random.Next(anim.Value.Count());
        aoc[key] = anim.Value[index];
        CurrentKey = key;
    }

    public void ChangeNextStateClip(string key1, string key2)
    {
        if(CurrentKey == key1)
        {
            ChangeClip(key2);
        }
        else if(CurrentKey == key2)
        {
            ChangeClip(key1);
        }
    }
}