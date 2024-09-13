using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LevelLayout : MonoBehaviour
{
	[field: SerializeField] public LayoutShape Shape { get; private set; }

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
	[NonSerialized] public List<LayoutDecorator> DecoratorList = new();

	private LayoutStyle style;
	private List<Vector3> initialDoorRotations = new();

	private bool areFreeAnchorsReady = false;
    private List<Transform> freeWallAnchors = new();
    private List<Transform> freeCeilingAnchors = new();
    private List<Transform> freeFloorAnchors = new();

    public void Setup(int mapIndex, LayoutStyle style, List<LayoutShape> nextLayoutShapes, bool isEndOfZone, params LevelDecorator[] decorators)
	{
		this.style = style;
		MapIndex = mapIndex;
		SetDoorActions(nextLayoutShapes, isEndOfZone);
        GetMaterials();
		SetLighting();
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

    private void SetDoorActions(List<LayoutShape> nextLayoutShapes, bool isEndOfZone)
	{
		entranceDoor.SetActive(false);
		if (doors == null || doors.Count == 0) return;

		for(int i = 0; i < doors.Count; i++)
		{
			initialDoorRotations.Add(doors[i].transform.localEulerAngles);

			if(nextLayoutShapes == null || nextLayoutShapes.Count == 0 || i >= nextLayoutShapes.Count || (nextLayoutShapes[i] == LayoutShape.None))
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

    private void SetLighting()
    {
		Array.ForEach(style0Lights, (x)=> x.gameObject.SetActive(style == LayoutStyle.Style0));
		Array.ForEach(style1Lights, (x)=> x.gameObject.SetActive(style == LayoutStyle.Style1));
		Array.ForEach(style2Lights, (x)=> x.gameObject.SetActive(style == LayoutStyle.Style2));
		Array.ForEach(style3Lights, (x)=> x.gameObject.SetActive(style == LayoutStyle.Style3));
		Array.ForEach(style4Lights, (x)=> x.gameObject.SetActive(style == LayoutStyle.Style4));
    }

    private void GetMaterials()
	{
		Material upperMat = null;
		Material lowerMat = null;
        Material ceilingMat = null;
        Material floorMat = null;
        Material windowDecorMat1 = null;
        Material windowDecorMat2 = null;

        switch (style)
		{
			default:
			case LayoutStyle.Style0:
				upperMat = layoutData.WallMat1;
				lowerMat = layoutData.LowerWallMat1;
				ceilingMat = layoutData.CeilingMat1;
				floorMat = layoutData.FloorMat1;
				windowDecorMat1 = layoutData.WindowDecorMat1;
				windowDecorMat2 = layoutData.WindowDecorMat1;
				break;
            case LayoutStyle.Style1:
                upperMat = layoutData.WallMat2;
                lowerMat = layoutData.LowerWallMat2;
                ceilingMat = layoutData.CeilingMat2;
                floorMat = layoutData.FloorMat2;
                windowDecorMat1 = layoutData.WindowDecorMat2;
                windowDecorMat2 = layoutData.WindowDecorMat2;
                break;
            case LayoutStyle.Style2:
                upperMat = layoutData.WallMat3;
                lowerMat = layoutData.LowerWallMat3;
                ceilingMat = layoutData.CeilingMat3;
                floorMat = layoutData.FloorMat3;
                windowDecorMat1 = layoutData.WindowDecorMat3;
                windowDecorMat2 = layoutData.WindowDecorMat3;
                break;
            case LayoutStyle.Style3:
                upperMat = layoutData.WallMat4;
                lowerMat = layoutData.LowerWallMat4;
                ceilingMat = layoutData.CeilingMat4;
                floorMat = layoutData.FloorMat4;
                windowDecorMat1 = layoutData.WindowDecorMat4;
                windowDecorMat2 = layoutData.WindowDecorMat4;
                break;
            case LayoutStyle.Style4:
                upperMat = layoutData.WallMat5;
                lowerMat = layoutData.LowerWallMat5;
                ceilingMat = layoutData.CeilingMat5;
                floorMat = layoutData.FloorMat5;
                windowDecorMat1 = layoutData.WindowDecorMat5;
                windowDecorMat2 = layoutData.WindowDecorMat5;
                break;

        }

		SetMaterials(upperMat, lowerMat, ceilingMat, floorMat, windowDecorMat1, windowDecorMat2);
	}

	private void SetMaterials
		(Material upMat = null,
		 Material lowMat = null,
		 Material ceilingMat = null,
		 Material floorMat = null,
		 Material windowMat1 = null,
		 Material windowMat2 = null)
	{
		if(upMat || lowMat)
		{
			Array.ForEach(wallRenderers, r =>
			{
                var wallMaterials = r.materials;

                if (upMat) wallMaterials[0] = upMat;
                if (lowMat) wallMaterials[1] = lowMat;

                r.materials = wallMaterials;
            });

			foreach (var renderer in doorWallRenderers)
			{
				var doorWallMaterials = renderer.materials;

				if (upMat) doorWallMaterials[1] = upMat;
				if (lowMat) doorWallMaterials[0] = lowMat;

				renderer.materials = doorWallMaterials;
			}
		}

		if(upMat || lowMat || windowMat1 || windowMat2)
		{
			Array.ForEach(windowWallRenderers, r =>
			{
                var mats = r.materials;
                if (lowMat) mats[0] = lowMat;
                if (windowMat2) mats[1] = windowMat2;
                if (windowMat1) mats[3] = windowMat1;
                if (upMat) mats[4] = upMat;
                r.materials = mats;
            });
		}

		if (ceilingMat) Array.ForEach(ceilingRenderers, r => { var mats = r.materials; mats[0] = ceilingMat; r.materials = mats; });

		if(floorMat) Array.ForEach(floorRenderers, r => { var mats = r.materials; mats[0] = floorMat; r.materials = mats; });
	}
}