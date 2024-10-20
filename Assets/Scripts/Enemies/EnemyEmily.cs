using SnowHorse.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyEmily : Enemy
    {
        [SerializeField] private AnimationClip SpecialAttack2Clip;

        [SerializeField] private List<Renderer> skinRenderers;
        [SerializeField] private Renderer clothesRenderer;
        [SerializeField] private Renderer hairRenderer;

        private bool isTrackingStopped;
        private bool currentVulnerable;
        private bool justExitedAttackState;
        private float specialAttackLerpTime;
        private float specialAttackLerpTime2;
        private float defaultLookSpeed = 5;
        private float colorChangeDuration = 0.15f;
        private float colorLerpTime;
        private float skinColorLerpTime;
        private Color skinImmortalColor;
        private Color skinMortalColor;
        private Color emissionColor;
        private Color transparentColor;
        private Color defaultClothesColor;
        private Color defaultHairColor;

        private Color currentSkinColor;

        protected override void Awake()
        {
            base.Awake();

            skinImmortalColor = data.Colors[0];
            skinMortalColor = data.Colors[1];
            emissionColor = data.Colors[2];

            transparentColor = data.Colors[3];
            defaultHairColor = data.Colors[4];
            defaultClothesColor = data.Colors[5];
        }

        protected override void Start()
        {
            base.Start();
            animation.AddStateAnimations(EnemyState.Attack, SpecialAttack2Clip.name);
            skinRenderers.ForEach(x => { var mat = x.material; mat.color = isVulnerable ? skinMortalColor : skinImmortalColor; mat.SetVector("_EmissionColor", emissionColor); x.material = mat; });
            currentVulnerable = isVulnerable;
            skinColorLerpTime = colorChangeDuration;
        }
        
        protected override void Move()
        {
            animation.SetState(data.MoveAnim.name, currentState, lookTarget:player, rootTransformForLook: transform);
            animation.SetLookSpeed(defaultLookSpeed);
        }

        protected override void OnWalkStarted(float speed)
        {
            //TODO: Add any other state that contains walk clip
            if (currentState == EnemyState.Walk) 
            {
                animation.SetState(data.MoveAnim.name, currentState, moveTarget: player);
                animation.SetAgentSpeed(speed);
            }
        }

        protected override void Attack()
        {
            var attackKeysList = new List<string> { data.AttackAnim.name };
            if (hasHeavyAttack) attackKeysList.Add(data.HeavyAttackAnim.name);
            if (hasSpecialAttack) attackKeysList.AddRange(new List<string> { data.SpecialAttackAnim.name, SpecialAttack2Clip.name });

            attack ??= StartCoroutine(AttackingPlayer(attackKeysList));
        }

        protected override void SetRandomAttack()
        {
            if (currentState == EnemyState.Attack)
            {
                if (animation.CurrentKey == data.AttackAnim.name)
                {
                    var p = random.Next(0, 100);

                    if (p < data.SpecialAttackProbability) SelectSpecialAttack();
                    else if (p < data.HeavyAttackProbability + data.SpecialAttackProbability) animation.SetState(data.HeavyAttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                }
                else
                {
                    RestartTracking();
                    if(currentState == EnemyState.Attack) animation.SetState(data.AttackAnim.name, currentState, rootTransformForLook: transform, lookTarget: player);
                } 
            }
            else
            {
                RestartTracking();
            }

            changeNextAttack = false;
        }

        //Added second special attack for Emily
        private void SelectSpecialAttack()
        {
            StopMovementFunctions();

            var sp = random.Next(0, 100);
            if (sp < 50) animation.SetState(data.SpecialAttackAnim.name, currentState);
            else animation.SetState(SpecialAttack2Clip.name, currentState);
        }

        private void StopMovementFunctions()
        {
            StopPlayerTracking();
            animation.StopNavigation(stopAgentCompletely:true);
            animation.SetLookSpeed(0);
            isTrackingStopped = true;
        }

        private void RestartTracking()
        {
            if (isTrackingStopped)
            {
                StartPlayerTracking(visualConeOnly: false);
                isTrackingStopped = false;
            }
        }

        protected override void OnAnimationEvent(object sender, AnimationEventArgs args)
        {
            if ((EnemyAnimation)sender != animation) return;
            base.OnAnimationEvent(sender, args);
            if (args.Event == "started_special_attack_movement") StartCoroutine(SpecialAttackMovement());
        }

        //Add custom movement for special attack animation
        private IEnumerator SpecialAttackMovement()
        {
            var initialPosition = transform.position;
            var target = transform.position + transform.forward * 7f;

            //Getting this numbers manually from the animation clip
            var runAttackClipSec = 0.67f;
            var AttackWaitSec = 1.5f;
            var returnClipSec = 2.33f;

            //Total attack time
            var currentTime = runAttackClipSec + AttackWaitSec + returnClipSec;

            while(currentTime > 0)
            { 
                //Go and come back
                if(currentTime > AttackWaitSec + returnClipSec) transform.position = Vector3.Lerp(initialPosition, target, Interpolation.Linear(runAttackClipSec, ref specialAttackLerpTime));
                else if(currentTime < returnClipSec) transform.position = Vector3.Lerp(target, initialPosition, Interpolation.Smooth(returnClipSec, ref specialAttackLerpTime2));

                currentTime -= Time.deltaTime;
                yield return null;
            }

            //When the model sets foot on the ground, there is a half second delay before clip ends
            yield return new WaitForSecondsRealtime(0.5f); 

            specialAttackLerpTime = 0;
            specialAttackLerpTime2 = 0;
        }

        private void Update()
        {
            SetColors();
        }

        

        private void SetColors()
        {
            Color clothesColor;
            Color hairColor;
            Color skinColor;

            if(currentState != EnemyState.Attack && justExitedAttackState)
            {
                currentSkinColor = skinRenderers[0].material.color;
                colorLerpTime = 0;
                justExitedAttackState = false;
            }
            else if(currentState == EnemyState.Attack && !justExitedAttackState)
            {
                currentSkinColor = skinImmortalColor;
                colorLerpTime = 0;
                skinColorLerpTime = colorChangeDuration;
                justExitedAttackState = true;
            }

            var percent = currentState == EnemyState.Attack ? Interpolation.Linear(colorChangeDuration, ref colorLerpTime)
                                                            : Interpolation.InverseLinear(colorChangeDuration, ref colorLerpTime);

            
            clothesColor = Color.Lerp(transparentColor, defaultClothesColor, percent);
            hairColor = Color.Lerp(transparentColor, defaultHairColor, percent);

            SetMaterial(hairRenderer, hairColor, percent);
            SetMaterial(clothesRenderer, clothesColor, percent);

            if(currentState != EnemyState.Attack || currentState == EnemyState.Attack && percent < 1)
            {
                skinColor = Color.Lerp(transparentColor, currentSkinColor, percent);
                skinRenderers.ForEach(x => { var mat = x.material; mat.color = skinColor; x.material = mat; });
            }

            SetSkinColor();
        }

        private void SetMaterial(Renderer renderer, Color color, float lerpPercent)
        {
            var mat = renderer.material;
            if(lerpPercent == 1) MaterialExtensions.ToOpaqueMode(mat);
            else MaterialExtensions.ToFadeMode(mat);
            mat.color = color;
            hairRenderer.material = mat;
        }


        private void SetSkinColor()
        {
            if(currentState == EnemyState.Attack)
            {
                if (currentVulnerable != isVulnerable) skinColorLerpTime = colorChangeDuration - skinColorLerpTime;
                if (Mathf.Approximately(skinColorLerpTime, colorChangeDuration)) return;

                var percent = isVulnerable ? Interpolation.Linear(colorChangeDuration, ref skinColorLerpTime) : Interpolation.InverseLinear(colorChangeDuration, ref skinColorLerpTime);
                currentSkinColor = Color.Lerp(skinImmortalColor, skinMortalColor, percent);
                skinRenderers.ForEach(x => { var mat = x.material; mat.color = currentSkinColor; x.material = mat; });
            }

            currentVulnerable = isVulnerable;
        }
    }
}