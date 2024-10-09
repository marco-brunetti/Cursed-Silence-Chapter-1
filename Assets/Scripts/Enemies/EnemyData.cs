using UnityEngine;

namespace Enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 2)]
    public class EnemyData : ScriptableObject
    {

        [field: SerializeField, Header("Resistance"), Space(-5)] public int Health { get; private set; } = 100;
        [field: SerializeField, Range(0, 100)] public int Poise { get; private set; } = 50;
        
        [field: SerializeField, Range(0, 50)] public int BlockProbability { get; private set; } = 20;
    
        [field: SerializeField, Header("Attack stats"), Range(0, 50)] public int LightAttack { get; private set; } = 10;
        [field: SerializeField, Range(0, 50)] public int LightPoiseDecrement { get; private set; } = 10;
        
        [field: SerializeField, Range(0, 50)] public int HeavyAttack { get; private set; } = 20;
        [field: SerializeField, Range(0, 50)] public int HeavyAttackProbability { get; private set; } = 20;
        [field: SerializeField, Range(0, 50)] public int HeavyPoiseDecrement { get; private set; } = 20;

        
        [field: SerializeField, Range(0, 50)] public int SpecialAttack { get; private set; } = 20;
        [field: SerializeField, Range(0, 50)] public int SpecialAttackProbability { get; private set; } = 20;
        [field: SerializeField, Range(0, 50)] public int SpecialPoiseDecrement { get; private set; } = 20;
        
        
        
        [field: SerializeField] public LayerMask DetectionMask { get; private set; }

        [field: SerializeField, Header("Animations"), Space(10)]
        public string IdleKey { get; private set; } = "idle";
        [field: SerializeField] public string WalkKey { get; private set; } = "walk";
        [field: SerializeField] public string AttackKey { get; private set; } = "attack";
        [field: SerializeField] public string HeavyAttackKey { get; private set; } = "";
        [field: SerializeField] public string SpecialAttackKey { get; private set; } = "";
        [field: SerializeField] public string ReactKey { get; private set; } = "";
        [field: SerializeField] public string BlockKey { get; private set; } = "";
        [field: SerializeField] public string DeathKey { get; private set; } = "die";
        
        
        [field: SerializeField] public string[] AnimationKeys { get; private set; }
        [field: SerializeField] public AnimationClip[] AnimationClips {  get; private set; }
        [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
    }
}