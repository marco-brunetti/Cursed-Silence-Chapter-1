using System;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Events;

public class LevelLayout : MonoBehaviour
{
	[field: SerializeField] public int Id { get; private set; }


    [field: SerializeField] public List<Vector3> NextLayoutOffsets { get; private set; }
    [field: SerializeField] public List<Vector3> NextLayoutRotations { get; private set; }

    [SerializeField] private List<Behaviour_GenericAction> doorActions;
	[SerializeField] private LayoutData layoutData;

	[Header("Style")]
	[SerializeField] private MeshRenderer[] wallRenderers;
	[SerializeField] private MeshRenderer[] doorWallRenderers;
	[SerializeField] private MeshRenderer[] windowWallRenderers;
	[SerializeField] private MeshRenderer[] ceilingRenderers;
	[SerializeField] private MeshRenderer[] floorRenderers;
	
	[NonSerialized] public bool CanDispose;

    public void Setup(LayoutStyle style, List<int> nextLevelIds, params LevelDecorator[] decorators)
	{
		if(nextLevelIds != null) SetDoorActions(nextLevelIds);
        SetLayoutStyle(style);
    }

	private void SetDoorActions(List<int> nextLevelIds)
	{
		for(int i = 0; i < doorActions.Count; i++)
		{
			var id = nextLevelIds[i];
			var offset = transform.position + NextLayoutOffsets[i];
			var rotation = Quaternion.Euler(NextLayoutRotations[i]);
            UnityAction action = ()=> LevelLayoutManager.Instance.ActivateLayout(id, offset, rotation, null);

            doorActions[i].Setup(action, onInteraction: true, onInspection: false);
        }
    }

	private void SetLayoutStyle(LayoutStyle style)
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
			case LayoutStyle.Style1:
				upperMat = layoutData.WallMat1;
				lowerMat = layoutData.LowerWallMat1;
				ceilingMat = layoutData.CeilingMat1;
				floorMat = layoutData.FloorMat1;
				windowDecorMat1 = layoutData.WindowDecorMat1;
				windowDecorMat2 = layoutData.WindowDecorMat1;
				break;
            case LayoutStyle.Style2:
                upperMat = layoutData.WallMat2;
                lowerMat = layoutData.LowerWallMat2;
                ceilingMat = layoutData.CeilingMat2;
                floorMat = layoutData.FloorMat2;
                windowDecorMat1 = layoutData.WindowDecorMat2;
                windowDecorMat2 = layoutData.WindowDecorMat2;
                break;
            case LayoutStyle.Style3:
                upperMat = layoutData.WallMat3;
                lowerMat = layoutData.LowerWallMat3;
                ceilingMat = layoutData.CeilingMat3;
                floorMat = layoutData.FloorMat3;
                windowDecorMat1 = layoutData.WindowDecorMat3;
                windowDecorMat2 = layoutData.WindowDecorMat3;
                break;
            case LayoutStyle.Style4:
                upperMat = layoutData.WallMat4;
                lowerMat = layoutData.LowerWallMat4;
                ceilingMat = layoutData.CeilingMat4;
                floorMat = layoutData.FloorMat4;
                windowDecorMat1 = layoutData.WindowDecorMat4;
                windowDecorMat2 = layoutData.WindowDecorMat4;
                break;
            case LayoutStyle.Style5:
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

	private void SetMaterials(Material upperMat = null, Material lowerMat = null,
		Material ceilingMat = null, Material floorMat = null,
		Material windowDecorMat1 = null, Material windowDecorMat2 = null)
	{
		if(upperMat || lowerMat)
		{
			foreach (var renderer in wallRenderers)
			{
				var wallMaterials = renderer.materials;

				if (upperMat) wallMaterials[0] = upperMat;
				if (lowerMat) wallMaterials[1] = lowerMat;

				renderer.materials = wallMaterials;
			}

			foreach (var renderer in doorWallRenderers)
			{
				var doorWallMaterials = renderer.materials;

				if (upperMat) doorWallMaterials[1] = upperMat;
				if (lowerMat) doorWallMaterials[0] = lowerMat;

				renderer.materials = doorWallMaterials;
			}
		}

		if(upperMat || lowerMat || windowDecorMat1 || windowDecorMat2)
		{
			foreach (var renderer in windowWallRenderers)
			{
				var windowWallMaterials = renderer.materials;

				if (lowerMat) windowWallMaterials[0] = lowerMat;
				if (windowDecorMat2) windowWallMaterials[1] = windowDecorMat2;
				if (windowDecorMat1) windowWallMaterials[3] = windowDecorMat1;
				if (upperMat) windowWallMaterials[4] = upperMat;

				renderer.materials = windowWallMaterials;
			}
		}

		if(ceilingMat)
		{
			foreach(var renderer in ceilingRenderers)
			{
				var ceilingMaterials = renderer.materials;
				ceilingMaterials[0] = ceilingMat;
				renderer.materials = ceilingMaterials;
			}
		}

		if(floorMat)
		{
			foreach(var renderer in  floorRenderers)
			{
				var floorMaterials = renderer.materials;
				floorMaterials[0] = floorMat;
				renderer.materials = floorMaterials;
			}
		}
	}
}

public record Layout
{
	public bool enable;
	public int id;
	public List<int> nextLevelIds;
	public LayoutStyle style;
}