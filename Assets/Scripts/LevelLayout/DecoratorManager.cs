using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecoratorManager : MonoBehaviour
{
	[SerializeField] private Transform decoratorPoolParent;
	[SerializeField] private LevelItemManager levelItemManager;

    private System.Random random = new();
    private LevelDecorator[] decoratorPrefabs;
	private HashSet<LevelDecorator> wallDecoPool = new();
	private HashSet<LevelDecorator> ceilingDecoPool = new();
	private HashSet<LevelDecorator> floorDecoPool = new();


	public static DecoratorManager Instance { get; private set; }
	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(this);

		decoratorPoolParent.gameObject.SetActive(false); //Ensures all pool children are inactive

		decoratorPrefabs = Resources.LoadAll<LevelDecorator>("Decorators/");


		//TODO: instantiate only when using decorators
		foreach (var prefab in decoratorPrefabs)
		{
			var decorator = Instantiate(prefab, decoratorPoolParent);

			if (decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Wall)) wallDecoPool.Add(decorator);
			if (decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Ceiling)) ceilingDecoPool.Add(decorator);
			if (decorator.LayoutAnchors.Contains(LayoutAnchorCompatibility.Floor)) floorDecoPool.Add(decorator);

			decorator.gameObject.SetActive(false);
		}
	}

	public void Decorate(LevelLayout layout)
	{
		layout.GetFreeAnchors(out List<Transform> wallAnchors, out List<Transform> ceilingAnchors, out List<Transform> floorAnchors);
		SetAnchors(wallDecoPool, wallAnchors, layout.Shape);
		SetAnchors(ceilingDecoPool, ceilingAnchors, layout.Shape);
		SetAnchors(floorDecoPool, floorAnchors, layout.Shape);
    }

	private void SetAnchors(HashSet<LevelDecorator> pool, List<Transform> layoutAnchors, LayoutShape shape)
	{
        foreach (var anchor in layoutAnchors)
        {
            if (pool == null ||
                pool.Count == 0 ||
                pool.All(x => (x.IsUsed || !x.Enable)) ||
                !pool.Any(x => x.Layouts.Contains(shape)))
            {
                break;
            }

            AddDecorator(pool, anchor, shape);
        }
    }

	private void AddDecorator(HashSet<LevelDecorator> pool, Transform layoutAnchor, LayoutShape shape)
	{
		if(!pool.Any(x=>x.Layouts.Contains(shape))) return;

        LevelDecorator decorator = null;

        do
        {
			var e = pool.ElementAt(random.Next(pool.Count));
            decorator = (e.enabled && !e.IsUsed) ? e : null;
        }
        while (decorator == null);

		decorator.IsUsed = true;
        decorator.transform.parent = layoutAnchor;
		decorator.transform.SetLocalPositionAndRotation(decorator.Position, Quaternion.Euler(decorator.Rotation));
		decorator.transform.localScale = decorator.Scale;
        decorator.gameObject.SetActive(true);
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