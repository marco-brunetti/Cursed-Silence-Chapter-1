using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelItemManager : MonoBehaviour
{
	private LevelItem[] itemPrefabs;

	private void Awake()
	{
		itemPrefabs = Resources.LoadAll<LevelItem>("Items/");
	}

	public void AddItems(Transform[] itemAnchors)
	{
		var freeAnchors = new List<Transform>(); 
		Array.ForEach(itemAnchors, anchor => {
			var isAnchorFree = anchor.Cast<Transform>().All(child => !child.gameObject.activeInHierarchy);
			if (isAnchorFree) freeAnchors.Add(anchor);
		});


	}
}