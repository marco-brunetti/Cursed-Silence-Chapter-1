using Enemies;
using SnowHorse.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAnimationGeneric : MonoBehaviour
{
    private bool canChangeAttackAnimation = true;
    private float reactMoveSpeed;
    private float lookLerpTime;
    private Vector3 lookPos;
    private Coroutine lookAtPlayer;
    private Coroutine moveTowards;
    private Coroutine attack;
    private Animator animator;
    private EnemyController controller;
    private EnemyData data;
    private Transform player;
    private new AnimationManager animation;
    private System.Random random;

    private Dictionary<string, KeyValuePair<string,int>> animKeys = new();

    [NonSerialized] public float WalkSpeed;

    public void Init(EnemyController controller, EnemyData enemyData, Transform player)
    {
        random = new System.Random(Guid.NewGuid().GetHashCode());
        animator = GetComponent<Animator>();
        this.controller = controller;
        data = enemyData;
        this.player = player;

        SetAnimationKeys(data.AnimationKeys);
        animation = new(animKeys.Values.ToArray(), animator, animatorController: data.AnimatorController, data.AnimationClips);
    }

    public void SetAnimationKeys(string[] keys)
    {
        foreach (var key in keys)
        {
            animKeys.Add(key, new KeyValuePair<string, int>(key, Animator.StringToHash(key)));
        }
    }

    #region Animation Events
    public void SetVulnerable(string flag) => controller.IsVulnerable(flag.ToLower() == "true");
    public void ChangeNextAttackClip() => canChangeAttackAnimation = true;

    public void ReactStart(float speed) { reactMoveSpeed = speed; StartCoroutine(ReactMoveTimer()); }
    public void ReactStopMovement()
    {
        reactMoveSpeed = 0;
    }
    public void ReactStop()
    {
        reactMoveSpeed = 0;
        controller.ReactStop();
    }

    public void BlockStop() => controller.ReactStop();

    public void WalkStarted(float walkSpeed)
    {
        if (animation.CurrentKey == animKeys["walk_forward"].Value && moveTowards == null)
        {
            WalkSpeed = walkSpeed;
            MoveTowardsPlayer(true, WalkSpeed);
        }
    }

    //public void HeavyAttack => //Apply heavy attack;
    //public void Attack => //Apply attack;
    #endregion

    public void Idle(string key = "idle", bool lookAtPlayer = false)
    {
        animation.Enable(animKeys[key], deactivateOtherKeys: true);
        LookAtPlayer(lookAtPlayer);
        MoveTowardsPlayer(false);
    }

    public void Die(string key = "die", bool lookAtPlayer = false)
    {
        animation.Enable(animKeys[key], deactivateOtherKeys: true);
        LookAtPlayer(lookAtPlayer);
        MoveTowardsPlayer(false);
    }

    public void React(string key = "react", bool lookAtPlayer = false)
    {
        animation.Enable(animKeys[key], deactivateOtherKeys: true);
        LookAtPlayer(lookAtPlayer);
        MoveTowardsPlayer(false);
    }

    public void Block(string key = "block", bool lookAtPlayer = false)
    {
        animation.Enable(animKeys[key], deactivateOtherKeys: true);
        LookAtPlayer(lookAtPlayer);
        MoveTowardsPlayer(false);
    }

    public void Attack(string key = "attack", bool lookAtPlayer = false)
    {
        attack ??= StartCoroutine(AttackingPlayer());

        LookAtPlayer(lookAtPlayer);
        MoveTowardsPlayer(false);
    }

    public void SpecialAttack(string key = "special_attack", bool lookAtPlayer = false)
    {

    }

    public void Walk(string key = "walk", bool lookAtPlayer = false)
    {
        animation.Enable(animKeys[key], deactivateOtherKeys: true);
        LookAtPlayer(lookAtPlayer);
    }

    private void MoveTowardsPlayer(bool isMoving, float speed = 0)
    {
        if (moveTowards != null)
        {
            StopCoroutine(moveTowards);
            moveTowards = null;
        }
        if (isMoving)
        {
            moveTowards = StartCoroutine(MovingTowardsPlayer(speed));
        }
    }

    private IEnumerator AttackingPlayer()
    {
        if (animation.CurrentKey != AnimAttack.Value && animation.CurrentKey != AnimHeavyAttack.Value)
        {
            animation.EnableKey(AnimAttack, deactivateOtherKeys: true);
            yield return null;
        }

        while (animation.CurrentKey == animKeys["attack"].Value || animation.CurrentKey == animKeys["heavy_attack"].Value)
        {
            if (canChangeAttackAnimation)
            {
                animation.ChangeNextState(key1:animKeys["attack"], key2:animKeys["heavy_attack"], key2Probability:data.HeavyAttackProbability);
                canChangeAttackAnimation = false;
            }

            yield return null;
        }

        attack = null;
    }

    private IEnumerator MovingTowardsPlayer(float speed)
    {
        while (true)
        {
            var targetPosition = new Vector3(player.position.x, controller.transform.position.y, player.position.z);
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void LookAtPlayer(bool isLooking, float duration = 50)
    {
        if (lookAtPlayer != null) StopCoroutine(lookAtPlayer);
        if (isLooking) lookAtPlayer = StartCoroutine(LookingAtPlayer(duration));
    }

    private IEnumerator LookingAtPlayer(float duration, float correctionAngle = 0)
    {
        lookLerpTime = 0;
        lookPos = controller.transform.position + controller.transform.forward;

        while (true)
        {
            var target = GetTargetDirOnYAxis(origin: controller.transform.position, target: player.position);

            if ((target - lookPos).magnitude < 0.01f) lookLerpTime = 0;
            else lookPos = Vector3.Slerp(lookPos, target, Interpolation.Linear(duration, ref lookLerpTime));

            controller.transform.LookAt(GetTargetDirOnYAxis(origin: controller.transform.position, target: lookPos));
            yield return null;
        }
    }

    //Get direction with correct vector length
    private Vector3 GetTargetDirOnYAxis(Vector3 origin, Vector3 target, bool debug = false, Color? color = null)
    {
        var finalPos = origin + (new Vector3(target.x, origin.y, target.z) - origin).normalized;

        if (debug && color != null) Debug.DrawLine(origin, finalPos, (Color)color);

        return finalPos;
    }

    private IEnumerator ReactMoveTimer()
    {
        while (reactMoveSpeed > 0)
        {
            controller.transform.position -= transform.forward * (reactMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
