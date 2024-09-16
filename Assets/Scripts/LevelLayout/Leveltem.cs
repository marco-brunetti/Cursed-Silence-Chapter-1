using System;
using System.Collections.Generic;
using UnityEngine;

public class Leveltem : MonoBehaviour
{
	[field: SerializeField, Header("Decorator parameters")] public int Id { get; private set; } = 0;
	[field: SerializeField] public bool Enable { get; private set; } = true;
	[field: SerializeField] public List<LayoutType> Layouts { get; private set; }
	[field: SerializeField] public List<LayoutAnchorCompatibility> LayoutAnchors { get; private set; }
	[field: SerializeField] public Vector3 Position { get; private set; }
	[field: SerializeField] public Vector3 Rotation { get; private set; }

	[SerializeField] private Vector3 scale;

	[Header("Item parameters")]
	[SerializeField] private List<Transform> itemAnchors;

	[NonSerialized] public bool IsUsed;

	public Vector3 Scale 
	{
		get
		{
			return scale == Vector3.zero ? transform.localScale : scale;
		}
	}

	public List<Transform> GetItemAnchors()
	{
		return new(itemAnchors);
	}
}

public enum LayoutAnchorCompatibility
{
	Ceiling,
	Wall,
	Floor
}