using Cinemachine;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Player Movement")]
    public float WalkSpeed = 50f;
    public float RunSpeed = 50f;
    public float Gravity = -19.62f;

    [Header("Player Rotation and Look")]
    public float MouseSensitivityX = 0.05f;
    public float MouseSensitivityY = 0.05f;
    public int DreamLevelFOV = 50;
    public int HouseLevelFOV = 45;

    [Header("Player Cameras")]
    public GameObject CamHolder;
    public Transform Camera;
    public CinemachineVirtualCamera VirtualCamera;
    public GameObject InventoryCamera;

    [Header("Player Audio Clips")]
    public AudioClip[] ConcreteFootstepClips;
    public float ConcreteFootstepClipsVolume;
    public AudioClip[] WoodFootstepClips;
    public float WoodFootstepClipsVolume;
    public AudioClip[] GrassFootstepClips;
    public float GrassFootstepClipsVolume;
    public AudioClip[] PlayerHeartbeatClips;
    public float PlayerHeartbeatClipsVolume;
    public AudioClip[] PlayerBreathClips;
    public float PlayerBreathClipsVolume;

    [Header("Player Audio Timing")]
    public float FootstepWalkingTime;
    public float FootstepsRunningTime;
    public float HeartbeatMinimumRate;
    public float BreathingMinimumRate;

    [Header("Player Stress")]
    public float MinStressLevel = 1;
    public float MaxStressLevel = 2;
    public float StressDisipateSpeed = 0.1f;

    [Header("Interactables")]
    public float InteractDistance = 5f;
    public Transform InventoryHolder;
    public Transform InspectorParent;
    public LayerMask InteractLayer;
    public AudioClip InspectablePickupClip;
    public AudioClip InspectableReturnClip;
    public AudioSource InspectablesSource;

    public CharacterController Character;
}
