using Player;
using System.Collections.Generic;
using SnowHorse.Systems;
using UnityEngine;
using System;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;
    private new AnimationManager animation;
    private PlayerData data;

    private readonly KeyValuePair<string, int> animIdle = new("idle", Animator.StringToHash("idle"));
    private readonly KeyValuePair<string, int> animWalk = new("walk", Animator.StringToHash("walk"));
    private readonly KeyValuePair<string, int> animRun = new("run", Animator.StringToHash("run"));
    private readonly KeyValuePair<string, int> animAttack = new("attack", Animator.StringToHash("attack"));
    private readonly KeyValuePair<string, int> animHeavyAttack = new("heavy_attack", Animator.StringToHash("heavy_attack"));
    private readonly KeyValuePair<string, int> animBlock = new("block", Animator.StringToHash("block"));
    private readonly KeyValuePair<string, int> animGrab = new("grab", Animator.StringToHash("grab"));
    private readonly KeyValuePair<string, int> animRelease = new("release", Animator.StringToHash("release"));

    public static EventHandler<PlayerAudioEventArgs> Step;

    public void StepEvent()
    {
        if(PlayerController.Instance.Character.velocity.magnitude < (data.WalkSpeed / 2)) return;

        var args = new PlayerAudioEventArgs("player", data.WoodFootstepClips[UnityEngine.Random.Range(0, data.WoodFootstepClips.Length)], data.WoodFootstepClipsVolume);

        Step?.Invoke(this, args);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (!controller) controller = PlayerController.Instance;
        data = controller.PlayerData;

        KeyValuePair<string, int>[] animationKeys =
        {
            animIdle,
            animWalk,
            animRun,
            animAttack,
            animHeavyAttack,
            animBlock,
            animGrab,
            animRelease
        };

        animation = new AnimationManager(animationKeys, animator, data.AnimatorController, data.AnimationClips);
    }

    public void Idle()
    {
        animation.Enable(animIdle);
    }
    public void Walk()
    {
        animation.Enable(animWalk);
    }
    public void Run()
    {
        animation.Enable(animRun);
    }

    public void Attack()
    {
        animation.Enable(animAttack);
    }

    public void HeavyAttack()
    {
        animation.Enable(animHeavyAttack);
    }

    public void Block()
    {
        animation.Enable(animBlock);
    }

    public void Grab()
    {
        animation.Enable(animGrab);
    }

    public void Release()
    {
        animation.Enable(animRelease);
    }
}