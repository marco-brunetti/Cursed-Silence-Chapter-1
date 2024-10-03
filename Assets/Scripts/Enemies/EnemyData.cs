using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 2)]
public class EnemyData : ScriptableObject
{
    [field: SerializeField, Header("Combat")] public int Health { get; private set; } = 100;
    [field: SerializeField] public int Poise { get; private set; } = 100;
    [field: SerializeField] public int LightAttack { get; private set; } = 10;
    [field: SerializeField] public int LightAttackPoiseDecrement { get; private set; } = 10;
    [field: SerializeField] public int HeavyAttack { get; private set; } = 20;
    [field: SerializeField] public int HeavyAttackPoiseDecrement { get; private set; } = 20;

    [field: SerializeField, Header("Animations")] public AnimationClip[] AnimationClips {  get; private set; }
    [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
}