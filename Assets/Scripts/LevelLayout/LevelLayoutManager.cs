using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{

	private LevelLayout currentLayout;
	[SerializeField] private TextAsset mapJson;

	private HashSet<LevelLayout> layoutPool = new();
	private Queue<LevelLayout> deactivateQueue = new();
	private LevelLayout[] layoutPrefabs;
	private List<Layout> loadedMap = new();
	private LayoutMap layoutMap;

	private int currentLayoutIndex = -1;

	public static LevelLayoutManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		layoutPrefabs = Resources.LoadAll<LevelLayout>("Layouts/");
		SetupMap();
	}

	private void Start()
	{
		if (currentLayout) layoutPool.Add(currentLayout);
		//StartCoroutine(DeactivateLevelLayouts());
	}

	private void SetupMap()
	{
		layoutMap = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

		for (int i = 0; i < layoutMap.LayoutStates.Count; i++)
		{
			loadedMap.Add(layoutMap.LayoutStates[i]);
			if (currentLayoutIndex == -1 && layoutMap.LayoutStates[i].enable) currentLayoutIndex = i;
		}

		var currentMapLayout = loadedMap[currentLayoutIndex];

		ActivateLayout(null, currentMapLayout.nextLayoutShapes[0], Vector3.zero, Quaternion.Euler(Vector3.zero), null);

		//Sets up first level

		//currentLayout = FindObjectOfType<LevelLayout>();
		//currentLayout.Setup(currentMapLayout.style, currentMapLayout.nextLayoutShapes, isEndOfZone: false, null);
	}

	public void SetCurrentLayout(LevelLayout newCurrent)
	{
		//MarkForDeactivation(exceptions: new() { currentLayout });
		//currentLayout = newCurrent;
	}

	public void ActivateLayout(LevelLayout triggeredLayout, LayoutShape nextShape, Vector3 position, Quaternion rotation, params LevelDecorator[] decorators)
	{
		var nextLayout = layoutPool.FirstOrDefault(x => x.Shape == nextShape && !x.gameObject.activeInHierarchy);

		if (!nextLayout)
		{
			nextLayout = Instantiate(Array.Find(layoutPrefabs, e => e.Shape == nextShape));
			layoutPool.Add(nextLayout);
		}

		if (triggeredLayout)
		{
			nextLayout.transform.parent = triggeredLayout.transform;
			nextLayout.transform.SetLocalPositionAndRotation(position, rotation);
			nextLayout.transform.parent = null;
		}
		else
		{
			nextLayout.transform.SetPositionAndRotation(position, rotation);
		}
			
		//levelLayout.Setup(LayoutStyle.Style2, doorActions: null, decorators: decorators);
		nextLayout.gameObject.SetActive(true);

		//foreach (var decorator in decorators)
		//{
		//    decorator.ApplyDecorator(levelLayout);
		//}

		var mapLayout = loadedMap[currentLayoutIndex];
		var isEndOfZone = currentLayoutIndex < loadedMap.Count - 1 && mapLayout.zone != loadedMap[currentLayoutIndex + 1].zone;

		currentLayout = nextLayout;

		if(currentLayoutIndex == loadedMap.Count)
		{
			Debug.Log("No more layouts");
			return;
		}
		else
		{
			currentLayout.Setup(currentLayoutIndex, mapLayout.style, mapLayout.nextLayoutShapes, isEndOfZone, null);
			if(currentLayout.HasDoors()) currentLayoutIndex++;
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
		if (deactivateQueue.Count == 0)
		{
			if (index < loadedMap.Count)
			{
				var currentZoneEntrance = layoutPool.FirstOrDefault(x => x.MapIndex == index && x.gameObject.activeInHierarchy);
				currentZoneEntrance.EntranceDoorEnabled(true);
			}
		}
	}

	private void MarkForDeactivation()
	{
		foreach (var layout in layoutPool)
		{
			if (layout.MapIndex <= currentLayoutIndex)
			{
				deactivateQueue.Enqueue(layout);
			}
		}
	}
}

public record LayoutMap
{
	[JsonProperty("layoutStates")]
	public List<Layout> LayoutStates;
}