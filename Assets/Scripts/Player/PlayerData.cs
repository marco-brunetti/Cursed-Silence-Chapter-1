using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Player Movement")] public float WalkSpeed = 3f;
    public float RunSpeed = 5f;
    public float Gravity = -19.62f;
    public float SprintStaminaDuration = 2f;

    [Header("Player Rotation and Look")] public float MouseSensitivityX = 0.05f;
    public float MouseSensitivityY = 0.05f;
    public int DreamLevelFOV = 50;
    public int HouseLevelFOV = 45;
    public float DefaultThickDistortion = 0.2f;
    public float DefaultFineDistortion = 0.2f;
    public float MaxthickDistortion = 1f;
    public float MaxFineDistortion = 5f;
    public float defaultDepthOfField = 90;

    [Header("Player Audio Clips")] public AudioClip[] ConcreteFootstepClips;
    public float ConcreteFootstepClipsVolume = 0.2f;
    public AudioClip[] WoodFootstepClips;
    public float WoodFootstepClipsVolume = 0.5f;
    public AudioClip[] GrassFootstepClips;
    public float GrassFootstepClipsVolume = 0.2f;
    public AudioClip[] PlayerHeartbeatClips;
    public float PlayerHeartbeatClipsVolume = 0.05f;
    public AudioClip[] PlayerBreathClips;
    public float PlayerBreathClipsVolume = 1f;

    [Header("Player Audio Timing")] public float FootstepWalkingTime = 0.8f;
    public float FootstepsRunningTime = 0.5f;
    public float HeartbeatMinimumRate = 0.4f;
    public float BreathingMinimumRate = 2f;

    [Header("Player Stress")] public float MinStressLevel = 1;
    public float MaxStressLevel = 2;
    public float StressDisipateSpeed = 0.1f;

    [Header("Player Combat")] public float AttackDistance {get; private set; } = 1.5f;
    [field: SerializeField] public int Health { get; private set; } = 100;
    [field: SerializeField, Range(0, 100)] public int Poise { get; private set; } = 50;
    [field: SerializeField, Range(0, 50)] public int LightAttackDamage { get; private set; } = 10;
    [field: SerializeField, Range(0, 50)] public float LightAttackMaxTime { get; private set; } = 0.3f;
    [field: SerializeField, Range(0, 50)] public int LightAttackPoiseDecrement { get; private set; } = 10;
    [field: SerializeField, Range(0, 50)] public int HeavyAttackDamage { get; private set; } = 20;
    [field: SerializeField, Range(0, 50)] public float HeavyAttackLoadTime{ get; private set; } = 2.5f;
    [field: SerializeField, Range(0, 50)] public int HeavyAttackPoiseDecrement { get; private set; } = 20;

    [Header("Interactables")] public float InteractDistance = 2f;
    public LayerMask InteractLayer;
    public AudioClip InspectablePickupClip;
    public AudioClip InspectableReturnClip;
}