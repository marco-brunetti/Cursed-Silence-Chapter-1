using System.Collections.Generic;
using UnityEngine;

public class LevelItemManager : MonoBehaviour
{
	[SerializeField] private Transform itemPoolParent;
	[SerializeField] private LayoutData layoutData;
	private System.Random random = new();
	private LevelItem[] prefabResources;
	private Dictionary<int, LevelItem> prefabs = new();
	private Dictionary<int, LevelItem> itemPool = new();
	private Dictionary<int, LevelItem> wallItemPool = new();
	private Dictionary<int, LevelItem> ceilingItemPool = new();
	private Dictionary<int, LevelItem> floorItemPool = new();

	private void Awake()
	{
		itemPoolParent.gameObject.SetActive(false);

		prefabResources = layoutData.itemPrefabs;

		foreach (var prefab in prefabResources)
		{
			prefabs.Add(prefab.Id, prefab);
		}
	}

	//In the map generator, make sure the ids are compatible with this layout shape
	public void FillItems(LevelLayout layout)
	{
		if (layout.ItemList == null) return;

		layout.GetAnchors(out List<Transform> smallAnchors, out List<Transform> mediumAnchors, out List<Transform> largeAnchors);

		foreach(var itemReference in layout.ItemList)
		{
			var item = GetItem(itemReference.id);
			if (item == null) continue;

			if (largeAnchors.Count > 0 && item.Size == LayoutItemSize.Large)
			{
				AddItem(item, largeAnchors);
				continue;
			}

			if (mediumAnchors.Count > 0 && item.Size == LayoutItemSize.Medium)
			{
				AddItem(item, mediumAnchors);
				continue;
			}

			if (smallAnchors.Count > 0 && item.Size == LayoutItemSize.Small)
			{
				AddItem(item, smallAnchors);
				continue;
			}
		}
	}

	private LevelItem GetItem(int id)
	{
		if (itemPool.TryGetValue(id, out var item))
		{
			//Makes sure the item is free for use
			if(item.transform.parent = itemPoolParent)
			{
				return item;
			}
			else
			{
				Debug.Log($"Item with id: {id} being used.");
				return null;
			}
		}

		if(prefabs.TryGetValue(id, out var prefab))
		{
			var instance = Instantiate(prefab, itemPoolParent);
			instance.transform.parent = itemPoolParent;
			instance.gameObject.SetActive(false);
			return instance;
		}

		Debug.Log($"Item with id: {id} not found.");
		return null;
	}

	private void AddItem(LevelItem item, List<Transform> anchors)
	{
		item.IsUsed = true;
		item.transform.parent = anchors[0];
		item.transform.SetLocalPositionAndRotation(item.Position, Quaternion.Euler(item.Rotation));
		item.transform.localScale = item.Scale;
		item.gameObject.SetActive(true);
		anchors.RemoveAt(0);
	}

	public void RemoveFrom(LevelLayout layout)
	{
		layout.GetAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

		foreach (var layoutAnchor in wallAnchors)
		{
			if(layoutAnchor.childCount >  0) RemoveItemFrom(layoutAnchor);
		}

		foreach (var layoutAnchor in ceilingAnchors)
		{
			if (layoutAnchor.childCount > 0) RemoveItemFrom(layoutAnchor);
		}

		foreach (var layoutAnchor in floorAnchors)
		{
			if (layoutAnchor.childCount > 0) RemoveItemFrom(layoutAnchor);
		}
	}

	private void RemoveItemFrom(Transform layoutAnchor) 
	{
		foreach(Transform child in layoutAnchor)
		{
			if(child.TryGetComponent(out LevelItem item))
			{
				item.gameObject.SetActive(false);
				item.transform.parent = itemPoolParent;
				item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				item.IsUsed = false;
			}
		}
	}
}