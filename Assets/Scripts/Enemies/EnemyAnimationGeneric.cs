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
    private Animator animator;
    private EnemyController controller;
    private EnemyData data;
    private new AnimationManager animation;
    private System.Random random;

    private Dictionary<string, KeyValuePair<string,int>> animKeys = new();

    [NonSerialized] public float WalkSpeed;
    public string CurrentKey => animation.CurrentKeyString;
    public void Init(EnemyController controller, EnemyData enemyData)
    {
        random = new System.Random(Guid.NewGuid().GetHashCode());
        animator = GetComponent<Animator>();
        this.controller = controller;
        data = enemyData;

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
        }
    }

    //public void HeavyAttack => //Apply heavy attack;
    //public void Attack => //Apply attack;
    #endregion

    public void SetState(string animKey, Transform lookTarget = null, Transform moveTarget = null, float moveSpeed = 0)
    {
        if (lookTarget) LookAtTarget(lookTarget);
        else StopLooking();

        if (moveTarget)
        {
            if(animKey.Contains("walk")) MoveTowardsTarget(moveTarget, WalkSpeed);
            else MoveTowardsTarget(moveTarget, moveSpeed);
        }
        else
        {
            StopMoving(); 
        }
        
        animation.Enable(animKeys[animKey]);
    }

    private void MoveTowardsTarget(Transform targetTransform, float speed)
    {
        StopMoving();
        moveTowards = StartCoroutine(MovingTowardsTarget(targetTransform, speed));
    }

    private IEnumerator MovingTowardsTarget(Transform targetTransform, float speed)
    {
        while (true)
        {
            var targetPosition = new Vector3(targetTransform.position.x, controller.transform.position.y, targetTransform.position.z);
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void StopMoving()
    {
        if (moveTowards != null)
        {
            StopCoroutine(moveTowards);
            moveTowards = null;
            if (CurrentKey.Contains("walk")) WalkSpeed = 0;
        }
    }

    private void LookAtTarget(Transform targetTransform, float duration = 50)
    {
        StopLooking();
        lookAtPlayer = StartCoroutine(LookingAtTarget(targetTransform, duration));
    }

    private IEnumerator LookingAtTarget(Transform targetTransform, float duration)
    {
        lookLerpTime = 0;
        lookPos = controller.transform.position + controller.transform.forward;

        while (true)
        {
            var target = GetTargetDirOnYAxis(origin: controller.transform.position, target: targetTransform.position);

            if ((target - lookPos).magnitude < 0.01f) lookLerpTime = 0;
            else lookPos = Vector3.Slerp(lookPos, target, Interpolation.Linear(duration, ref lookLerpTime));

            controller.transform.LookAt(GetTargetDirOnYAxis(origin: controller.transform.position, target: lookPos));
            yield return null;
        }
    }

    private void StopLooking()
    {
        if (lookAtPlayer != null) StopCoroutine(lookAtPlayer);
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
