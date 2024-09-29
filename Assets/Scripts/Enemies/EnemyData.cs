using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 2)]
public class EnemyData : ScriptableObject
{
    public int Health = 100;

    [field: SerializeField, Header("Animations")] public AnimationClip[] AnimationClips {  get; private set; }
    [field: SerializeField] public AnimatorController AnimatorController { get; private set; }
}