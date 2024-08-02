using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Player Movement")]
    public float WalkSpeed = 3f;
    public float RunSpeed = 5f;
    public float Gravity = -19.62f;
    public float SprintStaminaDuration = 2f;

    [Header("Player Rotation and Look")]
    public float MouseSensitivityX = 0.05f;
    public float MouseSensitivityY = 0.05f;
    public int DreamLevelFOV = 50;
    public int HouseLevelFOV = 45;

    [Header("Player Audio Clips")]
    public AudioClip[] ConcreteFootstepClips;
    public float ConcreteFootstepClipsVolume = 0.2f;
    public AudioClip[] WoodFootstepClips;
    public float WoodFootstepClipsVolume = 0.5f;
    public AudioClip[] GrassFootstepClips;
    public float GrassFootstepClipsVolume = 0.2f;
    public AudioClip[] PlayerHeartbeatClips;
    public float PlayerHeartbeatClipsVolume = 0.05f;
    public AudioClip[] PlayerBreathClips;
    public float PlayerBreathClipsVolume = 1f;

    [Header("Player Audio Timing")]
    public float FootstepWalkingTime = 0.8f;
    public float FootstepsRunningTime = 0.5f;
    public float HeartbeatMinimumRate = 0.4f;
    public float BreathingMinimumRate = 2f;

    [Header("Player Stress")]
    public float MinStressLevel = 1;
    public float MaxStressLevel = 2;
    public float StressDisipateSpeed = 0.1f;

    [Header("Interactables")]
    public float InteractDistance = 2f;
    public LayerMask InteractLayer;
    public AudioClip InspectablePickupClip;
    public AudioClip InspectableReturnClip;
}