using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
    [SerializeField] private LevelLayout[] layoutPrefabs;
    private LevelLayout currentLayout;
    [SerializeField] private TextAsset mapJson;

    private HashSet<LevelLayout> activeLayouts = new();
    private Queue<LevelLayout> deactivateQueue = new();
    private List<Layout> mapLayouts = new();
    private LayoutMap map;

    private int currentLayoutIndex = -1;

    public static LevelLayoutManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        SetupMap();
    }

    private void Start()
    {
        if (currentLayout) activeLayouts.Add(currentLayout);
        //StartCoroutine(DeactivateLevelLayouts());
    }

    private void SetupMap()
    {
        map = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

        for (int i = 0; i < map.LayoutStates.Count; i++)
        {
            mapLayouts.Add(map.LayoutStates[i]);
            if (currentLayoutIndex == -1 && map.LayoutStates[i].enable) currentLayoutIndex = i;
        }

        //Sets up first level
        var currentMapLayout = mapLayouts[currentLayoutIndex];
        currentLayout = FindObjectOfType<LevelLayout>();
        currentLayout.Setup(currentMapLayout.style, currentMapLayout.nextLayoutShapes, null);
    }

    public void SetCurrentLayout(LevelLayout newCurrent)
    {
        MarkForDeactivation(exceptions: new() { currentLayout });
        currentLayout = newCurrent;
    }

    public void ActivateLayout(LevelLayout triggeredLayout, LayoutShape nextShape, Vector3 position, Quaternion rotation, params LevelDecorator[] decorators)
    {
        var nextLayout = activeLayouts.FirstOrDefault(x => x.Shape == nextShape && !x.gameObject.activeInHierarchy);

        if (!nextLayout)
        {
            nextLayout = Instantiate(Array.Find(layoutPrefabs, e => e.Shape == nextShape));
            activeLayouts.Add(nextLayout);
        }

        nextLayout.transform.parent = triggeredLayout.transform;
        nextLayout.transform.SetLocalPositionAndRotation(position, rotation);
        nextLayout.transform.parent = null;
        //levelLayout.Setup(LayoutStyle.Style2, doorActions: null, decorators: decorators);
        nextLayout.gameObject.SetActive(true);

        //foreach (var decorator in decorators)
        //{
        //    decorator.ApplyDecorator(levelLayout);
        //}

        MarkForDeactivation(exceptions: new() { currentLayout });
        currentLayout = nextLayout;

        if(currentLayoutIndex == mapLayouts.Count - 1)
        {
            Debug.Log("No more layouts");
            return;
        }
        else
        {
            currentLayoutIndex++;
            var mapLayout = mapLayouts[currentLayoutIndex];
            currentLayout.Setup(mapLayout.style, mapLayout.nextLayoutShapes, null);
        }
    }

    private void MarkForDeactivation(List<LevelLayout> exceptions)
    {
        foreach (var layout in activeLayouts)
        {
            if (exceptions.Contains(layout))
            {
                layout.CanDispose = false;
            }
            else
            {
                layout.CanDispose = true;
                deactivateQueue.Enqueue(layout);
            }
        }
    }

    private IEnumerator DeactivateLevelLayouts()
    {
        while (true)
        {
            if (deactivateQueue.Count > 0)
            {
                var layout = deactivateQueue.Dequeue();
                if (layout.CanDispose) layout.gameObject.SetActive(false);
            }

            yield return null;
        }
    }
}

public record LayoutMap
{
    [JsonProperty("layoutStates")]
    public List<Layout> LayoutStates;
}