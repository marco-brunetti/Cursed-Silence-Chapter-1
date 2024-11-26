using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Interactables.Behaviours;
using Player;

public class Interactable : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Vector3 InspectableInitialRotation { get; private set; }
    [field: SerializeField] public Vector3 InspectablePosition { get; private set; }
    public bool RotateX;
    public bool RotateY;
    public bool FreezePlayerRotation;
    public bool FreezePlayerMotion;

    public bool NonInspectable { get; private set; }
    public bool InspectableOnly { get; private set; }
    public bool DeactivateBehaviours;
    public List<GameObject> RequiredInventoryItems { get; private set; } = new();

    private List<IBehaviour> behaviours;


    private void Awake()
    {
        behaviours = gameObject.gameObject.GetComponents<IBehaviour>().ToList();

        InspectableOnly = !behaviours.Any(x => x.IsInteractable());
        if (!InspectableOnly) NonInspectable = !behaviours.Any(x => x.IsInspectable());

        behaviours.Where(x => x is IRequireInventoryItem).ToList().ForEach(x => RequiredInventoryItems.AddRange(((IRequireInventoryItem)x).RequiredObjects));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Interact(PlayerController playerController, bool isInteracting, bool isInspecting)
    {
        foreach (var behaviour in behaviours)
        {
            if (!InspectableOnly && isInteracting && behaviour.IsInteractable())
            {
                behaviour.Behaviour(isInteracting:true, isInspecting:false);
            }
            else if (!NonInspectable && isInspecting && behaviour.IsInspectable())
            {
                behaviour.Behaviour(isInteracting:false, isInspecting:true);
            }
            else if (!isInteracting && !isInspecting)
            {
                behaviour.Behaviour(isInteracting:false, isInspecting:false);
            }
        }

        if(playerController)
        {
            //Remember to unfreeze player in behaviour components
            if (FreezePlayerMotion) playerController.FreezePlayerMovement = true;
            if (FreezePlayerRotation) playerController.FreezePlayerRotation = true;
        }

        if(DeactivateBehaviours) GetComponent<Collider>().enabled = false;
    }

    public bool[] RotateXY()
    {
        bool[] rotateXY = { RotateX, RotateY };
        return rotateXY;
    }
}