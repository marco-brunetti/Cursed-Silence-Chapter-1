using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
	[SerializeField] private TextAsset mapJson;
	[SerializeField] private Transform decoratorPoolParent;

    private LayoutMap savedMap;
    private List<Layout> loadedMap = new();
    private LevelLayout[] layoutPrefabs;
    private HashSet<LevelLayout> layoutPool = new();
	private Queue<LevelLayout> deactivateQueue = new();

    private int currentIndex;
    private System.Random random = new();
    private LevelDecorator[] decoratorPrefabs;
	private HashSet<LevelDecorator> wallDecoPool = new();
	private HashSet<LevelDecorator> ceilingDecoPool = new();
	private HashSet<LevelDecorator> floorDecoPool = new();


	public static LevelLayoutManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		layoutPrefabs = Resources.LoadAll<LevelLayout>("Layouts/");
		decoratorPrefabs = Resources.LoadAll<LevelDecorator>("Decorators/");
		SetupMap();
		PrepareDecorators();
    }

	private void SetupMap()
	{
		savedMap = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

		for (int i = 0; i < savedMap.LayoutStates.Count; i++)
		{
			if(savedMap.LayoutStates[i].enable) loadedMap.Add(savedMap.LayoutStates[i]);
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

        var newLayout = layoutPool.FirstOrDefault(x => x.Shape == nextShape && !x.gameObject.activeInHierarchy);

		if (!newLayout)
		{
			newLayout = Instantiate(Array.Find(layoutPrefabs, x => x.Shape == nextShape));
			layoutPool.Add(newLayout);
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
        Decorate(newLayout);
        if (newLayout.HasDoors()) currentIndex++;
    }

	private void Decorate(LevelLayout layout)
	{
		layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

		foreach(var anchor in wallAnchors)
		{
            if (wallDecoPool == null || wallDecoPool.Count == 0 || wallDecoPool.All(x => x.IsUsed)) break;
            AddDecorator(wallDecoPool, anchor);
		}

        foreach (var anchor in ceilingAnchors)
        {
            if (ceilingDecoPool == null || ceilingDecoPool.Count == 0 || ceilingDecoPool.All(x => x.IsUsed)) break;
            AddDecorator(ceilingDecoPool, anchor);
        }

        foreach (var anchor in floorAnchors)
        {
            if (floorDecoPool == null || floorDecoPool.Count == 0 || floorDecoPool.All(x => x.IsUsed)) break;
            AddDecorator(floorDecoPool, anchor);
        }
    }

	private void AddDecorator(HashSet<LevelDecorator> pool, Transform anchor)
	{
        LevelDecorator decorator = null;

        do
        {
            var i = random.Next(pool.Count);
            decorator = pool.ElementAt(i).IsUsed ? null : pool.ElementAt(i);
        }
        while (decorator == null);

		decorator.IsUsed = true;
        decorator.transform.parent = anchor;
		decorator.transform.SetLocalPositionAndRotation(decorator.positionOffset, Quaternion.Euler(decorator.rotationOffset));
        decorator.gameObject.SetActive(true);
    }

	private void PrepareDecorators()
	{
		decoratorPoolParent.gameObject.SetActive(false); //Ensures all pool children are inactive
		foreach(var prefab in decoratorPrefabs)
		{
			var decorator = Instantiate(prefab, decoratorPoolParent);

			if (decorator.Compatibility.Contains(DecoratorCompatibility.Wall)) wallDecoPool.Add(decorator);
            if (decorator.Compatibility.Contains(DecoratorCompatibility.Ceiling)) ceilingDecoPool.Add(decorator);
            if (decorator.Compatibility.Contains(DecoratorCompatibility.Floor)) floorDecoPool.Add(decorator);

			decorator.gameObject.SetActive(false);
        }
	}

	public IEnumerator DeactivateLevelLayouts()
	{
		while (deactivateQueue.Count > 0)
		{
			var layout = deactivateQueue.Dequeue();
			layout.gameObject.SetActive(false);
			ActivateZoneEntranceDoor(layout.MapIndex + 1);
			ResetDecorators(layout);
			layout.MapIndex = -1;
			yield return null;
		}

		MarkForDeactivation();
	}

	private void ResetDecorators(LevelLayout layout)
	{
        layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

        foreach (var anchor in wallAnchors)
        {
			if(anchor.childCount ==  0) continue;
            RemoveDecorators(anchor);
        }

        foreach (var anchor in ceilingAnchors)
        {
            if (anchor.childCount == 0) continue;
            RemoveDecorators(anchor);
        }

        foreach (var anchor in floorAnchors)
        {
            if (anchor.childCount == 0) continue;
            RemoveDecorators(anchor);
        }
    }

	private void RemoveDecorators(Transform anchor)
	{
		foreach(Transform child in anchor)
		{
			if(child.TryGetComponent(out LevelDecorator decorator))
			{
                decorator.gameObject.SetActive(false);
                decorator.transform.parent = decoratorPoolParent;
				decorator.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				decorator.IsUsed = false;
			}
		}
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

public record Layout
{
    public bool enable;
    public int zone;
    public List<LayoutShape> nextShapes;
    public LayoutStyle style;
}