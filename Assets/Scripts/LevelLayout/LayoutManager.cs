using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
	[SerializeField] private TextAsset mapJson;
    [SerializeField] private LayoutData layoutData;
    [SerializeField] private LevelItemManager itemManager;

    private int currentIndex;
	private LayoutMap savedMap;
	private List<Layout> loadedMap = new();
	private LevelLayout mainLevel;
    private Dictionary<LayoutType, LevelLayout> layoutPool = new();
    private Queue<KeyValuePair<LayoutType, LevelLayout>> deactivateQueue = new();

	public static LayoutManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		savedMap = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

		if (layoutData.prefabs.TryGetValue(LayoutType.MainLevelStyle0, out var mainLevelPrefab))
		{
            mainLevel = Instantiate(mainLevelPrefab);
            mainLevel.gameObject.SetActive(false);
        }

		for (int i = 0; i < savedMap.Layouts.Count; i++)
		{
			if (savedMap.Layouts[i].enable) loadedMap.Add(savedMap.Layouts[i]);
		}

		var currentMapLayout = loadedMap[currentIndex];
		ActivateLayout(null, currentMapLayout.nextShapes[0], Vector3.zero, Quaternion.Euler(Vector3.zero), null);
	}

	public void ActivateLayout(Transform previousLayout, LayoutType nextShape, Vector3 position, Quaternion rotation, params Leveltem[] decorators)
	{
		if (currentIndex >= loadedMap.Count)
		{
			Debug.Log("End of map.");
			return;
		}

		LevelLayout newLayout = null;

		if(nextShape == LayoutType.MainLevelStyle0 && mainLevel)
		{
			newLayout = mainLevel;
		}
		else
		{
			if(!layoutPool.TryGetValue(nextShape, out newLayout))
			{
				if(layoutData.prefabs.TryGetValue(nextShape, out newLayout))
				{
					newLayout = Instantiate(newLayout);
					layoutPool.Add(newLayout.Shape, newLayout);
                }
				else
				{
					Debug.Log($"Layout shape {nextShape} not found.");
					return;
				}
            }
		}

		if (previousLayout) newLayout.transform.parent = previousLayout;
		newLayout.transform.SetLocalPositionAndRotation(position, rotation);
		newLayout.transform.parent = null;
		newLayout.gameObject.SetActive(true);

		var i = currentIndex;
		var mapLayout = loadedMap[i];
		var isEndOfZone = i < (loadedMap.Count - 1) && mapLayout.zone != loadedMap[i + 1].zone;

		newLayout.Setup(i, mapLayout.nextShapes, isEndOfZone, decorators: null);
		if (i == 0) newLayout.EntranceDoorEnabled(true);
		newLayout.ItemList = mapLayout.decorators;
		itemManager.FillItems(newLayout);
		if (newLayout.HasDoors()) currentIndex++;
	}

	public IEnumerator DeactivateLevelLayouts()
	{
		while (deactivateQueue.Count > 0)
		{
			var layout = deactivateQueue.Dequeue();
			layout.Value.gameObject.SetActive(false);
			ActivateZoneEntranceDoor(layout.Value.MapIndex + 1);
			itemManager.RemoveFrom(layout.Value);
			layout.Value.MapIndex = -1;
			yield return null;
		}

		MarkForDeactivation();
	}

	private void ActivateZoneEntranceDoor(int index)
	{
		if (deactivateQueue.Count == 0 && index < loadedMap.Count)
		{
			layoutPool.FirstOrDefault(x => x.Value.MapIndex == index && x.Value.gameObject.activeInHierarchy).Value.EntranceDoorEnabled(true);
		}
	}

	private void MarkForDeactivation()
	{
		foreach (var layout in layoutPool)
		{
			if (layout.Value.MapIndex <= currentIndex) deactivateQueue.Enqueue(layout);
		}
	}
}