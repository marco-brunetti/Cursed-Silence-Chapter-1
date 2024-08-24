using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelLayoutManager : MonoBehaviour
{
    [SerializeField] private LevelLayout[] layoutPrefabs;
    [SerializeField] private LevelLayout currentLayout;
    [SerializeField] private TextAsset mapJson;

    private HashSet<LevelLayout> activeLayouts = new();
    private Queue<LevelLayout> deactivateQueue = new();
    private List<LayoutState> mapLayoutStates = new();
    private LayoutMap map;

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
        StartCoroutine(DeactivateLevelLayouts());
    }

    private void SetupMap()
    {
        map = JsonConvert.DeserializeObject<LayoutMap>(mapJson.ToString());

        foreach (var state in map.LayoutStates)
        {
            mapLayoutStates.Add(state);
        }

        //Sets up first level
        currentLayout.Setup(mapLayoutStates[0].style, null, null);
    }

    public void SetCurrentLayout(LevelLayout newCurrent)
    {
        //MarkForDeactivation(exceptions: new() { currentLayout, newCurrent });
        currentLayout = newCurrent;
    }

    public void ActivateLayout(int id, Vector3 position, Quaternion rotation, params LevelDecorator[] decorators)
    {
        var levelLayout = activeLayouts.FirstOrDefault(x => x.Id == id && !x.gameObject.activeInHierarchy);

        if (!levelLayout)
        {
            levelLayout = Instantiate(Array.Find(layoutPrefabs, e => e.Id == id));
            activeLayouts.Add(levelLayout);
        }

        levelLayout.transform.SetPositionAndRotation(position, rotation);
        levelLayout.Setup(LayoutStyle.Style2, doorAction: null, decorators: decorators);
        levelLayout.gameObject.SetActive(true);

        foreach (var decorator in decorators)
        {
            decorator.ApplyDecorator(levelLayout);
        }

        //MarkForDeactivation(exceptions: new() { currentLayout, levelLayout });

        currentLayout = levelLayout;
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
                if (layout.CanDispose)
                {
                    layout.gameObject.SetActive(false);
                }
            }

            yield return null;
        }
    }
}

public record LayoutMap
{
    [JsonProperty("layoutStates")]
    public List<LayoutState> LayoutStates;
}