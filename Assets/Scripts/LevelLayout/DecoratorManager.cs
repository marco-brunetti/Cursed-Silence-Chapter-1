using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DecoratorManager : MonoBehaviour
{
	[SerializeField] private Transform decoratorPoolParent;
	[SerializeField] private LevelItemManager levelItemManager;

	private System.Random random = new();
	private LevelDecorator[] prefabResources;
	private Dictionary<int, LevelDecorator> prefabs = new();
	private Dictionary<int, LevelDecorator> decoPool = new();
	private Dictionary<int, LevelDecorator> wallDecoPool = new();
	private Dictionary<int, LevelDecorator> ceilingDecoPool = new();
	private Dictionary<int, LevelDecorator> floorDecoPool = new();

	//private HashSet<LevelDecorator> wallDecoPool = new();
	//private HashSet<LevelDecorator> ceilingDecoPool = new();
	//private HashSet<LevelDecorator> floorDecoPool = new();


	public static DecoratorManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		decoratorPoolParent.gameObject.SetActive(false); //Ensures all pool children are inactive

		prefabResources = Resources.LoadAll<LevelDecorator>("Decorators/");


		//TODO: instantiate only when using decorators
		foreach (var prefab in prefabResources)
		{
			//var decorator = Instantiate(prefab, decoratorPoolParent);

			//if (decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Wall)) wallDecoPool.Add(decorator);
			//if (decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Ceiling)) ceilingDecoPool.Add(decorator);
			//if (decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Floor)) floorDecoPool.Add(decorator);

			//decorator.gameObject.SetActive(false);

			prefabs.Add(prefab.Id, prefab);
		}
	}

	//In the map generator, make sure the ids are compatible with this layout shape
	public void Decorate(LevelLayout layout)
	{
		if (layout.DecoratorList == null) return;

		layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

		foreach(var decoratorReference in layout.DecoratorList)
		{
			var decorator = GetDecorator(decoratorReference.id);
			if (decorator == null) continue;

			if (ceilingAnchors.Count > 0 && decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Ceiling))
			{
				AddDecorator(decorator, ceilingAnchors);
				continue;
			}

			if (wallAnchors.Count > 0 && decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Wall))
			{
				AddDecorator(decorator, wallAnchors);
				continue;
			}

			if (floorAnchors.Count > 0 && decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Floor))
			{
				AddDecorator(decorator, floorAnchors);
				continue;
			}
		}
	}

	private LevelDecorator GetDecorator(int id)
	{
		if (decoPool.TryGetValue(id, out var decorator))
		{
			//Makes sure the decorator is free for use
			if(decorator.transform.parent = decoratorPoolParent)
			{
				return decorator;
			}
			else
			{
				Debug.Log($"Decorator with id: {id} being used.");
				return null;
			}
		}

		if(prefabs.TryGetValue(id, out var prefab))
		{
			var instance = Instantiate(prefab, decoratorPoolParent);
			instance.transform.parent = decoratorPoolParent;
			instance.gameObject.SetActive(false);
			return instance;
		}

		Debug.Log($"Decorator with id: {id} not found.");
		return null;
	}

	private void AddDecorator(LevelDecorator decorator, List<Transform> anchors)
	{
		decorator.IsUsed = true;
		decorator.transform.parent = anchors[0];
		decorator.transform.SetLocalPositionAndRotation(decorator.Position, Quaternion.Euler(decorator.Rotation));
		decorator.transform.localScale = decorator.Scale;
		decorator.gameObject.SetActive(true);
		anchors.RemoveAt(0);
	}

	public void RemoveFrom(LevelLayout layout)
	{
		layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);

		foreach (var layoutAnchor in wallAnchors)
		{
			if(layoutAnchor.childCount ==  0) continue;
			RemoveDecorators(layoutAnchor);
		}

		foreach (var layoutAnchor in ceilingAnchors)
		{
			if (layoutAnchor.childCount == 0) continue;
			RemoveDecorators(layoutAnchor);
		}

		foreach (var layoutAnchor in floorAnchors)
		{
			if (layoutAnchor.childCount == 0) continue;
			RemoveDecorators(layoutAnchor);
		}
	}

	private void RemoveDecorators(Transform layoutAnchor)
	{
		foreach(Transform child in layoutAnchor)
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
}