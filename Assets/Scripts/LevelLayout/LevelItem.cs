using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItem : MonoBehaviour
{
	[field: SerializeField] public int Id { get; private set; }
	[field: SerializeField] public Vector3 Position { get; private set; }
	[field: SerializeField] public Vector3 Rotation { get; private set; }

	[SerializeField] private Vector3 scale;

	[NonSerialized] public bool IsUsed;

	public Vector3 Scale
	{
		get
		{
			return scale == Vector3.zero ? transform.localScale : scale;
		}
	}
}