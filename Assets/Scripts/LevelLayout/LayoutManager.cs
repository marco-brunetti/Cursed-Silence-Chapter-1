using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
	[SerializeField] private TextAsset mapJson;
	[SerializeField] private DecoratorManager decoratorManager;

	private int currentIndex;
	private LayoutMap savedMap;
	private List<Layout> loadedMap = new();
	private LevelLayout mainLevel;
	private LevelLayout[] layoutPrefabs;
	private HashSet<LevelLayout> layoutPool = new();
	private Queue<LevelLayout> deactivateQueue = new();

	public static LayoutManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		layoutPrefabs = Resources.LoadAll<LevelLayout>("Layouts/");
		savedMap = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

		mainLevel = Instantiate(Array.Find(layoutPrefabs, x => x.Shape == LayoutShape.MainLevel));
		mainLevel.gameObject.SetActive(false);

		for (int i = 0; i < savedMap.Layouts.Count; i++)
		{
			if (savedMap.Layouts[i].enable) loadedMap.Add(savedMap.Layouts[i]);
		}

		var currentMapLayout = loadedMap[currentIndex];
		ActivateLayout(null, currentMapLayout.nextShapes[0], Vector3.zero, Quaternion.Euler(Vector3.zero), null);
	}

	public void ActivateLayout(Transform previousLayout, LayoutShape nextShape, Vector3 position, Quaternion rotation, params LevelDecorator[] decorators)
	{
		if (currentIndex >= loadedMap.Count)
		{
			Debug.Log("End of map.");
			return;
		}

		LevelLayout newLayout = null;

		if(nextShape == LayoutShape.MainLevel)
		{
			newLayout = mainLevel;
		}
		else
		{
			newLayout = layoutPool.FirstOrDefault(x => x.Shape == nextShape && !x.gameObject.activeInHierarchy);

			if (!newLayout)
			{
				newLayout = Instantiate(Array.Find(layoutPrefabs, x => x.Shape == nextShape));
				layoutPool.Add(newLayout);
			}
		}

		if (previousLayout) newLayout.transform.parent = previousLayout;
		newLayout.transform.SetLocalPositionAndRotation(position, rotation);
		newLayout.transform.parent = null;
		newLayout.gameObject.SetActive(true);

		var i = currentIndex;
		var mapLayout = loadedMap[i];
		var isEndOfZone = i < (loadedMap.Count - 1) && mapLayout.zone != loadedMap[i + 1].zone;

		newLayout.Setup(i, mapLayout.style, mapLayout.nextShapes, isEndOfZone, decorators: null);
		if (i == 0) newLayout.EntranceDoorEnabled(true);
		newLayout.DecoratorList = mapLayout.decorators;
		decoratorManager.Decorate(newLayout);
		if (newLayout.HasDoors()) currentIndex++;
	}

	public IEnumerator DeactivateLevelLayouts()
	{
		while (deactivateQueue.Count > 0)
		{
			var layout = deactivateQueue.Dequeue();
			layout.gameObject.SetActive(false);
			ActivateZoneEntranceDoor(layout.MapIndex + 1);
			decoratorManager.RemoveFrom(layout);
			layout.MapIndex = -1;
			yield return null;
		}

		MarkForDeactivation();
	}

	private void ActivateZoneEntranceDoor(int index)
	{
		if (deactivateQueue.Count == 0 && index < loadedMap.Count)
		{
			layoutPool.FirstOrDefault(x => x.MapIndex == index && x.gameObject.activeInHierarchy).EntranceDoorEnabled(true);
		}
	}

	private void MarkForDeactivation()
	{
		foreach (var layout in layoutPool)
		{
			if (layout.MapIndex <= currentIndex) deactivateQueue.Enqueue(layout);
		}
	}
}