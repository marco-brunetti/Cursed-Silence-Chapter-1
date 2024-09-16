using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LevelLayout : MonoBehaviour
{
	[field: SerializeField] public LayoutType Shape { get; private set; }

    [field: SerializeField] public List<Vector3> NextLayoutOffsets { get; private set; }
    [field: SerializeField] public List<Vector3> NextLayoutRotations { get; private set; }

	[SerializeField] private GameObject entranceDoor;
	[SerializeField] private List<Behaviour_DoorNew> doors;
	[SerializeField] private LayoutData layoutData;

	[SerializeField] private Transform[] wallAnchors;
	[SerializeField] private Transform[] ceilingAnchors;
	[SerializeField] private Transform[] floorAnchors;

	[Header("Style")]
	[SerializeField] private MeshRenderer[] wallRenderers;
	[SerializeField] private MeshRenderer[] doorWallRenderers;
	[SerializeField] private MeshRenderer[] windowWallRenderers;
	[SerializeField] private MeshRenderer[] ceilingRenderers;
	[SerializeField] private MeshRenderer[] floorRenderers;

	[Header("Lighting")]
	[SerializeField] private LayoutLight[] style0Lights;
	[SerializeField] private LayoutLight[] style1Lights;
	[SerializeField] private LayoutLight[] style2Lights;
	[SerializeField] private LayoutLight[] style3Lights;
	[SerializeField] private LayoutLight[] style4Lights;

	[NonSerialized] public int MapIndex = -1;
	[NonSerialized] public List<LayoutDecorator> ItemList = new();

	private List<Vector3> initialDoorRotations = new();

	private bool areFreeAnchorsReady = false;
    private List<Transform> freeWallAnchors = new();
    private List<Transform> freeCeilingAnchors = new();
    private List<Transform> freeFloorAnchors = new();

    public void Setup(int mapIndex, List<LayoutType> nextLayoutShapes, bool isEndOfZone, params Leveltem[] decorators)
	{
		MapIndex = mapIndex;
		SetDoorActions(nextLayoutShapes, isEndOfZone);
		FindFreeAnchors();
    }

	public bool HasDoors()
	{
		return doors != null && doors.Count > 0;
	}

    public void GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors)
    {	
		wallAnchors = new(freeWallAnchors);
		ceilingAnchors = new(freeCeilingAnchors);
		floorAnchors = new(freeFloorAnchors);
    }

    public void EntranceDoorEnabled(bool enabled)
	{
		entranceDoor.SetActive(enabled);
	}

    private void FindFreeAnchors()
    {
		if (areFreeAnchorsReady) return;

        Array.ForEach(this.wallAnchors, anchor => {
            var isAnchorFree = anchor.Cast<Transform>().All(child => !child.gameObject.activeInHierarchy);
            if (isAnchorFree) freeWallAnchors.Add(anchor);
        });

        Array.ForEach(this.ceilingAnchors, anchor => {
            var isAnchorFree = anchor.Cast<Transform>().All(child => !child.gameObject.activeInHierarchy);
            if (isAnchorFree) freeCeilingAnchors.Add(anchor);
        });

        Array.ForEach(this.floorAnchors, anchor => {
            var isAnchorFree = anchor.Cast<Transform>().All(child => !child.gameObject.activeInHierarchy);
            if (isAnchorFree) freeFloorAnchors.Add(anchor);
        });

		areFreeAnchorsReady = true;
    }

    private void SetDoorActions(List<LayoutType> nextLayoutShapes, bool isEndOfZone)
	{
		entranceDoor.SetActive(false);
		if (doors == null || doors.Count == 0) return;

		for(int i = 0; i < doors.Count; i++)
		{
			initialDoorRotations.Add(doors[i].transform.localEulerAngles);

			if(nextLayoutShapes == null || nextLayoutShapes.Count == 0 || i >= nextLayoutShapes.Count || (nextLayoutShapes[i] == LayoutType.None))
			{
                doors[i].SetDoorState(DoorState.Locked);
                continue;
            }

            if (i < nextLayoutShapes.Count)
            {
                var nextShape = nextLayoutShapes[i];
                var offset = NextLayoutOffsets[i];
                var rotation = Quaternion.Euler(NextLayoutRotations[i]);
                UnityAction action = () => LayoutManager.Instance.ActivateLayout(previousLayout: transform, nextShape, offset, rotation, null);

				//If end of zone, start deactivation process of previous zone
				if (isEndOfZone && i == nextLayoutShapes.Count - 1)
				{
					var manager = LayoutManager.Instance;
					manager.StartCoroutine(manager.DeactivateLevelLayouts());
				}

				doors[i].SetDoorState(DoorState.Closed);
                doors[i].SetDoorAction(action);
            }
        }
    }

	private void OnDisable()
	{
		for(int i = 0; i < doors.Count; i++)
		{
			doors[i].transform.localEulerAngles = initialDoorRotations[i];
		}

		initialDoorRotations.Clear();
	}
}