using UnityEngine;

public class LevelLayout : MonoBehaviour
{
	[field: SerializeField] public int Id { get; private set; }
	public bool CanDispose;

	[SerializeField] private GameObject LayoutTrigger1;
	[SerializeField] private GameObject LayoutTrigger2;

	[SerializeField] private Material testMat;
	[SerializeField] private Material testMat2;

	[SerializeField] private MeshRenderer[] wallRenderers;
	[SerializeField] private MeshRenderer[] doorWallRenderers;
	[SerializeField] private MeshRenderer[] windowWallRenderers;
	[SerializeField] private MeshRenderer[] ceilingRenderers;

	private void Start()
	{
		SetMaterials(upperMat: testMat, lowerMat: testMat2);
	}

	public void SetMaterials(Material upperMat = null, Material lowerMat = null, Material ceilingMat = null, Material windowDecorMat1 = null, Material windowDecorMat2 = null)
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
	}

	private void OnDisable()
	{
		LayoutTrigger1.SetActive(false);
		LayoutTrigger2.SetActive(false);
	}
}