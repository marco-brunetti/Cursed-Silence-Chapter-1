using System.Collections.Generic;
using UnityEngine;

public class LevelItemManager : MonoBehaviour
{
	[SerializeField] private Transform itemPoolParent;
	private System.Random random = new();
	private Leveltem[] prefabResources;
	private Dictionary<int, Leveltem> prefabs = new();
	private Dictionary<int, Leveltem> itemPool = new();
	private Dictionary<int, Leveltem> wallItemPool = new();
	private Dictionary<int, Leveltem> ceilingItemPool = new();
	private Dictionary<int, Leveltem> floorItemPool = new();

	private void Awake()
	{
		itemPoolParent.gameObject.SetActive(false); //Ensures all pool children are inactive

		prefabResources = Resources.LoadAll<Leveltem>("Items/");

		foreach (var prefab in prefabResources)
		{
			prefabs.Add(prefab.Id, prefab);
		}
	}

	//In the map generator, make sure the ids are compatible with this layout shape
	public void FillItems(LevelLayout layout)
	{
		if (layout.ItemList == null) return;

		layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

		foreach(var itemReference in layout.ItemList)
		{
			var item = GetItem(itemReference.id);
			if (item == null) continue;

			if (ceilingAnchors.Count > 0 && item.LayoutAnchors.Contains(LayoutAnchorCompatibility.Ceiling))
			{
				AddItem(item, ceilingAnchors);
				continue;
			}

			if (wallAnchors.Count > 0 && item.LayoutAnchors.Contains(LayoutAnchorCompatibility.Wall))
			{
				AddItem(item, wallAnchors);
				continue;
			}

			if (floorAnchors.Count > 0 && item.LayoutAnchors.Contains(LayoutAnchorCompatibility.Floor))
			{
				AddItem(item, floorAnchors);
				continue;
			}
		}
	}

	private Leveltem GetItem(int id)
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

	private void AddItem(Leveltem item, List<Transform> anchors)
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
		layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

		foreach (var layoutAnchor in wallAnchors) //15
		{
			if(layoutAnchor.childCount >  0) RemoveItemFrom(layoutAnchor);
		}

		foreach (var layoutAnchor in ceilingAnchors) //15
        {
			if (layoutAnchor.childCount > 0) RemoveItemFrom(layoutAnchor);
		}

		foreach (var layoutAnchor in floorAnchors) //15
        {
			if (layoutAnchor.childCount > 0) RemoveItemFrom(layoutAnchor);
		}
	}

	private void RemoveItemFrom(Transform layoutAnchor) 
	{
		foreach(Transform child in layoutAnchor) //40
        {
			if(child.TryGetComponent(out Leveltem item))
			{
				item.gameObject.SetActive(false);
				item.transform.parent = itemPoolParent;
				item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				item.IsUsed = false;
			}
		}
	}
}