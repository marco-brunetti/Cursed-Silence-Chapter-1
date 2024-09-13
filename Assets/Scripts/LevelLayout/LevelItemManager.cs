using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelItemManager : MonoBehaviour
{
	[SerializeField] private Transform itemPoolParent;

	private LevelItem[] prefabResources;
	private Dictionary<int, LevelItem> prefabs = new();
	private Dictionary<int, LevelItem> itemPool;

	private void Awake()
	{
		prefabResources = Resources.LoadAll<LevelItem>("Items/");
	}

	public void AddItems(LevelDecorator decorator)
	{
		var freeAnchors = new List<Transform>();

		decorator.GetItemAnchors().ForEach(x => { 
			var isFree = x.Cast<Transform>().All(child => !child.gameObject.activeInHierarchy);
			if(isFree) freeAnchors.Add(x);
		});

		foreach(var itemReference in decorator.ItemList) 
		{
			var item = GetItem(itemReference.Id);
			if (item == null) continue;

			if (freeAnchors.Count > 0)
			{
				AddItem(itemReference, freeAnchors);
				continue;
			}
		}
	}

	private LevelItem GetItem(int id)
	{
		if (itemPool.TryGetValue(id, out var item))
		{
			//Makes sure the item is free for use
			if (item.transform.parent = itemPoolParent)
			{
				return item;
			}
			else
			{
				Debug.Log($"Item with id: {id} being used.");
				return null;
			}
		}

		if (prefabs.TryGetValue(id, out var prefab))
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

	public void RemoveFrom(LevelDecorator decorator)
	{
		decorator.GetItemAnchors().ForEach(x =>
		{
			if(x.childCount > 0) RemoveItems(x);
		});
	}

	private void RemoveItems(Transform anchor)
	{
		foreach (Transform child in anchor)
		{
			if (child.TryGetComponent(out LevelItem item))
			{
				item.gameObject.SetActive(false);
				item.transform.parent = itemPoolParent;
				item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				item.IsUsed = false;
			}
		}
	}
}