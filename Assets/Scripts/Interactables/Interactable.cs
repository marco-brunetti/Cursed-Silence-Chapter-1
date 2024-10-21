using System;
using System.Collections.Generic;
using UnityEngine;
using Interactables.Behaviours;
using Player;

public class Interactable : MonoBehaviour
{
    public Vector3 InspectableInitialRotation;
    public Vector3 InspectablePosition;
    public bool RotateX;
    public bool RotateY;
    public bool FreezePlayerRotation;
    public bool FreezePlayerMotion;

    public bool NonInspectable { get; private set; }
    public bool InspectableOnly { get; private set; }
    private List<IBehaviour> InteractionBehaviours = new();
    private List<IBehaviour> InspectionBehaviours = new();

    public bool DeactivateBehaviours;

    [NonSerialized] public List<GameObject> RequiredInventoryItems = new();

    private void Awake()
    {
        SetupInteractable();
    }

    private void SetupInteractable()
    {
        var behavioursInObject = gameObject.GetComponents<IBehaviour>();

        foreach (IBehaviour behaviour in behavioursInObject)
        {
            if (behaviour != null)
            {
                if(behaviour.GetType() is IRequireInventoryItem)
                {
                    var requireItem = behaviour.GetType() as IRequireInventoryItem;
                    if (requireItem.RequiredObjects != null) RequiredInventoryItems.AddRange(requireItem.RequiredObjects);
                }
                
                if (behaviour.IsInteractable()) InteractionBehaviours.Add(behaviour);
                if (behaviour.IsInspectable()) InspectionBehaviours.Add(behaviour);

                if (!behaviour.IsInteractable() && !behaviour.IsInspectable()) InteractionBehaviours.Add(behaviour);
            }
        }

        foreach (Transform child in transform)
        {
            var behaviour = child.GetComponent<IBehaviour>();

            if (behaviour != null)
            {
                if (behaviour.IsInteractable()) InteractionBehaviours.Add(behaviour);
                if (behaviour.IsInspectable()) InspectionBehaviours.Add(behaviour);

                if (!behaviour.IsInteractable() && !behaviour.IsInspectable()) InteractionBehaviours.Add(behaviour);
            }
        }

        if (InteractionBehaviours.Count == 0) InspectableOnly = true;
        if (!InspectableOnly && InspectionBehaviours.Count == 0) NonInspectable = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Interact(PlayerController playerController, bool isInteracting, bool isInspecting)
    {
        if (!InspectableOnly && isInteracting)
        {
            ManageInteractionBehaviours(isInteracting);
        }
        else if (!NonInspectable && isInspecting)
        {
            ManageInspectionBehaviours(isInspecting);
        }
        else if (!isInteracting && !isInspecting)
        {
            ManageInteractionBehaviours(isInteracting);
            ManageInspectionBehaviours(isInspecting);
        }

        if(playerController)
        {
            //Remember to unfreeze player in behaviour components
            if (FreezePlayerMotion) playerController.FreezePlayerMovement = true;
            if (FreezePlayerRotation) playerController.FreezePlayerRotation = true;
        }

        if(DeactivateBehaviours) GetComponent<Collider>().enabled = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ManageInteractionBehaviours(bool isInteracting)
    {
        if (InteractionBehaviours.Count > 0)
        {
            for (int i = 0; i < InteractionBehaviours.Count; i++)
            {
                var behaviour = InteractionBehaviours[i];
                if (behaviour.gameObject.activeInHierarchy) behaviour.Behaviour(isInteracting, false);
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ManageInspectionBehaviours(bool isInspecting)
    {
        if (InspectionBehaviours.Count > 0)
        {
            for (int i = 0; i < InspectionBehaviours.Count; i++)
            {
                var behaviour = InspectionBehaviours[i];
                if (behaviour.gameObject.activeInHierarchy) behaviour.Behaviour(false, isInspecting);
            }
        }
    }

    public bool[] RotateXY()
    {
        bool[] rotateXY = new bool[] { RotateX, RotateY };
        return rotateXY;
    }
}