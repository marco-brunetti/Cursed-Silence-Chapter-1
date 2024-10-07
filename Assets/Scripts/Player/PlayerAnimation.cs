using Player;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;
    private new AnimationManager animation;
    private PlayerData data;

    private readonly KeyValuePair<string, int> AnimAttack = new("attack", Animator.StringToHash("attack"));
    private readonly KeyValuePair<string, int> AnimHeavyAttack = new("heavy_attack", Animator.StringToHash("heavy_attack"));
    private readonly KeyValuePair<string, int> AnimBlock = new("block", Animator.StringToHash("block"));
    private readonly KeyValuePair<string, int> AnimGrab = new("grab", Animator.StringToHash("grab"));
    private readonly KeyValuePair<string, int> AnimRelease = new("release", Animator.StringToHash("release"));

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (!controller) controller = PlayerController.Instance;
        data = controller.PlayerData;

        KeyValuePair<string, int>[] animationKeys =
        {
            AnimAttack,
            AnimHeavyAttack,
            AnimBlock
        };

        animation = new(animationKeys, animator, data.AnimatorController, data.AnimationClips);
    }

    public void Attack()
    {
        animation.EnableKey(AnimAttack);
    }

    public void HeavyAttack()
    {
        animation.EnableKey(AnimHeavyAttack);
    }

    public void Block()
    {
        animation.EnableKey(AnimBlock);
    }

    public void Grab()
    {
        animation.EnableKey(AnimGrab);
    }

    public void Release()
    {
        animation.EnableKey(AnimRelease);
    }
}