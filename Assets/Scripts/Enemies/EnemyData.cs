using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 2)]
    public class EnemyData : ScriptableObject
    {
        [field: SerializeField, Header("Enemy State")] public EnemyState InitialEnemyState { get; private set; }
        [field: SerializeField, Header("Resistance"), Space(10)] public int Health { get; private set; } = 100;
        [field: SerializeField, Range(0, 100)] public int Poise { get; private set; } = 50;
        [field: SerializeField] public float OnDieDisappearSpeed { get; private set; } = 1;

        [field: SerializeField, Header("Damage"), Range(0, 50), Space(10)] public int LightAttackDamage { get; private set; } = 10;
        [field: SerializeField, Range(0, 50)] public int HeavyAttackDamage { get; private set; } = 20;
        [field: SerializeField, Range(0, 50)] public int SpecialAttackDamage { get; private set; } = 50;

        [field: SerializeField, Header("Poise Decrement"), Range(0, 30), Space(10)] public int LightAttackPoiseDecrement { get; private set; } = 10;
        [field: SerializeField, Range(0, 30)] public int HeavyAttackPoiseDecrement { get; private set; } = 20;
        [field: SerializeField, Range(0, 30)] public int SpecialAttackPoiseDecrement { get; private set; } = 20;

        [field: SerializeField, Header("Probability"), Range(0, 20), Space(10)] public int HeavyAttackProbability { get; private set; } = 20;
        [field: SerializeField, Range(0, 20)] public int SpecialAttackProbability { get; private set; } = 5;
        [field: SerializeField, Range(0, 50)] public int BlockProbability { get; private set; } = 20;

        [field: SerializeField, Header("Player Tracking"), Space(10)] public LayerMask DetectionMask { get; private set; }
        [field: SerializeField, Range(0.1f, 10)] public float DetectionInterval { get; private set; } = 1;
        [field: SerializeField] public float MaxAttackDistance { get; private set; }
        [field: SerializeField] public float MaxAwareDistance { get; private set; }

        [field: SerializeField, Header("NavMesh"), Space(10), Range(0.3f, 5)] public float PathFindInterval { get; private set; } = 1;
        [field: SerializeField, Tooltip("Use for erratic movement")] public bool RandomizePath { get; private set; } = false;
        [field: SerializeField, Range(0, 5), Tooltip("Use for erratic movement")] public float RandomPathRange { get; private set; } = 0;
        
        [field: SerializeField, Header("Animations"), Space(10)]
        public RuntimeAnimatorController AnimatorController { get; private set; }
        [field: SerializeField] public AnimationClip IdleAnim { get; private set; }
        [field: SerializeField] public AnimationClip MoveAnim { get; private set; }
        [field: SerializeField] public AnimationClip AttackAnim { get; private set; }
        [field: SerializeField] public AnimationClip HeavyAttackAnim { get; private set; }
        [field: SerializeField] public AnimationClip SpecialAttackAnim { get; private set; }
        [field: SerializeField] public AnimationClip ReactAnim { get; private set; }
        [field: SerializeField] public AnimationClip BlockAnim { get; private set; }
        [field: SerializeField] public AnimationClip DeathAnim { get; private set; }
        [field: SerializeField, Tooltip("Add the base animation for any custom state with a custom bool")] public AnimationClip[] AdditionalStateAnims { get; private set; }
        [field: SerializeField] public AnimationClip[] AlternativeClips {  get; private set; }


        [field: SerializeField, Header("Custom colors"), Space(10)] public Color[] Colors { get; private set; }
    }
}