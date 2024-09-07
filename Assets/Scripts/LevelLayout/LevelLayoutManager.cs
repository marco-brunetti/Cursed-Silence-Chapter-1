using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
	[SerializeField] private TextAsset mapJson;

    private HashSet<LevelLayout> layoutPool = new();
	private Queue<LevelLayout> deactivateQueue = new();
	private LevelLayout[] layoutPrefabs;
	private LayoutMap layoutMap;
    private List<Layout> loadedMap = new();
    private int currentIndex;

	public static LevelLayoutManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		layoutPrefabs = Resources.LoadAll<LevelLayout>("Layouts/");
		SetupMap();
	}

	private void SetupMap()
	{
		layoutMap = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

		for (int i = 0; i < layoutMap.LayoutStates.Count; i++)
		{
			if(layoutMap.LayoutStates[i].enable) loadedMap.Add(layoutMap.LayoutStates[i]);
		}

		var currentMapLayout = loadedMap[currentIndex];
		ActivateLayout(null, currentMapLayout.nextLayoutShapes[0], Vector3.zero, Quaternion.Euler(Vector3.zero), null);
	}

	public void ActivateLayout(LevelLayout previousLayout, LayoutShape nextShape, Vector3 position, Quaternion rotation, params LevelDecorator[] decorators)
	{
        if (currentIndex == loadedMap.Count)
        {
            Debug.Log("End of map.");
            return;
        }

        var newLayout = layoutPool.FirstOrDefault(x => x.Shape == nextShape && !x.gameObject.activeInHierarchy);

		if (!newLayout)
		{
			newLayout = Instantiate(Array.Find(layoutPrefabs, x => x.Shape == nextShape));
			layoutPool.Add(newLayout);
		}

		if (previousLayout) newLayout.transform.parent = previousLayout.transform;

        newLayout.transform.SetLocalPositionAndRotation(position, rotation);
        newLayout.transform.parent = null;
        newLayout.gameObject.SetActive(true);

        //levelLayout.Setup(LayoutStyle.Style2, doorActions: null, decorators: decorators);

        //foreach (var decorator in decorators)
        //{
        //    decorator.ApplyDecorator(levelLayout);
        //}

        var mapLayout = loadedMap[currentIndex];
		var isEndOfZone = currentIndex < loadedMap.Count - 1 && mapLayout.zone != loadedMap[currentIndex + 1].zone;

        if (currentIndex < loadedMap.Count)
        {
            newLayout.Setup(currentIndex, mapLayout.style, mapLayout.nextLayoutShapes, isEndOfZone, decorators: null);
            if (currentIndex == 0) newLayout.EntranceDoorEnabled(true);
            if (newLayout.HasDoors()) currentIndex++;
        }
    }

	public IEnumerator DeactivateLevelLayouts()
	{
		while (deactivateQueue.Count > 0)
		{
			var layout = deactivateQueue.Dequeue();
			layout.gameObject.SetActive(false);
			ActivateZoneEntranceDoor(layout.MapIndex + 1);
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

public record LayoutMap
{
	[JsonProperty("layoutStates")]
	public List<Layout> LayoutStates;
}